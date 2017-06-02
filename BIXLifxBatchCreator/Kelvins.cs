using System.Collections.Generic;
using System.Windows.Media;
using Util;

namespace BIXLifxBatchCreator
{
    public static class Kelvin
    {
        private static Dictionary<string, Color> _kelvins;

        public static Dictionary<string, Color> Kelvins
        {
            get
            {
                if (_kelvins == null)
                {
                    _kelvins = new Dictionary<string, Color>();
                    foreach (var k in KelvinColor.KelvinColors)
                    {
                        _kelvins[k.Key] = Color.FromRgb((byte) k.Value.R, (byte)k.Value.G, (byte)k.Value.B);
                    }
                }
                return _kelvins;
            }
        }
    }
}