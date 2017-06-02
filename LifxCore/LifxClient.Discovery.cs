using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LIFXDevices;

namespace LifxCore
{
    public partial class LifxClient : IDisposable
    {
        private static readonly Random randomizer = new Random();
        private readonly Dictionary<string, LightBulb> DiscoveredBulbs = new Dictionary<string, LightBulb>();
        private CancellationTokenSource _DiscoverCancellationSource;
        private readonly IList<LightBulb> devices = new List<LightBulb>();

        private uint discoverSourceID;

        /// <summary>
        ///     Event fired when a LIFX bulb is discovered on the network
        /// </summary>
        public event EventHandler<DeviceDiscoveryEventArgs> DeviceDiscovered;

        /// <summary>
        ///     Event fired when a LIFX bulb hasn't been seen on the network for a while (for more than 5 minutes)
        /// </summary>
        public event EventHandler<DeviceDiscoveryEventArgs> DeviceLost;


        private async void ProcessDeviceDiscoveryMessage(IPAddress remoteAddress, int remotePort, LifxResponse msg)
        {
            var id = msg.Header.TargetMacAddressName; //remoteAddress.ToString()
            if (DiscoveredBulbs.ContainsKey(id)) //already discovered
            {
                DiscoveredBulbs[id].LastSeen = DateTime.UtcNow; //Update datestamp
                DiscoveredBulbs[id].HostName = remoteAddress.ToString(); //Update hostname in case IP changed

                return;
            }
            if (msg.Source != discoverSourceID || //did we request the discovery?
                _DiscoverCancellationSource == null ||
                _DiscoverCancellationSource.IsCancellationRequested) //did we cancel discovery?
                return;

            var device = new LightBulb
            {
                HostName = remoteAddress.ToString(),
                Service = msg.Payload[0],
                Port = BitConverter.ToUInt32(msg.Payload, 1),
                LastSeen = DateTime.UtcNow,
                MacAddress = msg.Header.TargetMacAddress
            };

            await AddDeviceAsync(device, id).ConfigureAwait(false);

            if (DeviceDiscovered != null)
                DeviceDiscovered(this, new DeviceDiscoveryEventArgs { LightBulb = device });
        }


        /// <summary>
        ///     Begins searching for bulbs.
        /// </summary>
        /// <seealso cref="DeviceDiscovered" />
        /// <seealso cref="DeviceLost" />
        /// <seealso cref="StopDeviceDiscovery" />
        public void StartDeviceDiscovery()
        {
            if (_DiscoverCancellationSource != null && !_DiscoverCancellationSource.IsCancellationRequested)
                return;
            _DiscoverCancellationSource = new CancellationTokenSource();
            var token = _DiscoverCancellationSource.Token;
            var source = discoverSourceID = (uint)randomizer.Next(int.MaxValue);

            //Start discovery thread
            Task.Run(async () =>
            {
                Debug.WriteLine("Sending GetServices");
                var header = new FrameHeader
                {
                    Identifier = source
                };
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await BroadcastMessageAsync<UnknownResponse>(null, header, MessageType.DeviceGetService, null);
                    }
                    catch
                    {
                    }
                    await Task.Delay(10*1000, token);
                    var lostDevices = devices.Where(d => (DateTime.UtcNow - d.LastSeen).TotalMinutes > 5).ToArray();
                    if (lostDevices.Any())
                        foreach (var device in lostDevices)
                        {
                            devices.Remove(device);
                            DiscoveredBulbs.Remove(device.MacAddressName);
                            if (DeviceLost != null)
                                DeviceLost(this, new DeviceDiscoveryEventArgs { LightBulb = device });
                        }
                }
            }, token);
        }


        private async Task AddDeviceAsync(LightBulb device, string id)
        {

            if (devices.Select(a => a.MacAddress).Contains(device.MacAddress) || DiscoveredBulbs.ContainsKey(id))
                return;

            DiscoveredBulbs[id] = device;
            devices.Add(device);

            device.SetState( await GetLightStateAsync(device).ConfigureAwait(false));
           // var d = devices.First(a => a.MacAddress == device.MacAddress);
        }


        public sealed class DeviceDiscoveryEventArgs : EventArgs
        {
            /// <summary>
            ///     The device the event relates to
            /// </summary>
            public LightBulb LightBulb { get; internal set; }
        }

    }
}