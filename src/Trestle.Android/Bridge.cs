using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Android.Runtime;
using Android.Webkit;
using Archetypical.Software.Trestle.Abstractions;

namespace Archetypical.Software.Trestle
{
    public class Bridge :  ITrestle
    {
        private WebView _webView;
        private BridgeWebViewClient _webViewClient;

        #region constructors
        public Bridge()
        {
        }
        #endregion

        public string Send(string payload)
        {
            return "success";
        }

        public void AddUrlOverride(string url, Func<string> action)
        {
            if (_webView == null || _webViewClient == null)
            {
                throw new Exception("WebView and WebViewClient were not initialized");
            }

            _webViewClient.AddOverrideUrl(url, action);
        }

        public void WireWebView<T>(T webView)
        {
            if (webView.GetType() == typeof(WebView))
            {
                _webView = webView as WebView;
                _webViewClient = new BridgeWebViewClient();
                _webView.SetWebViewClient(_webViewClient);
            }
        }
    }

    public class BridgeWebViewClient : WebViewClient
    {
        private List<string> _urls;
        private Dictionary<string, Func<string>> _urlActions;

        public BridgeWebViewClient()
        {
            _urls = new List<string>();
            _urlActions = new Dictionary<string, Func<string>>();
        }

        protected BridgeWebViewClient(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            _urls = new List<string>();
            _urlActions = new Dictionary<string, Func<string>>();
        }

        public override WebResourceResponse ShouldInterceptRequest(WebView view, IWebResourceRequest request)
        {
            WebResourceResponse webResourceResponse = null;
            var urlToCheck = $"{request.Url.Scheme}:{request.Url.SchemeSpecificPart}";
            if (_urls.Contains(urlToCheck))
            {
                var action = _urlActions[urlToCheck];
                var response = action.Invoke();
                var stream = new MemoryStream();
                
                var sw = new StreamWriter(stream);
                
                sw.WriteLine(response);
                stream.Position = 0;

                webResourceResponse = new WebResourceResponse(
                    request.RequestHeaders.ContainsKey("mimeType") ? request.RequestHeaders["mimeType"] : "application/json", 
                    request.RequestHeaders.ContainsKey("encoding") ? request.RequestHeaders["encoding"] : "UTF-8",
                    200,"OK",request.RequestHeaders,
                    stream);

                //  Uncomment the code below to see what is in web resource response data
                //var blah = new MemoryStream();
                //webResourceResponse.Data.CopyTo(blah);
                //blah.Position = 0;

                //var sr = new StreamReader(blah);
                //var test = sr.ReadToEnd();

                return webResourceResponse;
            }

            return base.ShouldInterceptRequest(view, request);
        }

        public void AddOverrideUrl(string url, Func<string> action)
        {
            _urls.Add(url);
            _urlActions.Add(url, action);
        }
    }

    public static class Helpers
    {
        public static Stream GenerateStreamFromString(string s)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(s);
            var stream = new MemoryStream(byteArray);
            //var writer = new StreamWriter(stream);
            //writer.Write(s);
            //writer.Flush();
            //stream.Position = 0;
            return stream;
        }
    }
}
