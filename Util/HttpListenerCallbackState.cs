using System;
using System.Net;
using System.Threading;

namespace Util
{
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