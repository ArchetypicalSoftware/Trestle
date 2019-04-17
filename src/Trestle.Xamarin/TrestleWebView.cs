using Archetypical.Software.Trestle.Abstractions;
using System;
using Xamarin.Forms;

namespace Archetypical.Software.Trestle.Xamarin
{
    public class TrestleWebView : WebView, ITrestle
    {
        private ITrestle _trestle;

        public TrestleWebView() {
            _trestle = CrossTrestle.Current;
        }

        public void AddUrlOverride(string url, Func<string> action)
        {
            _trestle.AddUrlOverride(url, action);
        }

        public string Send(string payload)
        {
            return _trestle.Send(payload);
        }

        public void WireWebView<T>(T webView)
        {
            _trestle.WireWebView<T>(webView);
        }
    }
}
