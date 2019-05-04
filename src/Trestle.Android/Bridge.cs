using System;
using System.Text;
using Android.Webkit;
using Android.Widget;
using Archetypical.Software.Trestle.Abstractions;
using static Android.Views.ViewGroup;

namespace Archetypical.Software.Trestle
{
    public class Bridge :  ITrestle
    {
        private Android.Views.ViewGroup _view;
        private WebView _webView;
        private BridgeWebViewClient _webViewClient;

        public string Send(string payload)
        { 

            var myValue = new JavaScriptValueCallback();
            _webView.EvaluateJavascript(payload, myValue); 

            return myValue.Value;
        }

        public void AddUrlOverride(string url, Func<string> action)
        {  
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
            else 
            {
                _view = webView as Android.Views.ViewGroup;

                _webView = new WebView(_view.Context);
                _webView.ClearCache(true);
                _webView.Settings.JavaScriptEnabled = true;
                _webView.Settings.DomStorageEnabled = true;
                _webView.LayoutParameters = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent);

                _webViewClient = new BridgeWebViewClient();
                _webView.SetBackgroundColor(Android.Graphics.Color.Pink);
                _view.AddView(_webView);
                _webView.SetWebViewClient(_webViewClient);

                var button = new Button(_view.Context);
                button.Text = "My Dynamic Button";
                button.SetBackgroundColor(Android.Graphics.Color.Brown);
                button.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                _view.AddView(button);
            }
        }

        public void LoadHtmlString(string html)
        {
            var header = "<?xml version=\"1.0\" encoding=\"UTF-8\" ?>";
            var base64 = Android.Util.Base64.EncodeToString(Encoding.ASCII.GetBytes(header + html), Android.Util.Base64Flags.Default);
            _webView.LoadDataWithBaseURL(base64, html, "text/html;", "charset=utf-8", "base64");
        }

        public void SetUrl(string url)
        {
            _webView.LoadUrl(url);
        }
    }
}
