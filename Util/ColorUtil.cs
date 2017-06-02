using System;

namespace Util
{
    public static class ColorUtil
    {
        public static string HSBtoHEX(float hue, float saturation, float brightness)
        {
            int r = 0, g = 0, b = 0;
            if (saturation == 0)
            {
                r = g = b = (int) (brightness * 255.0f + 0.5f);
            }
            else
            {
                var h = (hue - (float) Math.Floor(hue)) * 6.0f;
                var f = h - (float) Math.Floor(h);
                var p = brightness * (1.0f - saturation);
                var q = brightness * (1.0f - saturation * f);
                var t = brightness * (1.0f - saturation * (1.0f - f));
                switch ((int) h)
                {
                    case 0:
                        r = (int) (brightness * 255.0f + 0.5f);
                        g = (int) (t * 255.0f + 0.5f);
                        b = (int) (p * 255.0f + 0.5f);
                        break;
                    case 1:
                        r = (int) (q * 255.0f + 0.5f);
                        g = (int) (brightness * 255.0f + 0.5f);
                        b = (int) (p * 255.0f + 0.5f);
                        break;
                    case 2:
                        r = (int) (p * 255.0f + 0.5f);
                        g = (int) (brightness * 255.0f + 0.5f);
                        b = (int) (t * 255.0f + 0.5f);
                        break;
                    case 3:
                        r = (int) (p * 255.0f + 0.5f);
                        g = (int) (q * 255.0f + 0.5f);
                        b = (int) (brightness * 255.0f + 0.5f);
                        break;
                    case 4:
                        r = (int) (t * 255.0f + 0.5f);
                        g = (int) (p * 255.0f + 0.5f);
                        b = (int) (brightness * 255.0f + 0.5f);
                        break;
                    case 5:
                        r = (int) (brightness * 255.0f + 0.5f);
                        g = (int) (p * 255.0f + 0.5f);
                        b = (int) (q * 255.0f + 0.5f);
                        break;
                }
            }

            var hex = $"#{Convert.ToByte(r):X2}{Convert.ToByte(g):X2}{Convert.ToByte(b):X2}";
            return hex;
        }
    }
}