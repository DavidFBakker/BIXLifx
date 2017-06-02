using System;
using System.Linq;

namespace LIFXDevices
{
    public class FrameHeader
    {
        public bool AcknowledgeRequired;
        public DateTime AtTime;
        public uint Identifier;
        public bool ResponseRequired;
        public byte Sequence;
        public byte[] TargetMacAddress;

        public FrameHeader()
        {
            Identifier = 0;
            Sequence = 0;
            AcknowledgeRequired = false;
            ResponseRequired = false;
            TargetMacAddress = new byte[] {0, 0, 0, 0, 0, 0, 0, 0};
            AtTime = DateTime.MinValue;
        }

        public string TargetMacAddressName
        {
            get
            {
                if (TargetMacAddress == null) return null;
                return string.Join(":", TargetMacAddress.Take(6).Select(tb => tb.ToString("X2")).ToArray());
            }
        }
    }
}