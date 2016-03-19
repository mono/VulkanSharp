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

    public static class MarshalHeler
    {
        public static string[] PtrUTF8ArrayToStringArray(IntPtr ptr, uint count)
        {
            int c = (int)count;
            string[] names = new string[count];
            // scan ptr till we found all elements of LayerNames
            for (int i = 0; i < c; i++)
            {
                // Scan null termiantor for lenght
                int lenght = 0;
                while (Marshal.ReadByte(ptr, lenght) != 0)
                {
                    lenght++;
                }

                byte[] data = new byte[lenght];
                Marshal.Copy(ptr, data, 0, lenght);
                string name = System.Text.Encoding.UTF8.GetString(data, 0, lenght);
                names[i] = name;
                ptr += lenght + 1; // skipp the null terminator
            }
            return names;
        }

        public static void StringArrayToPtrUTF8Array(string[] strings, ref IntPtr ptr, ref uint count)
        {
            string names = string.Join("\0", strings) + "\0";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(names);
            IntPtr newPtr = Marshal.AllocHGlobal(data.Length);
            try
            {
                Marshal.Copy(data, 0, newPtr, data.Length);
            }
            catch (Exception ex)
            {
                Marshal.FreeHGlobal(newPtr);
                throw new Exception("Failed to copy string data to target location", ex);
            }
            finally
            {
                // free old pointer 
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }

                // set new correct values
                count = System.Convert.ToUInt32(strings.Length);
                ptr = newPtr;
            }
        }
    }
}

