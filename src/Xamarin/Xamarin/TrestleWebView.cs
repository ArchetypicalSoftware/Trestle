using Archetypical.Software.Trestle;
using Archetypical.Software.Trestle.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xamarin
{
    public class TrestleWebView : WebView, IXamTrestle
    {
        private List<string> urlOverrides;
        private ITrestle _trestle;

        public TrestleWebView() {
            urlOverrides = new List<string>();
            _trestle = CrossTrestle.Current;
        }

        public void AddUrlOverride(string url, Func<string> action)
        {
            _trestle.AddUrlOverride(url, action);
        }

        public async Task AddServiceWorker(string javaScript)
        {
            await this.EvaluateJavaScriptAsync(javaScript);
        }

        public async Task AddServiceWorker()
        {
            var otherJS = "get this from somewhere else";
            await this.EvaluateJavaScriptAsync(otherJS);
        }

        void IXamTrestle.AddServiceWorker(string javaScript)
        {
            throw new NotImplementedException();
        }
    }

    public static class TrestleHelper
    {
        public static void AddServiceWorker(this WebView webView, string javaScript)
        {

        }
    }
}
