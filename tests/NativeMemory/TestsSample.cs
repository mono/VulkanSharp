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
			
			Console.WriteLine ("enumerated {0} devices, allocated size {1}", devices.Length, NativeMemoryDebug.AllocatedSize);
			foreach (var device in devices)
				Console.WriteLine ("device: {0}", device.GetProperties ().DeviceName);

			Console.WriteLine ("before forced GC, allocated size {0}", NativeMemoryDebug.AllocatedSize);

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Console.WriteLine ("after forced GC, allocated size {0}", NativeMemoryDebug.AllocatedSize);

			Assert.False (NativeMemoryDebug.AllocatedSize > 0);

			instance.Dispose ();
			instance = null;

			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Console.WriteLine ("after Instance disposed, allocated size {0}", NativeMemoryDebug.AllocatedSize);
		}

		[Test]
		public void CommandsNativeMemory ()
		{
			Console.WriteLine ("query layers and extensions info");
			var layerProps = Commands.EnumerateInstanceLayerProperties ();
			Console.WriteLine ("after layers info created, allocated size {0}", NativeMemoryDebug.AllocatedSize);
			var extProps = Commands.EnumerateInstanceExtensionProperties ();
			Console.WriteLine ("after extensions info created, allocated size {0}", NativeMemoryDebug.AllocatedSize);
			layerProps = null;
			extProps = null;
			Console.WriteLine ("after info dropped, allocated size {0}", NativeMemoryDebug.AllocatedSize);
			GC.Collect ();
			GC.WaitForPendingFinalizers ();
			Console.WriteLine ("after GC and finalizers, allocated size {0}", NativeMemoryDebug.AllocatedSize);
			Assert.False (NativeMemoryDebug.AllocatedSize > 0);
		}
	}
}

