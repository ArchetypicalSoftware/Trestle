using System;

namespace Archetypical.Software.Trestle.Abstractions
{
    public interface ITrestle
    {
        string Send(string payload);

        void WireWebView<T>(T webView);

        void AddUrlOverride(string url, Func<string> action);

        void LoadHtmlString(string html);

        void SetUrl(string url);
    }
}
