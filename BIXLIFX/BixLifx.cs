using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxCore;
using LIFXDevices;
using Newtonsoft.Json;
using Util;
using static Util.Messaging;
using static System.String;

namespace BIXLIFX
{
    internal enum ReturnCodes
    {
        Return,
        ReturnNoReponse,
        Continue
    }

    public static partial class BixLifx
    {
        private const int CommandSendsDef = 5;
        public static ObservableCollection<LightBulb> Bulbs = new ObservableCollection<LightBulb>();

        private static readonly int TaskTimeoutDefault = 500;
        private static readonly int TaskTimeoutIncDefault = 500;


        private static LifxClient _client;
        private static readonly ushort KelvinLow = 2500;
        private static readonly ushort KelvinHigh = 9000;

        private static StringBuilder responseBuilder = new StringBuilder();


        private static List<string> Labels
        {
            get
            {
                if (Bulbs == null)
                    return new List<string>();

                return
                    Bulbs.Where(a => !IsNullOrEmpty(a.Label))
                        .Select(a => a.Label)
                        .ToList();
            }
        }


        public static void Init(bool startHTTPListener = true)
        {
            if (startHTTPListener)
            {
                Log.Info("Starting BixListens");
                var bixListens = new BixListens();
                bixListens.OnHttpEventReceived += BixListens_OnHttpEventReceived;
            }

            _client = new LifxClient();
            Log.Bulb($"Listening on {_client.ListenAddress}");
            _client.DeviceDiscovered += Client_DeviceDiscovered;
            _client.DeviceLost += Client_DeviceLost;

            Log.Bulb("Start Device Discovery");
            _client.StartDeviceDiscovery();
        }


        private static async void BixListens_OnHttpEventReceived(object sender, HttpEvent e)
        {
            Log.Bulb($"{e.ID} Received new event");


            var qs = new Dictionary<string, string>();
            foreach (var command in e.QueryString.AllKeys.Where(a => a != null))
                qs[command.ToLower()] = e.QueryString[command];
            foreach (var command in e.QueryString.GetValues(null).Where(a => a != null))
                qs[command.ToLower()] = "";

            var rs = await DoCommands(e.ID,  qs);
            await SendResponseAsync(e.HttpListenerResponse, rs).ConfigureAwait(false);
        }

        public static async Task<string> DoCommands(string ID,  Dictionary<string, string> qs)
        {
            responseBuilder = new StringBuilder();
            var bulbs = new List<LightBulb>();

            var powerstate = "";

            var doReturn = ReturnCodes.Continue;

            var jsonResult = qs.ContainsKey("json");

            var cmdsResult = qs.ContainsKey("cmds");

            var color = new LIFXColor();

            foreach (var command in qs.Keys.Where(a=>!a.Equals("cmds") && !a.Equals("json")))
            {
                Log.Info($"Proccessing command {command}");

                switch (command.ToLower())
                {
                    case "log":
                        responseBuilder.AppendLine(Log.GetMessages());
                        doReturn = ReturnCodes.Return;
                        break;

                    case "status":
                        responseBuilder.AppendLine(BuildStatus());
                        doReturn = ReturnCodes.Return;
                        ;
                        break;

                    case "colors":
                        if (jsonResult)
                        {
                            var json = JsonConvert.SerializeObject(BIXColors.Colors.Select(a=>a.Value));
                            return json;
                        }
                        responseBuilder.AppendLine(ColorsTable);
                        doReturn = ReturnCodes.Return;
                        ;
                        break;

                    case "lights":

                        if (jsonResult)
                        {
                            var json = JsonConvert.SerializeObject(Bulbs);
                            return json;
                        }
                        var lights = Bulbs.Where(a => !IsNullOrEmpty(a.Label)).Select(a => a.Label).OrderBy(b1 => b1)
                            .ToList();
                        foreach (var light1 in lights)
                            responseBuilder.AppendLine(light1);

                        doReturn = ReturnCodes.Return;
                        break;

                    case "updatestate":
                        //awaited so dont try to send a response, cause we dont care to send a response. the HttpListenerResponse will be disposed if waited
                        //await Bulbs.ForEachAsync(bulb => _client.UpdateLightStateAsync(bulb)).ConfigureAwait(false);
                        await Bulbs.ForEachAsync(bulb => _client.UpdateLightStateAsync(bulb)).ConfigureAwait(false);

                        doReturn = ReturnCodes.Return;
                        break;

                    case "buildbulbs":
                        responseBuilder.AppendLine(BuildCreateDevice());
                        doReturn = ReturnCodes.Return;
                        break;

                    case "power":
                        if (!IsNullOrEmpty(qs["power"]))
                            powerstate = qs["power"].ToLower();
                        break;

                    case "dim":
                        if (!IsNullOrEmpty(qs["dim"]))
                        {
                            var dimStr = qs["dim"].ToLower();
                            var colorBrightness = color.Brightness;
                            if (ushort.TryParse(dimStr, out colorBrightness))
                            {                                
                                color.Brightness = (ushort)(colorBrightness * 65535);
                            }
                            else
                            {
                                responseBuilder.AppendLine($"Dim {dimStr} needs to be a whole number");
                                doReturn = ReturnCodes.Return;
                            }
                        }
                        break;
                    case "color":
                        if (!IsNullOrEmpty(qs["color"]))
                            color = GetLIFXColorFromColorString(qs["color"], color.Brightness);
                        if (color == null)
                            doReturn = ReturnCodes.Return;
                        break;

                    case "light":

                        var light = qs["light"];
                        bulbs = GetBulbsFromLabel(light.ToLower());
                        //if (light.ToLower() == "all")
                        //{
                        //    lock (Bulbs)
                        //    {
                        //        bulbs.AddRange(Bulbs);
                        //    }
                        //}
                        //else
                        //{
                        //    var light1 = light;
                        //    var b = Bulbs.FirstOrDefault(
                        //        a => !IsNullOrEmpty(a.Label) && a.Label.ToLower() == light1.ToLower());
                        //    if (b != null)
                        //    {
                        //        bulbs.Add(b);
                        //    }
                        //    else
                        //    {
                        //        var bd = Bulbs
                        //            .Where(a => !IsNullOrEmpty(a.Label) &&
                        //                        a.Label.ToLower().StartsWith(light1.ToLower()))
                        //            .ToList();
                        //        bulbs.AddRange(bd);
                        //    }
                        //}
                        if (!bulbs.Any())
                        {
                            responseBuilder.AppendLine($"Cant find light or light that starts with {light}");
                            doReturn = ReturnCodes.Return;
                        }
                        else
                        {
                            if (jsonResult)
                            {
                                var json = JsonConvert.SerializeObject(bulbs);
                                return json;
                            }
                        }

                        break;
                }
            }


            if (jsonResult || doReturn == ReturnCodes.Return || doReturn == ReturnCodes.ReturnNoReponse ||
                !color.IsSet && powerstate == "" && !cmdsResult)
            {
                Log.Bulb($"{ID} Proccessed event");

                if (doReturn != ReturnCodes.ReturnNoReponse)
                    return responseBuilder.ToString();

                return "";
            }

            foreach (var bulb in bulbs)
            {
                if (color.IsSet)
                {
                    bulb.Brightness = color.Brightness;
                    bulb.Hue = color.Hue;
                    bulb.Saturation = color.Saturation;
                    bulb.Kelvin = color.Kelvin;
                }

                if (powerstate != "")
                    bulb.PowerState = powerstate;
            }

            if (cmdsResult)
            {
                var lifxCommands = JsonConvert.DeserializeObject<List<LIFXCommand>>(qs["cmds"]);
                Log.Bulb($"{ID} Found {lifxCommands.Count} lifxCommands ");

                foreach (var lcommand in lifxCommands.Where(a => !IsNullOrEmpty(a.Label)))
                {
                    int dim = 0;
                    var toBulbs = GetBulbsFromLabel(lcommand.Label);
                    foreach (var b in toBulbs)
                    {                       

                        if (!IsNullOrEmpty(lcommand.Power))
                        {
                            b.PowerState = lcommand.Power.ToLower();
                            powerstate = lcommand.Power;
                        }

                        if (lcommand.Dim != 0)
                        {
                            dim = lcommand.Dim;
                        }

                        if (!IsNullOrEmpty(lcommand.Color))
                        {
                            color = GetLIFXColorFromColorString(lcommand.Color, color.Brightness);
                            b.Brightness = color.Brightness;
                            b.Hue = color.Hue;
                            b.Saturation = color.Saturation;
                            b.Kelvin = color.Kelvin;
                        }
                        if (dim != 0)
                            b.Brightness = (ushort) (dim * 65535);
                        bulbs.Add(b);
                    }
                }
            }


            if (!bulbs.Any())
            {
                Log.Bulb($"{ID} Proccessed event");
                return responseBuilder.ToString();
            }

            ////PowerCommand
            if (!IsNullOrEmpty(powerstate))
                lock (bulbs)
                {
                    foreach (var bulb in bulbs)
                    {
                        var power = SetPowerAsync(ID,bulb);//, powerstate);
                        responseBuilder.AppendLine($"Powered {bulb.Label} from {bulb.PowerState} to {power.Result}");
                    }
                }

            var colorStr = "";


            lock (bulbs)
            {
                foreach (var bulb in bulbs)
                {
                    var t = SetColor(ID, bulb); //, hue, saturation, brightness, kelvin);

                    responseBuilder.AppendLine(
                        $"Set color for bulb {bulb.Label} to color {colorStr} hue: {bulb.Hue} saturation: {bulb.Saturation} brightness: {bulb.Brightness} kelvin: {bulb.Kelvin}");
                }
            }

            //await SendResponseAsync(e.HttpListenerResponse, responseBuilder.ToString()).ConfigureAwait(false);
            Log.Bulb($"{ID} Proccessed event");
            return responseBuilder.ToString();
        }

        private static List<LightBulb> GetBulbsFromLabel(string label)
        {
            var ret = new List<LightBulb>();

            var b = Bulbs.FirstOrDefault(
                a => !IsNullOrEmpty(a.Label) && a.Label.ToLower() == label.ToLower());
            if (b != null)
            {
                ret.Add(b);
            }
            else
            {
                var bd = Bulbs
                    .Where(a => !IsNullOrEmpty(a.Label) &&
                                a.Label.ToLower().StartsWith(label.ToLower()))
                    .ToList();
                ret.AddRange(bd);
            }

            return ret;
        }
       

       
        private static LIFXColor GetLIFXColorFromColorString(string colorStr, ushort dim)
        {
            if (IsNullOrEmpty(colorStr))
            {
                responseBuilder.AppendLine("Color is empty");
                return null;
            }
            colorStr = colorStr.ToLower();
            if (colorStr.StartsWith("#") && colorStr.Length == 7)
            {
                var ret = new LIFXColor
                {
                    Hue = colorStr.GetHueFromHEX(),
                    Saturation = colorStr.GetSaturationFromHEX(),
                    Brightness = colorStr.GetBrightnessFromHEX()
                };

                if (dim != 0)
                    ret.Brightness = dim;

                return ret;
            }

            ushort kelvin;
            if (colorStr.Length == 4 && ushort.TryParse(colorStr, out kelvin))
            {
                if (kelvin < KelvinLow || kelvin > KelvinHigh)
                {
                    responseBuilder.AppendLine($"Kelvin {kelvin} out of range (2500-9000)");
                    return null;
                }
                var ret = new LIFXColor
                {
                    Hue = 255,
                    Saturation = 255,
                    Kelvin = kelvin
                };
                if (dim != 0)
                    ret.Brightness = dim;
                return ret;
            }

            if (BIXColors.Colors.ContainsKey(colorStr))
            {
                var color = BIXColors.Colors[colorStr];
                var ret = new LIFXColor
                {
                    Hue = color.LIFXHue,
                    Saturation = color.LIFXSaturation,
                    Brightness = color.LIFXBrightness
                };
                if (dim != 0)
                    ret.Brightness = dim;
                return ret;
            }
            responseBuilder.AppendLine($"Cannot find color {colorStr}");
            return null;
        }

        private static void Client_DeviceLost(object sender, LifxClient.DeviceDiscoveryEventArgs e)
        {
            lock (Bulbs)
            {
                var bulb = e.LightBulb; // as LightBulb;

                if (IsNullOrEmpty(bulb?.Label) || !Labels.Contains(bulb.Label)) return;

                if (bulb.LastSeen.AddMinutes(10) > DateTime.Now)
                    return;

                Log.Bulb($"Removing bulb {bulb.Label}");
                try
                {
                    Bulbs.Remove(bulb);
                }
                catch (Exception ex)
                {
                    Log.Error($"Removing bulb {bulb.Label} exception {ex.Message}");
                }
            }
        }

        private static void Client_DeviceDiscovered(object sender, LifxClient.DeviceDiscoveryEventArgs e)
        {
            lock (Bulbs)
            {
                var bulb = e.LightBulb; // as LightBulb;

                if (IsNullOrEmpty(bulb?.Label) || Labels.Contains(bulb.Label)) return;

                Log.Bulb($"Adding bulb {bulb.Label}");

                try
                {
                    Bulbs.Add(bulb);
                }
                catch (Exception ex)
                {
                    Log.Error($"Adding bulb {bulb.Label} exception {ex.Message}");
                }
            }
        }

        private class LIFXColor
        {
            private ushort _brightness;
            private ushort _hue;
            private ushort _kelvin;
            private ushort _saturation;
            public bool IsSet { get; set; }

            public ushort Hue
            {
                get => _hue;
                set
                {
                    IsSet = true;
                    _hue = value;
                }
            }

            public ushort Saturation
            {
                get => _saturation;
                set
                {
                    IsSet = true;
                    _saturation = value;
                }
            }

            public ushort Brightness
            {
                get => _brightness;
                set
                {
                    IsSet = true;
                    _brightness = value;
                }
            }

            public ushort Kelvin
            {
                get => _kelvin;
                set
                {
                    IsSet = true;
                    _kelvin = value;
                }
            }
        }
    }
}