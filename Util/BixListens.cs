using System;
using System.Net;
using System.Threading;
using Util;

namespace BIXLIFX
{
    public class BixListens
    {
      
        public BixListens(int port = 9105)
        {
            string liststring = $"http://+:{port}/";

            var handler = new HttpRequestHandler();
            handler.OnHttpEventReceived += Handler_OnHttpEventReceived;
            handler.ListenAsynchronously(liststring);
            
            Log.Info($"Server Listening.. {liststring}");
        }

        private void Handler_OnHttpEventReceived(object sender, HttpEvent e)
        {
            OnHttpEventReceived?.Invoke(this, e);
        }

        private void Handler_ProcessRequest(HttpEvent e)
        {
            OnHttpEventReceived?.Invoke(this, e);
        }

        public event HttpEventHandler OnHttpEventReceived;

        protected virtual void OnOnHttpEventCReceived(HttpEvent e)
        {
            OnHttpEventReceived?.Invoke(this, e);
        }
    }

    public class HttpListenerCallbackState
    {
        public HttpListenerCallbackState(HttpListener listener)
        {
            if (listener == null) throw new ArgumentNullException("listener");
            Listener = listener;
            ListenForNextRequest = new AutoResetEvent(false);
        }

        public HttpListener Listener { get; }
        public AutoResetEvent ListenForNextRequest { get; }
    }
}