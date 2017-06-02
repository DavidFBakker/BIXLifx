using System;
using System.Collections.Generic;
using System.Drawing;

namespace Util
{
    public static class BIXColors
    {
        private static Dictionary<string, BIXColor> _colors;
        private static List<string> _availColors;

        public static List<string> AvailColors
        {
            get
            {

                if (_availColors==null)
                {
                    _availColors = new List<string>
                    {
                        "AliceBlue",
                        "AntiqueWhite",
                        "Aqua",
                        "Aquamarine",
                        "Azure",
                        "Beige",
                        "Bisque",
                        "Black",
                        "BlanchedAlmond",
                        "Blue",
                        "BlueViolet",
                        "Brown",
                        "BurlyWood",
                        "CadetBlue",
                        "Chartreuse",
                        "Chocolate",
                        "Coral",
                        "CornflowerBlue",
                        "Cornsilk",
                        "Crimson",
                        "Cyan",
                        "DarkBlue",
                        "DarkCyan",
                        "DarkGoldenrod",
                        "DarkGray",
                        "DarkGreen",
                        "DarkKhaki",
                        "DarkMagenta",
                        "DarkOliveGreen",
                        "DarkOrange",
                        "DarkOrchid",
                        "DarkRed",
                        "DarkSalmon",
                        "DarkSeaGreen",
                        "DarkSlateBlue",
                        "DarkSlateGray",
                        "DarkTurquoise",
                        "DarkViolet",
                        "DeepPink",
                        "DeepSkyBlue",
                        "DimGray",
                        "DodgerBlue",
                        "Firebrick",
                        "FloralWhite",
                        "ForestGreen",
                        "Fuchsia",
                        "Gainsboro",
                        "GhostWhite",
                        "Gold",
                        "Goldenrod",
                        "Gray",
                        "Green",
                        "GreenYellow",
                        "Honeydew",
                        "HotPink",
                        "IndianRed",
                        "Indigo",
                        "Ivory",
                        "Khaki",
                        "Lavender",
                        "LavenderBlush",
                        "LawnGreen",
                        "LemonChiffon",
                        "LightBlue",
                        "LightCoral",
                        "LightCyan",
                        "LightGoldenrodYellow",
                        "LightGray",
                        "LightGreen",
                        "LightPink",
                        "LightSalmon",
                        "LightSeaGreen",
                        "LightSkyBlue",
                        "LightSlateGray",
                        "LightSteelBlue",
                        "LightYellow",
                        "Lime",
                        "LimeGreen",
                        "Linen",
                        "Magenta",
                        "Maroon",
                        "MediumAquamarine",
                        "MediumBlue",
                        "MediumOrchid",
                        "MediumPurple",
                        "MediumSeaGreen",
                        "MediumSlateBlue",
                        "MediumSpringGreen",
                        "MediumTurquoise",
                        "MediumVioletRed",
                        "MidnightBlue",
                        "MintCream",
                        "MistyRose",
                        "Moccasin",
                        "NavajoWhite",
                        "Navy",
                        "OldLace",
                        "Olive",
                        "OliveDrab",
                        "Orange",
                        "OrangeRed",
                        "Orchid",
                        "PaleGoldenrod",
                        "PaleGreen",
                        "PaleTurquoise",
                        "PaleVioletRed",
                        "PapayaWhip",
                        "PeachPuff",
                        "Peru",
                        "Pink",
                        "Plum",
                        "PowderBlue",
                        "Purple",
                        "Red",
                        "RosyBrown",
                        "RoyalBlue",
                        "SaddleBrown",
                        "Salmon",
                        "SandyBrown",
                        "SeaGreen",
                        "SeaShell",
                        "Sienna",
                        "Silver",
                        "SkyBlue",
                        "SlateBlue",
                        "SlateGray",
                        "Snow",
                        "SpringGreen",
                        "SteelBlue",
                        "Tan",
                        "Teal",
                        "Thistle",
                        "Tomato",
                        "Transparent",
                        "Turquoise",
                        "Violet",
                        "Wheat",
                        "White",
                        "WhiteSmoke",
                        "Yellow",
                        "YellowGreen"
                    };

                    _availColors.AddRange(KelvinColor.KelvinColors.Keys);
                }
                return _availColors;
            }
            set { _availColors = value; }
        }


        public static Dictionary<string, BIXColor> Colors
        {
            get
            {
                if (_colors == null)
                {
                    _colors = new Dictionary<string, BIXColor>();
                    int test;

                    foreach (var color in AvailColors)
                    {
                        Color colorValue;

                        if (Int32.TryParse(color, out test))
                        {
                            colorValue = Kelvin.Kelvins[color];
                        }
                        else
                        {
                            colorValue = Color.FromName(color);

                        }
                       
                        var bixColor = new BIXColor
                        {
                            Name = color.ToLower(),
                            Hue = colorValue.GetHue(),
                            Saturation = colorValue.GetSaturation(),
                            Brightness = colorValue.GetBrightness(),
                            Hex = $"#{colorValue.R:X2}{colorValue.G:X2}{colorValue.B:X2}"
                        };
                        _colors[bixColor.Name] = bixColor;
                    }
                }
                return _colors;
            }
            set => _colors = value;
        }

        public static ushort GetHueFromHEX(this string Hex)
        {
            var red = Convert.ToInt32(Hex.Substring(1, 2), 16);
            var green = Convert.ToInt32(Hex.Substring(3, 2), 16);
            var blue = Convert.ToInt32(Hex.Substring(5, 2), 16);

            var color = Color.FromArgb(red, green, blue);
            return (ushort)color.GetHue();
            
        }

        public static ushort GetBrightnessFromHEX(this string Hex)
        {
            var red = Convert.ToInt32(Hex.Substring(1, 2), 16);
            var green = Convert.ToInt32(Hex.Substring(3, 2), 16);
            var blue = Convert.ToInt32(Hex.Substring(5, 2), 16);

            var color = Color.FromArgb(red, green, blue);
            var ret = (ushort)(color.GetBrightness() * 65535);
            return ret;
            
        }

        public static ushort GetSaturationFromHEX(this string Hex)
        {
            var red = Convert.ToInt32(Hex.Substring(1, 2), 16);
            var green = Convert.ToInt32(Hex.Substring(3, 2), 16);
            var blue = Convert.ToInt32(Hex.Substring(5, 2), 16);

            var color = Color.FromArgb(red, green, blue);
            var ret = (ushort) (color.GetSaturation() * 65535);
            return ret;
        }

        public static BIXColor FromHtml(this string html)
        {
            //FIXME
            return Colors[AvailColors[0]];
        }
    }
}