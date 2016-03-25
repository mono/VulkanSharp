using System;
using System.IO;
using Vulkan;

namespace VulkanSharp.VulkanInfo
{
    class InfoGenerator
    {
        uint major;
        uint minor;
        uint patch;

        public InfoGenerator(uint major, uint minor, uint patch)
        {
            this.major = major;
            this.minor = minor;
            this.patch = patch;
        }

        public void DumpInfo(Stream stream)
        {
            StreamWriter sw = new StreamWriter(stream);

            WriteHeaderAndVersion(sw);

            Instance vkInst = CreateInstance();

            DumpInstanceExtentions(sw);
            DumpInstanceLayers(sw);

            DumpDevices(sw);

            sw.Flush();
        }

        void WriteHeaderAndVersion(StreamWriter sw)
        {
            sw.WriteLine("===========");
            sw.WriteLine("VULKAN INFO");
            sw.WriteLine("===========\n");
            sw.WriteLine("Vulkan API Version: {0:d}.{1:d}.{2:d}\n", major, minor, patch);
        }

        void DumpInstanceExtentions(StreamWriter sw)
        {
            sw.WriteLine("Instance Extensions and layers:");
            sw.WriteLine("===============================");
            
        }

        void DumpInstanceLayers(StreamWriter sw)
        {
            uint layerCount = 0;
            sw.WriteLine("Instance Layers\tcount = {0:d}", layerCount);
        }

        void DumpDevices(StreamWriter sw)
        {

        }

        void DumpDevice(StreamWriter sw, object device)
        {

        }

        Instance CreateInstance()
        {
            string appShortName = "vulkaninfo";
            uint appVersion = 1;

            ApplicationInfo appInfo = new ApplicationInfo
            {
                ApplicationName = appShortName,
                ApplicationVersion = appVersion,
                EngineName = appShortName,
                EngineVersion = appVersion,
                ApiVersion = VkVersion.MakeVersion(major, minor, patch),
            };

            InstanceCreateInfo instInfo = new InstanceCreateInfo
            {
                ApplicationInfo = appInfo,                
            };

            LayerProperties[] propertys = null;
            Result resault = Commands.EnumerateInstanceLayerProperties(out propertys);
            if (resault != Result.Success)
                throw new ResultException(resault);

            return null;
        }
    }
}
