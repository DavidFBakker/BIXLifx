using System.Collections.Generic;

namespace Util
{
    public static class KelvinColor
    {
        private static Dictionary<string, RGB> _kelvinColors;

        public static Dictionary<string, RGB> KelvinColors
        {
            get
            {
                if (_kelvinColors == null)
                {
                    _kelvinColors = new Dictionary<string, RGB>
                    {
                        ["2500"] = new RGB(255, 166, 69),
                        ["2600"] = new RGB(255, 170, 77),
                        ["2700"] = new RGB(255, 169, 87),
                        ["2800"] = new RGB(255, 178, 91),
                        ["2900"] = new RGB(255, 177, 101),
                        ["3000"] = new RGB(255, 185, 105),
                        ["3100"] = new RGB(255, 184, 114),
                        ["3200"] = new RGB(255, 192, 118),
                        ["3300"] = new RGB(255, 190, 126),
                        ["3400"] = new RGB(255, 198, 130),
                        ["3500"] = new RGB(255, 196, 137),
                        ["3600"] = new RGB(255, 203, 141),
                        ["3700"] = new RGB(255, 201, 148),
                        ["3800"] = new RGB(255, 208, 151),
                        ["3900"] = new RGB(255, 206, 159),
                        ["4000"] = new RGB(255, 213, 161),
                        ["4100"] = new RGB(255, 211, 168),
                        ["4200"] = new RGB(255, 217, 171),
                        ["4300"] = new RGB(255, 215, 177),
                        ["4400"] = new RGB(255, 221, 180),
                        ["4500"] = new RGB(255, 219, 186),
                        ["4600"] = new RGB(255, 225, 188),
                        ["4700"] = new RGB(255, 223, 194),
                        ["4800"] = new RGB(255, 228, 196),
                        ["4900"] = new RGB(255, 227, 202),
                        ["5000"] = new RGB(255, 231, 204),
                        ["5100"] = new RGB(255, 230, 210),
                        ["5200"] = new RGB(255, 234, 211),
                        ["5300"] = new RGB(255, 233, 217),
                        ["5400"] = new RGB(255, 237, 218),
                        ["5500"] = new RGB(255, 236, 224),
                        ["5600"] = new RGB(255, 239, 225),
                        ["5700"] = new RGB(255, 239, 230),
                        ["5800"] = new RGB(255, 241, 231),
                        ["5900"] = new RGB(255, 242, 236),
                        ["6000"] = new RGB(255, 244, 237),
                        ["6100"] = new RGB(255, 244, 242),
                        ["6200"] = new RGB(255, 246, 243),
                        ["6300"] = new RGB(255, 246, 248),
                        ["6400"] = new RGB(255, 248, 248),
                        ["6500"] = new RGB(255, 249, 253),
                        ["6600"] = new RGB(255, 249, 253),
                        ["6700"] = new RGB(252, 247, 255),
                        ["6800"] = new RGB(252, 248, 255),
                        ["6900"] = new RGB(247, 245, 255),
                        ["7000"] = new RGB(247, 245, 255),
                        ["7100"] = new RGB(243, 242, 255),
                        ["7200"] = new RGB(243, 243, 255),
                        ["7300"] = new RGB(239, 240, 255),
                        ["7400"] = new RGB(239, 240, 255),
                        ["7500"] = new RGB(235, 238, 255),
                        ["7600"] = new RGB(236, 238, 255),
                        ["7700"] = new RGB(231, 236, 255),
                        ["7800"] = new RGB(233, 236, 255),
                        ["7900"] = new RGB(228, 234, 255),
                        ["8000"] = new RGB(229, 233, 255),
                        ["8100"] = new RGB(225, 232, 255),
                        ["8200"] = new RGB(227, 232, 255),
                        ["8300"] = new RGB(222, 230, 255),
                        ["8400"] = new RGB(224, 230, 255),
                        ["8500"] = new RGB(220, 229, 255),
                        ["8600"] = new RGB(221, 228, 255),
                        ["8700"] = new RGB(217, 227, 255),
                        ["8800"] = new RGB(219, 226, 255),
                        ["8900"] = new RGB(215, 226, 255),
                        ["9000"] = new RGB(217, 225, 255)
                    };
                }

                return _kelvinColors;
            }
            set => _kelvinColors = value;
        }

        public class RGB
        {
            public RGB(int r, int g, int b)
            {
                R = r;
                G = g;
                B = b;
            }

            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }
        }
    }
}