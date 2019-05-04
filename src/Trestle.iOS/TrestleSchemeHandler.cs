using System;
using System.Collections.Generic;
using Foundation;
using WebKit;

namespace Archetypical.Software.Trestle
{
    public class TrestleSchemeHandler : NSObject, IWKUrlSchemeHandler
    {
        private List<string> _urls;
        private Dictionary<string, Func<string>> _urlActions;

        public TrestleSchemeHandler()
        {
            _urls = new List<string>();
            _urlActions = new Dictionary<string, Func<string>>();
        }

        public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
        {
            var urlToCheck = $"{urlSchemeTask.Request.Url.Scheme}:{urlSchemeTask.Request.Url.BaseUrl}";
            if (!_urls.Contains(urlToCheck))
                return;

            var action = _urlActions[urlToCheck];
            var actionResult = action.Invoke();
            urlSchemeTask.DidReceiveData(NSData.FromString(actionResult));
            urlSchemeTask.DidFinish();
        }

        public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
        {
            return;
        }

        public void AddOverrideUrl(string url, Func<string> action)
        {
            _urls.Add(url);
            _urlActions.Add(url, action);
        }
    }
}
