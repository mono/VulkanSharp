using Android.App;
using Android.Widget;
using Android.OS;
using ClearView.Common;
using VulkanSample.Android;

namespace ClearView.Android
{
	[Activity (Label = "ClearView", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			LinearLayout layout = FindViewById<LinearLayout> (Resource.Id.Layout);
			layout.AddView (new VulkanSampleView (ApplicationContext, new ClearViewSample ()));
		}
	}
}


