using System;
using System.Threading.Tasks;
using LIFXDevices;

namespace LifxCore
{
    public partial class LifxClient : IDisposable
    {
        /// <summary>
        ///     Sets the device power state
        /// </summary>
        /// <param name="device"></param>
        /// <param name="isOn"></param>
        /// <returns></returns>
        public async Task SetDevicePowerStateAsync(LightBulb device, bool isOn)
        {
            var header = new FrameHeader
            {
                Identifier = (uint) randomizer.Next(),
                AcknowledgeRequired = true
            };

            await BroadcastMessageAsync<AcknowledgementResponse>(device.HostName, header,
                MessageType.DeviceSetPower, (ushort) (isOn ? 65535 : 0)).ConfigureAwait(false);
        }
    }
}