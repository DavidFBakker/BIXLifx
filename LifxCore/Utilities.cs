﻿using System;

namespace LifxCore
{
    internal static partial class Utilities
    {
        public static ushort[] RgbToHsl(Color rgb)
        {
            // normalize red, green and blue values
            var r = rgb.R / 255.0;
            var g = rgb.G / 255.0;
            var b = rgb.B / 255.0;

            var max = Math.Max(r, Math.Max(g, b));
            var min = Math.Min(r, Math.Min(g, b));

            var h = 0.0;
            if (max == r && g >= b)
                h = 60 * (g - b) / (max - min);
            else if (max == r && g < b)
                h = 60 * (g - b) / (max - min) + 360;
            else if (max == g)
                h = 60 * (b - r) / (max - min) + 120;
            else if (max == b)
                h = 60 * (r - g) / (max - min) + 240;

            var s = max == 0 ? 0.0 : 1.0 - min / max;
            return new[]
            {
                (ushort) (h / 360 * 65535),
                (ushort) (s * 65535),
                (ushort) (max * 65535)
            };
        }
    }
}