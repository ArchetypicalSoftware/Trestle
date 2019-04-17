using System;

namespace Archetypical.Software.Trestle.Xamarin
{
    public interface IXamTrestle
    {
        void AddUrlOverride(string url, Func<string> action);
    }
}
