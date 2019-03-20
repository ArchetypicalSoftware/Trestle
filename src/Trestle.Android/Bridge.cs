using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Webkit;
using Archetypical.Software.Trestle.Abstractions;
using Java.Lang;

namespace Archetypical.Software.Trestle
{
    public class Bridge :  ITrestle
    {
        private Context _context;
        private WebView _webView;
        private BridgeWebViewClient _webViewClient;

        #region constructors
        public Bridge()
        {
            _webViewClient = new BridgeWebViewClient();
            _webView.SetWebViewClient(_webViewClient);
        }
        #endregion

        public string Send(string payload)
        {
            return "success";
        }

        public void AddUrlOverride(string url, Func<string> action)
        {
            if (typeof(WebViewClient) != typeof(BridgeWebViewClient))
            {
                _webViewClient = new BridgeWebViewClient();
                _webView.SetWebViewClient(_webViewClient);
            }

            _webViewClient.AddOverrideUrl(url, action);
        }


        public T WireWebView<T>()
        {
            throw new System.NotImplementedException();
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
            if (_urls.Contains(request.Url.Path))
            {
                var action = _urlActions[request.Url.Path];
                var response = action.Invoke();
                using (var stream = Helpers.GenerateStreamFromString(response))
                {
                    webResourceResponse = new WebResourceResponse(request.RequestHeaders["mimeType"], request.RequestHeaders["encoding"], stream);
                }
            }

            if (webResourceResponse != null)
                return webResourceResponse;

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
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
