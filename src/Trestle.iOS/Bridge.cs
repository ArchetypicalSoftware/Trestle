using System;
using Archetypical.Software.Trestle.Abstractions;

namespace Archetypical.Software.Trestle
{
    public class Bridge : ITrestle
    {
        public void AddUrlOverride(string url, Func<string> action)
        {
            throw new NotImplementedException();
        }

        public string Send(string payload)
        {
            throw new NotImplementedException("It kinda worked");
        }

        public void WireWebView<T>(T webView)
        {
            throw new NotImplementedException();
        }
    }
}
