using System;
using System.Reflection;

using Android.App;
using Android.Runtime;

using Xamarin.Android.NUnitLite;

namespace mono.vulkansharp.tests.NativeMemory {

	[Instrumentation (Name="mono.vulkansharp.tests.TestInstrumentation")]
	public class TestInstrumentation : TestSuiteInstrumentation {

		public TestInstrumentation (IntPtr handle, JniHandleOwnership transfer)
			: base (handle, transfer)
		{
		}

		protected override void AddTests ()
		{
			AddTest (Assembly.GetExecutingAssembly ());
		}
	}
}

