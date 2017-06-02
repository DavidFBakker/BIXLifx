using System;
using System.Linq;

namespace LIFXDevices
{
    public class LightBulb
    {
        public LightBulb()
        {
           
        }

        /// <summary>
        ///     Hostname for the device
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        ///     Light label
        /// </summary>
        public string Label
        { get; set; }

        /// <summary>
        ///     Service ID
        /// </summary>
        public byte Service { get; set; }

        /// <summary>
        ///     Service port
        /// </summary>
        public uint Port { get; set; }

        public DateTime LastSeen { get; set; }

        /// <summary>
        ///     Gets the MAC address
        /// </summary>
        public byte[] MacAddress { get; set; }

        /// <summary>
        ///     Gets the MAC address
        /// </summary>
        public string MacAddressName
        {
            get
            {
                return MacAddress == null
                    ? null
                    : string.Join(":", MacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray());
            }
        }

        public ushort Hue
        { get; set; }

        /// <summary>
        ///     Saturation (0=desaturated, 65535 = fully saturated)
        /// </summary>
        public ushort Saturation
        { get; set; }

        /// <summary>
        ///     Brightness (0=off, 65535=full brightness)
        /// </summary>
        public ushort Brightness
        { get; set; }

        /// <summary>
        ///     Bulb color temperature
        /// </summary>
        public ushort Kelvin
        { get; set; }

        /// <summary>
        ///     Power state
        /// </summary>
        public bool IsOn { get; set; }

        public string PowerState
        { get; set; }

    }
}