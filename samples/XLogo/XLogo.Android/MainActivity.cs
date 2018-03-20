using Android.App;
using Android.Widget;
using Android.OS;
using Samples.Common;
using VulkanSample.Android;
using XLogo.Common;

namespace XLogo
{
	[Activity (Label = "XLogo", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.SensorPortrait)]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			LinearLayout layout = FindViewById<LinearLayout> (Resource.Id.Layout);
			layout.AddView (new VulkanSampleView (ApplicationContext, new XLogoSample ()));
		}
	}
}