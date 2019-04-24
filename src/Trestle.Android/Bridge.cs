using System;
using Android.Webkit;
using Archetypical.Software.Trestle.Abstractions;

namespace Archetypical.Software.Trestle
{
    public class Bridge :  ITrestle
    {
        private WebView _webView;
        private BridgeWebViewClient _webViewClient;
        
        public Bridge()
        {
        }

        public string Send(string payload)
        {
            if (_webView == null || _webViewClient == null)
            {
                throw new System.Exception("WebView and WebViewClient were not initialized");
            }

            var myValue = new JavaScriptValueCallback();
            _webView.EvaluateJavascript(payload, myValue); 

            return myValue.Value;
        }

        public void AddUrlOverride(string url, Func<string> action)
        {
            if (_webView == null || _webViewClient == null)
            {
                throw new System.Exception("WebView and WebViewClient were not initialized");
            }

            _webViewClient.AddOverrideUrl(url, action);
        }

        public void WireWebView<T>(T webView)
        {
            WebView.SetWebContentsDebuggingEnabled(true);

            if (webView.GetType() == typeof(WebView))
            {
                _webView = webView as WebView;
                _webView.ClearCache(true);
                _webView.Settings.JavaScriptEnabled = true;

                _webViewClient = new BridgeWebViewClient();
                _webView.SetWebViewClient(_webViewClient);
            }
        }
    }

  
}
