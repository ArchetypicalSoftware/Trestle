using Android.Content;
using Archetypical.Software.Trestle;
using Archetypical.Software.Trestle.Xamarin;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TrestleWrapper), typeof(TrestleWrapperRenderer))]
namespace Archetypical.Software.Trestle
{
    public class TrestleWrapperRenderer : ViewRenderer<TrestleWrapper, Android.Views.ViewGroup>
    {
        public TrestleWrapperRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TrestleWrapper> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                var trestle = Element;
                trestle.WireWebView<Android.Views.ViewGroup>(this);
                trestle.AddUrlOverride("http://trestle.contacts/getall", () => { return JsonConvert.SerializeObject(TrestleHelper.GetContacts()); });
            }
        }
    }
}
