using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LIFXDevices;

namespace LifxCore
{
    public partial class LifxClient
    {
        private static readonly Random Randomizer = new Random();

        private readonly Dictionary<uint, Action<LifxResponse>> taskCompletions =
            new Dictionary<uint, Action<LifxResponse>>();

        public Task<LightStateResponse> GetLightStateAsync(LightBulb bulb)
        {
            var header = new FrameHeader
            {
                Identifier = (uint) Randomizer.Next(),
                AcknowledgeRequired = false
            };

            return BroadcastMessageAsync<LightStateResponse>(
                bulb.HostName, header, MessageType.LightGet);
        }

        public async Task UpdateLightStateAsync(LightBulb bulb)
        {
            bulb.SetState( await GetLightStateAsync(bulb).ConfigureAwait(false));
        }
      
        public async Task SetColorAsync(LightBulb bulb,
            TimeSpan transitionDuration)
        {
            if (transitionDuration.TotalMilliseconds > UInt32.MaxValue ||
                transitionDuration.Ticks < 0)
                throw new ArgumentOutOfRangeException("transitionDuration");
            if (bulb.Kelvin < 2500 || bulb.Kelvin > 9000)
            {
                throw new ArgumentOutOfRangeException("kelvin", "Kelvin must be between 2500 and 9000");
            }

            System.Diagnostics.Debug.WriteLine("Setting color to {0}", bulb.HostName);
            FrameHeader header = new FrameHeader()
            {
                Identifier = (uint)randomizer.Next(),
                AcknowledgeRequired = true
            };
            UInt32 duration = (UInt32)transitionDuration.TotalMilliseconds;
            
            await BroadcastMessageAsync<AcknowledgementResponse>(bulb.HostName, header,
                MessageType.LightSetColor, (byte)0x00, //reserved
                bulb.Hue, bulb.Saturation, bulb.Brightness, bulb.Kelvin, //HSBK
                duration
            );
        }
    }
}