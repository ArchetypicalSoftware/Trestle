using Archetypical.Software.Trestle;
using Archetypical.Software.Trestle.Xamarin;
using Newtonsoft.Json;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TrestleWrapper), typeof(TrestleWrapperRenderer))]
namespace Archetypical.Software.Trestle
{
    public class TrestleWrapperRenderer : ViewRenderer<TrestleWrapper, UIView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<TrestleWrapper> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                var trestle = Element as TrestleWrapper;
                trestle.WireWebView(this as UIView);
                trestle.AddUrlOverride("http://trestle.contacts/getall", () => { return JsonConvert.SerializeObject(TrestleHelper.GetContacts()); });
            }
        }
    }
}
