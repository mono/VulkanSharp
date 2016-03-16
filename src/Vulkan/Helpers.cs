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
}

