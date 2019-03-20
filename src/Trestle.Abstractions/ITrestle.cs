using System;

namespace Archetypical.Software.Trestle.Abstractions
{
    public interface ITrestle
    {
        string Send(string payload);

        T WireWebView<T>();

        void AddUrlOverride(string url, Func<string> action);
    }

    //public interface IPlatformElementConfiguration<out TPlatform, out TElement> : Xamarin.Forms.IConfigElement<out TElement> where TPlatform : IConfigPlatform where TElement : Element
}
