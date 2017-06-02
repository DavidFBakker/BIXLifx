using System;
using System.Text;

namespace LIFXDevices
{
    /// <summary>
    /// Sent by a device to provide the current light state
    /// </summary>
    public class LightStateResponse : LifxResponse
    {
        public LightStateResponse()
        {
            
        }
        internal LightStateResponse(byte[] payload) : base()
        {
            Hue = BitConverter.ToUInt16(payload, 0);
            Saturation = BitConverter.ToUInt16(payload, 2);
            Brightness = BitConverter.ToUInt16(payload, 4);
            Kelvin = BitConverter.ToUInt16(payload, 6);
            IsOn = BitConverter.ToUInt16(payload, 10) > 0;
            Label = Encoding.UTF8.GetString(payload, 12, 32).Replace("\0", "");
        }
        /// <summary>
        /// Hue
        /// </summary>
        public UInt16 Hue { get; set; }
        /// <summary>
        /// Saturation (0=desaturated, 65535 = fully saturated)
        /// </summary>
        public UInt16 Saturation { get; set; }
        /// <summary>
        /// Brightness (0=off, 65535=full brightness)
        /// </summary>
        public UInt16 Brightness { get; set; }
        /// <summary>
        /// Bulb color temperature
        /// </summary>
        public UInt16 Kelvin { get; set; }
        /// <summary>
        /// Power state
        /// </summary>
        public bool IsOn { get;  set; }
        /// <summary>
        /// Light label
        /// </summary>
        public string Label { get;  set; }
    }
}