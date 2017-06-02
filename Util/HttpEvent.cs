using System;
using System.Collections.Specialized;
using System.Net;

namespace Util
{
    public class HttpEvent : EventArgs
    {
        public string ID { get; set; }
        public NameValueCollection QueryString { get; set; }
        public HttpListenerResponse HttpListenerResponse { get; set; }

       
    }
}