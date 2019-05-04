using Archetypical.Software.Trestle;
using Archetypical.Software.Trestle.Xamarin;
using Newtonsoft.Json;
using WebKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TrestleWrapper), typeof(TrestleWrapperRenderer))]
namespace Archetypical.Software.Trestle
{
    public class TrestleWebViewRenderer : WkWebViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e) 
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                var trestle = Element as TrestleWrapper;
                trestle.WireWebView(this as WKWebView);
                trestle.AddUrlOverride("http://trestle.contacts/getall", () => { return JsonConvert.SerializeObject(TrestleHelper.GetContacts()); });
            }
        }
    }
}
