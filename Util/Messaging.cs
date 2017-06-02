using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Util
{
    public static class Messaging
    {
        public static async Task SendResponseAsync(HttpListenerResponse response, string message,
            bool logMessage = true)
        {
            if (logMessage)
                Log.Bulb($"Responded with {message}");
            if (!response.OutputStream.CanWrite)
                Log.Error($"Cannot write to response stream");
            else
                try
                {
                    var buffer = Encoding.UTF8.GetBytes(message);

                    response.ContentLength64 = buffer.LongLength;
                    response.OutputStream.WriteAsync(buffer, 0, buffer.Length).Wait(200);//.ConfigureAwait(false); //.Wait(200); //;.Wait(200);

                    //a.Wait(200);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error on response: {ex.Message}");
                }
            try
            {
                response.OutputStream.Flush();
                response.OutputStream.Close();
                response.Close();
            }
            catch
            {
            }
        }
    }
}