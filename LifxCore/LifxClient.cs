using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using LIFXDevices;
using Util;

namespace LifxCore
{
    public partial class LifxClient : IDisposable
    {
        private const int Port = 56700;
        private UdpClient _socket;
        public IPAddress ListenAddress;
        private bool _isRunning;

        public LifxClient(string addressStartWith = "192.168.85")
        {
            Log.Info("Starting LifxClient");
            taskCompletions=new Dictionary<uint, Action<LifxResponse>>();
            var su = NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(i => i.GetIPProperties().UnicastAddresses).Where(a => a.Address != null).ToList();

            var ip = NetworkInterface.GetAllNetworkInterfaces()
                .SelectMany(i => i.GetIPProperties().UnicastAddresses).FirstOrDefault(a => a.Address != null && !String.IsNullOrEmpty(a.Address.MapToIPv4().ToString()) &&
                            a.Address.MapToIPv4().ToString().StartsWith(addressStartWith));

            if (ip == null)
            {
                Log.Error($"Cannot find IP address starting with {addressStartWith}");
            }

            ListenAddress = ip.Address.MapToIPv4();
               
            //foreach (var ip in ips)
            //{
            //    if (ip.ToString().StartsWith("192.168.8"))
            //    {
            //        ListenAddress = ip;
            //        break;
            //    }
            //}

            var endPoint = new IPEndPoint(ListenAddress, Port);
            
            _socket = new UdpClient(endPoint)
            {
                Client = {Blocking = false,  EnableBroadcast = true,DontFragment = true}
            };

            _socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            _socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
            StartReceiveLoop();
            Log.Info("Started LifxClient");
        }

        private void StartReceiveLoop()
        {
            Log.Info("LifxClient starting receiveloop");
            _isRunning = true;
            Task.Run(async () =>
            {
                while (_isRunning)
                    try
                    {
                        var result = await _socket.ReceiveAsync().ConfigureAwait(false);
                        if (result.Buffer.Length > 0)
                        {
                            HandleIncomingMessages(result.Buffer, result.RemoteEndPoint);
                        }
                    }
                    catch { }
            });
        }


       

        private void HandleIncomingMessages(byte[] data, System.Net.IPEndPoint endpoint)
        {
            var remote = endpoint;
            var msg = ParseMessage(data);
            if (msg.Type == MessageType.DeviceStateService)
            {
                ProcessDeviceDiscoveryMessage(remote.Address, remote.Port, msg);
            }
            else
            {
                if (taskCompletions.ContainsKey(msg.Source))
                {
                    var tcs = taskCompletions[msg.Source];
                    tcs(msg);
                }
                else
                {
                    switch (msg.Type)
                    {
                        case MessageType.LightState:
                            if (DiscoveredBulbs.ContainsKey(remote.Address.ToString()))
                            {
                                var bulb = DiscoveredBulbs[remote.Address.ToString()];

                                bulb.SetState((LightStateResponse)msg);
                            }

                            break;
                        default:
                            Log.Info($"DID not proccess received msgtype {msg.Type} from {remote}");

                            break;
                    }
                }
            }
          //  Log.Info($"Received msgtype {msg.Type} from {remote.ToString()}:{string.Join(",", (from a in data select a.ToString("X2")).ToArray())}");
                
        }

        private LifxResponse ParseMessage(byte[] packet)
        {
            using (MemoryStream ms = new MemoryStream(packet))
            {
                var header = new FrameHeader();
                BinaryReader br = new BinaryReader(ms);
                //frame
                var size = br.ReadUInt16();
                if (packet.Length != size || size < 36)
                    throw new Exception("Invalid packet");
                var a = br.ReadUInt16(); //origin:2, reserved:1, addressable:1, protocol:12
                var source = br.ReadUInt32();
                //frame address
                byte[] target = br.ReadBytes(8);
                header.TargetMacAddress = target;
                ms.Seek(6, SeekOrigin.Current); //skip reserved
                var b = br.ReadByte(); //reserved:6, ack_required:1, res_required:1, 
                header.Sequence = br.ReadByte();
                //protocol header
                var nanoseconds = br.ReadUInt64();
                header.AtTime = StateHostFirmwareResponse.Epoch.AddMilliseconds(nanoseconds * 0.000001);
                var type = (MessageType)br.ReadUInt16();
                ms.Seek(2, SeekOrigin.Current); //skip reserved
                byte[] payload = null;
                if (size > 36)
                    payload = br.ReadBytes(size - 36);
                return LifxResponse.Create(header, type, source, payload);
            }
        }

        /// <summary>
        /// Disposes the client
        /// </summary>
        public void Dispose()
        {
            _isRunning = false;
            _socket.Dispose();
        }
    }
}
