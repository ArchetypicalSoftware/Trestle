using System;
using System.Collections.Generic;
using Foundation;
using WebKit;

namespace Archetypical.Software.Trestle
{
    public class TrestleNavigationDelegate : WKNavigationDelegate//, IWKNavigationDelegate
    {
        private List<string> _urls;
        private Dictionary<string, Func<string>> _urlActions;

        public TrestleNavigationDelegate()
        {
            _urls = new List<string>();
            _urlActions = new Dictionary<string, Func<string>>();
        }

        [Export("webView:decidePolicyForNavigationAction:decisionHandler:")]
        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            var urlToCheck = $"{navigationAction.Request.Url.Scheme}:{navigationAction.Request.Url.AbsoluteString}";
            if (!_urls.Contains(urlToCheck))
            {
                decisionHandler(WKNavigationActionPolicy.Allow);
                return; //base.ShouldInterceptRequest(view, request);
            }

            decisionHandler(WKNavigationActionPolicy.Cancel);

            // TODO: how to handle response?
        }

        [Export("webView:decidePolicyForNavigationResponse:decisionHandler:")]
        public override void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            var urlToCheck = $"{navigationResponse.Response.Url.Scheme}:{navigationResponse.Response.Url.AbsoluteString}";
            if (!_urls.Contains(urlToCheck))
            {
                decisionHandler(WKNavigationResponsePolicy.Allow);
                return; //base.ShouldInterceptRequest(view, request);
            }

            decisionHandler(WKNavigationResponsePolicy.Cancel);

            // TODO: how to handle response?
        }

        public void AddOverrideUrl(string url, Func<string> action)
        {
            _urls.Add(url);
            _urlActions.Add(url, action);
        }
    }
}
