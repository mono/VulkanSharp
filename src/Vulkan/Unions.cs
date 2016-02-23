using System;

namespace Vulkan
{
	unsafe public class ClearColorValue
	{
		internal Interop.ClearColorValue* m;

		public ClearColorValue ()
		{
			m = (Interop.ClearColorValue*) Interop.Structure.Allocate (typeof (Interop.ClearColorValue));
		}

		public float Float32 {
			get { return m->Float32; }
			set { m->Float32 = value; }
		}

		public Int32 Int32 {
			get { return m->Int32; }
			set { m->Int32 = value; }
		}

		public UInt32 Uint32 {
			get { return m->Uint32; }
			set { m->Uint32 = value; }
		}
	}

	unsafe public class ClearValue
	{
		internal Interop.ClearValue* m;

		public ClearValue ()
		{
			m = (Interop.ClearValue*) Interop.Structure.Allocate (typeof (Interop.ClearValue));
		}

		ClearColorValue lColor;
		public ClearColorValue Color {
			get { return lColor; }
			set { lColor = value; m->Color = (IntPtr) value.m; }
		}

		ClearDepthStencilValue lDepthStencil;
		public ClearDepthStencilValue DepthStencil {
			get { return lDepthStencil; }
			set { lDepthStencil = value; m->DepthStencil = (IntPtr) value.m; }
		}
	}
}
