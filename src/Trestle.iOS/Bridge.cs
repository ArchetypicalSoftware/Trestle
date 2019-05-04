using System;
using System.Collections.Generic;
using Archetypical.Software.Trestle.Abstractions;
using Foundation;
using UIKit;
using WebKit;

namespace Archetypical.Software.Trestle
{
    public class Bridge : ITrestle
    {
        private UIView _layout;
        private WKWebView _webView;
        private TrestleNavigationDelegate _webViewClient;
        private WKWebViewConfiguration _configuration;
        private string _html;
        private string _url;

        public void AddUrlOverride(string url, Func<string> action)
        {
            if (_webView == null)
            {
                throw new System.Exception("WebView and WebViewClient were not initialized");
            }

            _webViewClient.AddOverrideUrl(url, action);

            var schemeHandler = new TrestleSchemeHandler();
            schemeHandler.AddOverrideUrl(url, action);

            _configuration.SetUrlSchemeHandler(schemeHandler, "trestle");

            _webView.EvaluateJavaScript("document.documentElement.outerHTML.toString()", (NSObject result, NSError err) => {
                if (err != null)
                {
                    System.Console.WriteLine(err);
                }
                if (result != null)
                {
                    _html = result.ToString();
                }
            });

            _webView.RemoveFromSuperview();
            _webView = new WKWebView(_layout.Bounds, _configuration);
            _webView.TranslatesAutoresizingMaskIntoConstraints = false;

            _webView.NavigationDelegate = _webViewClient;

            _layout.AddSubview(_webView);
            NSLayoutConstraint.Create(_webView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _layout, NSLayoutAttribute.CenterX, 1f, 0f).Active = true;
            NSLayoutConstraint.Create(_webView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, _layout, NSLayoutAttribute.CenterY, 1f, 0f).Active = true;
            _webView.SetNeedsDisplay();
            _webView.LoadHtmlString(_html, null);
        }

        public void LoadHtmlString(string html)
        {
            _html = html;
            _webView.LoadHtmlString(_html, null);
        }

        public void SetUrl(string url)
        {
            _url = url;
            // TODO ios impl
        }

        public string Send(string payload)
        {
            string response = "";
            _webView.EvaluateJavaScript(payload, (NSObject result, NSError error) => { 
                if (error != null)
                {
                    System.Console.WriteLine(error); 
                }
                if (result != null)
                {
                    response = result.ToString(); 
                }
            });
            return response;
        }

        public void WireWebView<T>(T webView)
        {
            // Having issues with getting the correct type to match even though passing in WKWebView from client
            //_webView = webView as WKWebView;
            _layout = webView as UIView;

            var preferences = new WKPreferences();
            preferences.JavaScriptEnabled = true;

            _configuration = new WKWebViewConfiguration();
            _configuration.Preferences = preferences;

            _webViewClient = new TrestleNavigationDelegate();

            _webView = new WKWebView(_layout.Frame, _configuration);
            _webView.TranslatesAutoresizingMaskIntoConstraints = false;

            _webView.NavigationDelegate = _webViewClient;

            _layout.AddSubview(_webView);
            NSLayoutConstraint.Create(_webView, NSLayoutAttribute.CenterX, NSLayoutRelation.Equal, _layout, NSLayoutAttribute.CenterX, 1f, 0f).Active = true;
            NSLayoutConstraint.Create(_webView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, _layout, NSLayoutAttribute.CenterY, 1f, 0f).Active = true;
            _webView.SetNeedsDisplay();
        }
    }
}
