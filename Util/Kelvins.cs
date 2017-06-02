using System.Collections.Generic;
using System.Drawing;

namespace Util
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
                        _kelvins[k.Key] = Color.FromArgb(k.Value.R, k.Value.G, k.Value.B);
                    }
                }
                return _kelvins;
            }
        }
    }
}