using System;
using NUnit.Framework;
using Vulkan;

namespace NativeMemory
{
	[TestFixture]
	public class TestsSample
	{

		[SetUp]
		public void Setup ()
		{
			NativeMemoryDebug.ReportCallback = Console.WriteLine;
			// uncomment for more vebose output
			// NativeMemoryDebug.StackTrace = () => new System.Diagnostics.StackTrace ().ToString ();
			NativeMemoryDebug.Enabled = true;
		}


		[TearDown]
		public void Tear () { }

		[Test]
		public void NativeMemory ()
		{
			Console.WriteLine ("create instance and query devices");

			var instance = new Instance ();
			if (instance == null)
				Assert.Fail ("unable to create Vulkan instance");
			var devices = instance.EnumeratePhysicalDevices ();
			if (devices.Length < 1)
				Assert.Fail ("this test needs device with at least one Vulkan physical device");
			
			Console.WriteLine ("enumerated {0} devices, total references {1} allocated size {2}", devices.Length, NativeMemoryDebug.GlobalRefCount, NativeMemoryDebug.AllocatedSize);
			foreach (var device in devices)
				Console.WriteLine ("device: {0}", device.GetProperties ().DeviceName);

			Console.WriteLine ("before forced GC, total references {0} allocated size {1}", NativeMemoryDebug.GlobalRefCount, NativeMemoryDebug.AllocatedSize);

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Console.WriteLine ("after forced GC, total references {0} allocated size {1}", NativeMemoryDebug.GlobalRefCount, NativeMemoryDebug.AllocatedSize);

			Assert.False (NativeMemoryDebug.GlobalRefCount > 0);

			instance.Dispose ();
			instance = null;

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Console.WriteLine ("after Instance disposed, total references {0} allocated size {1}", NativeMemoryDebug.GlobalRefCount, NativeMemoryDebug.AllocatedSize);
		}
	}
}

