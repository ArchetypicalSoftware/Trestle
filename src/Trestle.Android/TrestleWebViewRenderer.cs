using Android.Content;
using Archetypical.Software.Trestle;
using Archetypical.Software.Trestle.Xamarin;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TrestleWebView), typeof(TrestleWebViewRenderer))]
namespace Archetypical.Software.Trestle
{
    public class TrestleWebViewRenderer : WebViewRenderer
    {
        public TrestleWebViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);

            if (Element != null && Control != null)
            {
                var trestle = (TrestleWebView)Element;
                trestle.WireWebView(Control as Android.Webkit.WebView);
                trestle.AddUrlOverride("http://trestle.contacts/getall", () => { return JsonConvert.SerializeObject(TrestleHelper.GetContacts()); });
            }
        }
    }
}
