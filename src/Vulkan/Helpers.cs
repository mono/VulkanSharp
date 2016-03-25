﻿using System;
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

		public ResultException (Result res)
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
    
    public static unsafe class MarshalHelper
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

        // TODO : how to dealocate this
        public static T[] CreateArray<T>(uint count, int tSize, Func<IntPtr,T> creator)
        {
            int iCount = (int)count;
            Type t = typeof(T);

            // Calc alloc size
           // int tSize = Marshal.SizeOf(t);
            int size = tSize * iCount;

            // alloc
            IntPtr ptr = Marshal.AllocHGlobal(size);

            // Null out allocation
            unsafe
            {
                byte* bptr = (byte*)ptr.ToPointer();
                for (int i = 0; i < size; i++)
                    bptr[i] = 0;
            }

            // create net Array with elemtns
            T[] netArray = new T[iCount];

            // map pointer to native
            for(int i = 0; i < iCount; i++)
            {
                netArray[i] = creator(ptr);
                ptr += tSize;
            }

            return netArray;
        }

        public static string FixedTextToString(byte* source, int lenght)
        {
            return Marshal.PtrToStringAnsi((IntPtr)source, lenght);
            //return System.Text.Encoding.UTF8.GetString(source, lenght);
        }

        public static void StringToFixedText(string str, byte* pTarget, int lenght)
        {
            // TODO : ANSII is not available dount know why
            //byte[] source = System.Text.Encoding.ANSII.GetBytes(str);
            byte[] source = System.Text.Encoding.UTF8.GetBytes(str);
            fixed (byte* pSource = source)
            {
                byte* ps = pSource;
                byte* pt = pTarget;

                for (int i = 0; i < lenght; i++)
                {
                    if (i < source.Length)
                    {
                        *pt = *ps;
                        ps++;
                    }
                    else
                    {
                        *pt = 0;
                    }
                    pt++;
                }
            }
        }
    }
}

