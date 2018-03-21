using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Inspector.Android;

namespace Inspector
{
	[Activity (Label = "Inspector", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		TextView textView;

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			textView = FindViewById<TextView> (Resource.Id.TextView);
			var layout = FindViewById<LinearLayout> (Resource.Id.Layout);
			try {
				var inspectorView = new InspectorView (BaseContext, textView);
				layout.AddView (inspectorView, -1, 1);
			} catch (Exception e) {
				textView.Append (string.Format ("Unable to instantiate Vulkan\n\nException:\n{0}", e));
			}
		}
	}
}


