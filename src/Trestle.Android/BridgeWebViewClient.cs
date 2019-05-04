using System;
using System.Collections.Generic;
using System.Net.Http;
using Android.Runtime;
using Android.Webkit;

namespace Archetypical.Software.Trestle
{
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
            var urlToCheck = $"{request.Url.Scheme}:{request.Url.SchemeSpecificPart}";
            if (!_urls.Contains(urlToCheck))
                return base.ShouldInterceptRequest(view, request);
            try
            {
                var action = _urlActions[urlToCheck];
                var actionResult = action.Invoke();

                using (var c = new HttpClient())
                {
                    var response = new HttpResponseMessage();
                    response.Content = new StringContent(actionResult);
                    var content = response.Content.ReadAsStreamAsync().Result;
                    var contentType = "*/*";
                    var headers = new Dictionary<string, string>();
                    headers.Add("Access-Control-Allow-Origin", "*");

                    return new WebResourceResponse(contentType, "UTF-8", 200, "OK", headers, content);
                }
            }
            catch (AggregateException)
            {
                return base.ShouldInterceptRequest(view, request);
            }
        }

        public void AddOverrideUrl(string url, Func<string> action)
        {
            _urls.Add(url);
            _urlActions.Add(url, action);
        }
    }
}