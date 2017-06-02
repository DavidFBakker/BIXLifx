using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LifxCore;
using LIFXDevices;
using Util;
using static System.String;

namespace BIXLIFX
{
    public static partial class BixLifx
    {
        private static async Task SetColor(string eventID,LightBulb bulb)//, LightBulb command)//ushort hue, ushort saturation, ushort brightness,ushort kelvin)
        {

            var taskTimeOut = Config.GetAppSetting("TaskTimeout", TaskTimeoutDefault);
            var taskTimeOutInc = Config.GetAppSetting("TaskTimeoutInc", TaskTimeoutIncDefault);
            var commandSends = Config.GetAppSetting("CommandSends", CommandSendsDef);
            
            //if (kelvin == ckelvin && hue == chue && saturation == csaturation && brightness == cbrightness)
            //    return;
            if (bulb.Kelvin < KelvinLow)
                bulb.Kelvin = KelvinLow;

            if (bulb.Kelvin > KelvinHigh)
                bulb.Kelvin = KelvinHigh;

            for (var count = 0; count < commandSends; ++count)
            {                             
                var rr = taskTimeOut + (taskTimeOutInc * count);
                var to = GetTS(rr);

                Log.Bulb($"{eventID} SetColor {bulb.Label} Try: {count}/{commandSends} TimeOut: {to}");
                
                var res = await _client.SetColorAsync(bulb, new TimeSpan(0)).TimeoutAfter(to).ConfigureAwait(false); ;
                if (!res) continue;
                break;                
            }

            Log.Bulb($"{eventID} SetColor {bulb.Label} Done");
        }
        private static async Task<string> SetPowerAsync(string eventid,LightBulb bulb)//, string powerState)
        {
            var taskTimeOut = Config.GetAppSetting("TaskTimeout", TaskTimeoutDefault);
            var taskTimeOutInc = Config.GetAppSetting("TaskTimeoutInc", TaskTimeoutIncDefault);
            var commandSends = Config.GetAppSetting("CommandSends", CommandSendsDef);

          
            if (bulb.PowerState.ToLower() == "toggle")
            {
                bulb.PowerState = bulb.IsOn ? "off" : "on";
            }

            for (var count = 0; count < commandSends; ++count)
            {
                var rr = taskTimeOut + (taskTimeOutInc * count);
                var to =GetTS(rr);
                Log.Bulb($"{eventid} SetPower {bulb.Label} {bulb.PowerState} Try: {count}/{commandSends} TimeOut: {to}");
                var res = await _client.SetDevicePowerStateAsync(bulb, bulb.PowerState == "on").TimeoutAfter(to).ConfigureAwait(false);
                if (!res) continue;
                break;                
            }

            Log.Bulb($"{eventid} SetPower {bulb.Label} {bulb.PowerState} Done");

            return bulb.PowerState;
        }
        public static string ColorsTable
        {
            get
            {
                var colors = new StringBuilder();
                colors.AppendLine("<table>");
                foreach (var key in BIXColors.Colors.Keys)
                {
                    var bixColor = BIXColors.Colors[key];
                    colors.AppendLine(bixColor.TableRow);
                }
                colors.AppendLine("</table>");
                return colors.ToString();
            }
        }

        private static string FontSize(int size)
        {
            return $"style=\"font-family:arial;font-size:{size}px; \"";
        }

        public static TimeSpan GetTS(long miliseconds)
        {
            return new TimeSpan(miliseconds * 10000);
        }

        private static string BuildCreateDevice()
        {
            var ret = new StringBuilder();

            ret.AppendLine("<html>");
            ret.AppendLine("<head>");
            ret.AppendLine("</head>");
            ret.AppendLine("<body>");


            ret.AppendLine(@"public object CreateDev(string device)
{
    var dvRef = hs.GetDeviceRefByName(device);
    if (dvRef > 0)
    {
        hs.WriteLog(""Info"", ""Devce exists"" + dvRef + "" deleting"");
        hs.DeleteDevice(dvRef);
        hs.SaveEventsDevices();
    }

    Scheduler.Classes.DeviceClass deviceClass = hs.NewDeviceEx(device);
    dvRef = deviceClass.get_Ref(hs);
    deviceClass.set_Can_Dim(hs, true);
    deviceClass.set_Location(hs, ""LIFX"");
    deviceClass.set_Location2(hs, device);
    deviceClass.set_Code(hs, device);
    deviceClass.set_Device_Type_String(hs, ""LIFX Bulb"");
    deviceClass.MISC_Set(hs, Enums.dvMISC.SHOW_VALUES);

    CreatePair(dvRef, ""Dim"");
    CreatePair(dvRef, ""On"");
    CreatePair(dvRef, ""Off"");

   
    var newevent = hs.NewEventEx(device + "" On"", device, ""Event stype"");
    var capiOn = GetCAPI(dvRef, ""On"");

    var newevent = hs.NewEventEx(device + "" On"", device, ""Event stype"");
    hs.AddDeviceActionToEvent(newevent, capiOn);
    hs.AddDeviceActionToEvent(newevent, capiOn);
    hs.EnableEventByRef(newevent);

    hs.WriteLog(""Info"", ""Created device "" + device + "" "" + dvRef);
    hs.SaveEventsDevices();

    return 0;
}

private void CreatePair(int dvRef, string command)
{
    HomeSeerAPI.VSVGPairs.VSPair VSPair = new HomeSeerAPI.VSVGPairs.VSPair(ePairStatusControl.Both);
    HomeSeerAPI.VSVGPairs.VGPair VGPair = new HomeSeerAPI.VSVGPairs.VGPair();


    switch (command.ToLower())
    {
        case ""on"":
            VSPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
            VSPair.Render = Enums.CAPIControlType.Button;

            VGPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
            VGPair.Set_Value = 100;
            VGPair.Graphic = ""/images/HomeSeer/status/on.gif"";

            VSPair.Value = 100;
            VSPair.Status = ""On"";
            VSPair.ControlUse = ePairControlUse._On;
            break;
        case ""off"":
            VSPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
            VSPair.Render = Enums.CAPIControlType.Button;

            VGPair.PairType = VSVGPairs.VSVGPairType.SingleValue;
            VGPair.Set_Value = 0;
            VGPair.Graphic = ""/images/HomeSeer/status/off.gif"";

            VSPair.Value = 0;
            VSPair.Status = ""Off"";
            VSPair.ControlUse = ePairControlUse._Off;
            break;
        case ""dim"":
           
            VSPair.RangeStatusPrefix = ""Dim"";
            VSPair.RangeStart = 1;
            VSPair.RangeEnd = 100;
            VSPair.PairType = VSVGPairs.VSVGPairType.Range;
            VSPair.Render = Enums.CAPIControlType.ValuesRangeSlider;
            VSPair.Status = ""Dim88"";
            VSPair.ControlUse = ePairControlUse._Dim;
            break;
    }

    hs.DeviceVSP_AddPair(dvRef, VSPair);
    if (command.ToLower() != ""dim"")
        hs.DeviceVGP_AddPair(dvRef, VGPair);
}");
            ret.AppendLine(@"public void CreateBulbs()
{ ");

            foreach (var bulb in Bulbs)
                ret.AppendLine(@"    CreateDev(""" + bulb.Label + @""");");
            ret.AppendLine("}");

            ret.AppendLine("</body>");
            ret.AppendLine("</html>");
            return ret.ToString();
        }

        private static string BuildStatus()
        {
            var ret = new StringBuilder();

            ret.AppendLine("<html>");
            ret.AppendLine("<head>");
            ret.AppendLine("</head>");
            ret.AppendLine("<body>");

            ret.AppendLine("<table border=\"1\">");
            var bulbs = Bulbs.Where(a => !IsNullOrEmpty(a.Label)).OrderBy(a => a.Label).ToList();

            ret.AppendLine(
                $"<tr {FontSize(32)}\"><td>Label</td><td>IsOn</td><td>Hue</td><td>Saturation</td><td>Brightness</td><td>Kelvin</td><td>Color</td></tr>");

            foreach (var bulb in bulbs)
            {
                var h = (float) (bulb.Hue / 65535.0);
                var s = (float) (bulb.Saturation / 65535.0);
                var b = (float) (bulb.Brightness / 65535.0);


                var color = ColorUtil.HSBtoHEX(h, s, 1f);

                var sb = new StringBuilder();

                sb.AppendLine($"<tr {FontSize(20)}>");
                sb.AppendLine($"<td>{bulb.Label}</td>");
                sb.AppendLine($"<td>{bulb.IsOn}</td>");
                //   sb.AppendLine($"<td>{bulb.IsOn}</td>");
                sb.AppendLine($"<td>{h:F2}</td>");
                sb.AppendLine($"<td>{s:F2}</td>");
                sb.AppendLine($"<td>{b:F2}</td>");
                sb.AppendLine($"<td>{bulb.Kelvin}</td>");
                sb.AppendLine($"<td bgcolor=\"{color}\" width=\"50px\"></td>");
                sb.AppendLine("</tr>");
                ret.AppendLine(sb.ToString());
            }
            ret.AppendLine("</table>");

            ret.AppendLine("</body>");
            ret.AppendLine("</html>");
            return ret.ToString();
        }
    }
}