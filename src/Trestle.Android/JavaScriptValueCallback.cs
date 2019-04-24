using System;
using Android.Webkit;

namespace Archetypical.Software.Trestle
{
    public class JavaScriptValueCallback : Java.Lang.Object, IValueCallback
    {
        public string Value;

        public void OnReceiveValue(Java.Lang.Object value)
        {
            Value = value.ToString();
        }
    }
}