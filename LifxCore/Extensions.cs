using System;
using System.Threading;
using System.Threading.Tasks;
using LIFXDevices;
using Util;

namespace LifxCore
{
    public static class Extensions
    {
        public static void SetState(this LightBulb bulb, LightStateResponse response)
        {
            bulb.Hue = response.Hue;
            bulb.Saturation = response.Saturation;
            bulb.Brightness = response.Brightness;
            bulb.Kelvin = response.Kelvin;
            bulb.Label = response.Label;
            bulb.IsOn = response.IsOn;
        }

        public static async Task<bool> TimeoutAfter(this Task task, TimeSpan timeout)
        {
            var timeoutCancellationTokenSource = new CancellationTokenSource();

            var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
            if (completedTask == task)
            {
                timeoutCancellationTokenSource.Cancel();
                await task; // Very important in order to propagate exceptions
            }
            else
            {
                Log.Error("TimeoutException The operation has timed out.");
                //   throw new TimeoutException("The operation has timed out.");
                return false;
            }

            return true;
        }
    }
}