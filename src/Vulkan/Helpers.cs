using System;
using System.Runtime.InteropServices;

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

    public static class VkVersion
    {
        public static uint MakeVersion(uint major, uint minor, uint patch)
        {
            return (((major) << 22) | ((minor) << 12) | (patch));
        }
    }

    public static unsafe class MarshalHeler
    {
        public static string[] PtrUTF8ArrayToStringArray(IntPtr ptr, uint count)
        {
            string[] strings = new string[count];
            for(int i = 0; i < count; i++)
            {
                IntPtr strPtr = Marshal.ReadIntPtr(ptr, IntPtr.Size * i);
                strings[i] = PtrUTF8ToString(strPtr);
            }
            return strings;
        }

        public static void StringArrayToPtrUTF8Array(string[] strings, ref IntPtr ptr, ref uint count)
        {
            int l = strings.Length;
            IntPtr array = Marshal.AllocHGlobal(IntPtr.Size * l);
            for(int i = 0; i < l; i++)
            {
                Marshal.WriteIntPtr(array, i * IntPtr.Size, StringToPtrUTF8(strings[i]));
            }
            // TODO : free memory of ould ptr befor assining
            // TODO : free memory of IntPtr inside array
            count = (uint)l;
            ptr = array;
        }

        public static string PtrUTF8ToString(IntPtr ptr)
        {
            int lenght = 0;
            while (Marshal.ReadByte(ptr, lenght) != 0)
            {
                lenght++;
            }

            byte[] data = new byte[lenght];
            Marshal.Copy(ptr, data, 0, lenght);
            return System.Text.Encoding.UTF8.GetString(data, 0, lenght);            
        }

        public static IntPtr StringToPtrUTF8(string str)
        {
            str += "\0";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(str);
            IntPtr ptr = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, ptr, data.Length);
            }
            catch (Exception ex)
            {
                Marshal.FreeHGlobal(ptr);
                throw new Exception("Failed to copy string data to target location", ex);
            }
            return ptr;
        }
    }
}

