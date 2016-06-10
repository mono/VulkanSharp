using System;

namespace Vulkan
{
	public struct Bool32
	{
		UInt32 value;

		public Bool32 (bool bValue)
		{
			value = bValue ? 1u : 0;
		}

		public static implicit operator Bool32 (bool bValue)
		{
			return new Bool32 (bValue);
		}

		public static implicit operator bool (Bool32 bValue)
		{
			return bValue.value == 0 ? false : true;
		}
	}

	public struct DeviceSize
	{
		UInt64 value;

		public static implicit operator DeviceSize (UInt64 iValue)
		{
			return new DeviceSize { value = iValue };
		}

		public static implicit operator DeviceSize (uint iValue)
		{
			return new DeviceSize { value = iValue };
		}

		public static implicit operator DeviceSize (int iValue)
		{
			return new DeviceSize { value = (ulong)iValue };
		}

		public static implicit operator UInt64 (DeviceSize size)
		{
			return size.value;
		}
	}

	public class ResultException : Exception
	{
		internal Result result;

		public Result Result {
			get { return result; }
		}

		internal ResultException (Result res)
		{
			result = res;
		}
	}

	public class Version
	{
		public static uint Make (uint major, uint minor, uint patch)
		{
			return (major << 22) | (minor << 12) | patch;
		}

		public static string ToString (uint version)
		{
			return string.Format ("{0}.{1}.{2}", version >> 22, (version >> 12) & 0x3ff, version & 0xfff);
		}
	}
}

