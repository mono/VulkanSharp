using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Vulkan;

// TODO : vulkan
//          - Flag enum same type as in api QueueFlags is int but in api is uint
//          - Flag enum with null Entry
//          - Use enum in api
//          - PhysicalDeviceMemoryProperties.MemoryTpyes is not a array
//          - PhysicalDeviceMemoryProperties.MemoryHeapCount is not a array
//          - DeviceCreateInfo.QueueCreateInfoCount shuld not be exposed
//          - PhysicalDeviceProperties.PipelineCacheUUID is a array with 16 elements


namespace vulkaninfo
{
    public class InfoGenerator
    {
        public string ApplicationName { get; set; }
        public uint ApplicationVersion { get; set; }
        public string EngineName { get; set; }
        public uint EnginveVersion { get; set; }
        public string[] KnownExtensions { get; set; }
        public string[] KnownDeviceExtensions { get; set; }

        public InfoGenerator()
        {
            ApplicationName = "vulkaninfo";
            ApplicationVersion = 1;
            EngineName = ApplicationName;
            EnginveVersion = ApplicationVersion;
            KnownExtensions = new string[]
            {
                "VK_KHR_surface",
#if VK_USE_PLATFORM_ANDROID_KHR
        "VK_KHR_android_surface",
#endif
#if VK_USE_PLATFORM_MIR_KHR
        "VK_KHR_mir_surface",
#endif
#if VK_USE_PLATFORM_WAYLAND_KHR
        "VK_KHR_wayland_surface",
#endif
#if VK_USE_PLATFORM_WIN32_KHR
        "VK_KHR_win32_surface",
#endif
#if VK_USE_PLATFORM_XCB_KHR
        "VK_KHR_xcb_surface",
#endif
#if VK_USE_PLATFORM_XLIB_KHR
        "VK_KHR_xlib_surface",
#endif
            };
            KnownDeviceExtensions = new string[]
            {
                "VK_KHR_swapchain"
            };
        }

        void AppDevInitFormats(AppDev dev)
        {
            int formatCount = 0;
            foreach(Format f in Enum.GetValues(typeof(Format)))
            { 
                int fi = (int)f;
                formatCount = formatCount < fi ? fi : formatCount;
            }

            FormatProperties[] formatList = new FormatProperties[formatCount+1];
            foreach (Format f in Enum.GetValues(typeof(Format)))
            {
                int index = (int)f;
                formatList[index] = (dev.Gpu.Obj.GetFormatProperties(f));
            }

            dev.FormatProbs = formatList;
        }

        void ExtractVersion(uint version, out uint major, out uint minor, out uint patch)
        {
            major = version >> 22;
            minor = version >> 11 & 0x3ff;
            patch = version & 0xfff;
        }

        AppDev AppDevInit(AppGpu gpu)
        {
            DeviceCreateInfo info = new DeviceCreateInfo
            {
                QueueCreateInfoCount = 0, // TODO : this sould not be 
                QueueCreateInfos = new DeviceQueueCreateInfo[0],
                EnabledLayerCount = 0,
                EnabledLayerNames = new string[0],
            };

            // Scan layers
            List<LayerExtensionList> layers = new List<LayerExtensionList>();
            LayerProperties[] layerProperties = gpu.Obj.EnumerateDeviceLayerProperties();
            if (layerProperties != null)
            {
                foreach (LayerProperties layer in layerProperties)
                {
                    LayerExtensionList layerExtList = new LayerExtensionList
                    {
                        LayerProperties = layer,
                        ExtensionProperties = gpu.Obj.EnumerateDeviceExtensionProperties(layer.LayerName),
                    };
                    if (layerExtList.ExtensionProperties == null)
                    {
                        layerExtList.ExtensionProperties = new ExtensionProperties[0];
                    }
                    layers.Add(layerExtList);
                }
            }

            ExtensionProperties[] extensions = gpu.Obj.EnumerateDeviceExtensionProperties("");

            foreach (string knownExtName in KnownDeviceExtensions)
            {
                bool extensionFound = false;
                foreach (ExtensionProperties extention in extensions)
                {
                    if (extention.ExtensionName == knownExtName)
                    {
                        extensionFound = true;
                        break;
                    }
                }

                if (!extensionFound)
                {
                    throw new Exception("Cannot find extension: " + knownExtName);
                }
            }

            gpu.DeviceLayers = layers.ToArray();
            gpu.DeviceExtensions = extensions;

            info.QueueCreateInfoCount = (uint)gpu.QueueReqs.Length;
            info.QueueCreateInfos = gpu.QueueReqs;

            info.EnabledExtensionNames = KnownDeviceExtensions;
            info.EnabledExtensionCount = (uint)KnownDeviceExtensions.Length;

            Device device = gpu.Obj.CreateDevice(info, null);

            return new AppDev
            {
                Gpu = gpu,
                Obj = device,
            };
        }
        
        void AppDevDestroy(AppDev dev)
        {
            dev.Obj.Destroy(null);
        }

        AppInstance AppCreateInstance(uint apiVersion)
        {
            ApplicationInfo appInfo = new ApplicationInfo
            {
                ApplicationName = ApplicationName,
                ApplicationVersion = ApplicationVersion,
                EngineName = EngineName,
                EngineVersion = EnginveVersion,
                ApiVersion = apiVersion,
            };

            InstanceCreateInfo createInfo = new InstanceCreateInfo
            {
                ApplicationInfo = appInfo,
                EnabledLayerCount = 0,
                EnabledExtensionCount = 0,
            };

            // Scan layers
            List<LayerExtensionList> layers = new List<LayerExtensionList>();
            LayerProperties[] layerProperties = Commands.EnumerateInstanceLayerProperties();
            if (layerProperties != null)
            {
                foreach (LayerProperties layer in layerProperties)
                {
                    LayerExtensionList layerExtList = new LayerExtensionList
                    {
                        LayerProperties = layer,
                        ExtensionProperties = Commands.EnumerateInstanceExtensionProperties(layer.LayerName),
                    };
                    if (layerExtList.ExtensionProperties == null)
                    {
                        layerExtList.ExtensionProperties = new ExtensionProperties[0];
                    }
                    layers.Add(layerExtList);
                }
            }

            ExtensionProperties[] extensions = Commands.EnumerateInstanceExtensionProperties("");
            if(extensions == null)
            {
                extensions = new ExtensionProperties[0];
            }

            foreach (string knownExtName in KnownExtensions)
            {
                bool extensionFound = false;
                foreach (ExtensionProperties extention in extensions)
                {
                    if (extention.ExtensionName == knownExtName)
                    {
                        extensionFound = true;
                        break;
                    }
                }

                if (!extensionFound)
                {
                    throw new Exception("Cannot find extension: " + knownExtName);
                }
            }

            createInfo.EnabledExtensionNames = KnownExtensions;
            createInfo.EnabledExtensionCount = (uint)KnownExtensions.Length;

            // TODO : Register debug callback

            Instance instance = new Instance(createInfo);

            return new AppInstance
            {
                Instance = instance,
                Layers = layers.ToArray(),
                Extensions = extensions,
            };
        }

        void AppDestroyInstance(AppInstance instance)
        {
            // TODO : Check if we need to free some structs

            instance.Instance.Destroy(null);
        }

        AppGpu AppGpuInit(uint id, PhysicalDevice obj)
        {
            // TODO : Limits

            AppGpu gpu = new AppGpu
            {
                Id = id,
                Obj = obj,
                Props = obj.GetProperties(),
                QueueProps = obj.GetQueueFamilyProperties(),
                MemoryProps = obj.GetMemoryProperties(),
                Features = obj.GetFeatures(),
                Limits = null,
            };

            gpu.QueueReqs = new DeviceQueueCreateInfo[gpu.QueueProps.Length];
            for (uint i = 0; i < gpu.QueueProps.Length; i++)
            {
                uint queueCount = gpu.QueueProps[i].QueueCount;
                DeviceQueueCreateInfo queueReq = new DeviceQueueCreateInfo
                { 
                    QueueFamilyIndex = i,
                    QueueCount = queueCount,
                    QueuePriorities = new float[queueCount],
                };
                gpu.QueueReqs[i] = queueReq;
            }

            gpu.Device = AppDevInit(gpu);            
            AppDevInitFormats(gpu.Device);

            return gpu;
        }

        void AppGpuDestroy(AppGpu gpu)
        {
            AppDevDestroy(gpu.Device);

            // TODO : Check if we need to free some structs
        }

        string GetVkName(string name, string prefix = "", string surfix = "")
        {
            string str = prefix;
            bool wasUpper = true;
            foreach (char c in name)
            {
                bool isUpper = Char.IsUpper(c);
                if(!wasUpper && isUpper)
                {
                    str += "_";
                }
                str += char.ToUpper(c);
                wasUpper = isUpper;
            }
            return str + surfix;
        }

        void AppDevDumpFormatProps(AppDev dev, Format fmt, StreamWriter output)
        {
            FormatProperties props = dev.FormatProbs[(int)fmt];

            Feature[] features = new Feature[3];
            features[0].Name = "linearTiling   FormatFeatureFlags";
            features[0].Flags = (FormatFeatureFlags)props.LinearTilingFeatures;
            features[1].Name = "optimalTiling  FormatFeatureFlags";
            features[1].Flags = (FormatFeatureFlags)props.OptimalTilingFeatures;
            features[2].Name = "bufferFeatures FormatFeatureFlags";
            features[2].Flags = (FormatFeatureFlags)props.BufferFeatures;

            output.Write("\nFORMAT_{0}:", GetVkName(fmt.ToString()));
            foreach(Feature feature in features)
            {
                output.Write("\n\t{0}:", feature.Name);
                if(feature.Flags == 0)
                {
                    output.Write("\n\t\tNone");
                }
                else
                {
                    foreach(FormatFeatureFlags flag in Enum.GetValues(typeof(FormatFeatureFlags)))
                    {
                        if ((feature.Flags & flag) == flag)
                        {
                            string name = GetVkName(flag.ToString(), "VK_FORMAT_FEATURE_", "_BIT");
                            output.Write("\n\t\t{0}", name);
                        }
                    }
                }
            }
            output.WriteLine();
        }

        void AppDevDump(AppDev dev, StreamWriter output)
        {
            foreach (Format fmt in Enum.GetValues(typeof(Format)))
            {
                AppDevDumpFormatProps(dev, fmt, output);
            }
        }

        void AppGpuDumpFeatures(AppGpu gpu, StreamWriter output)
        {
            PhysicalDeviceFeatures features = gpu.Features;

            output.WriteLine("VkPhysicalDeviceFeatures:");
            output.WriteLine("=========================");

            output.WriteLine("\trobustBufferAccess                      = {0}", features.RobustBufferAccess);
            output.WriteLine("\tfullDrawIndexUint32                     = {0}", features.FullDrawIndexUint32);
            output.WriteLine("\timageCubeArray                          = {0}", features.ImageCubeArray);
            output.WriteLine("\tindependentBlend                        = {0}", features.IndependentBlend);
            output.WriteLine("\tgeometryShader                          = {0}", features.GeometryShader);
            output.WriteLine("\ttessellationShader                      = {0}", features.TessellationShader);
            output.WriteLine("\tsampleRateShading                       = {0}", features.SampleRateShading);
            output.WriteLine("\tdualSrcBlend                            = {0}", features.DualSrcBlend);
            output.WriteLine("\tlogicOp                                 = {0}", features.LogicOp);
            output.WriteLine("\tmultiDrawIndirect                       = {0}", features.MultiDrawIndirect);
            output.WriteLine("\tdrawIndirectFirstInstance               = {0}", features.DrawIndirectFirstInstance);
            output.WriteLine("\tdepthClamp                              = {0}", features.DepthClamp);
            output.WriteLine("\tdepthBiasClamp                          = {0}", features.DepthBiasClamp);
            output.WriteLine("\tfillModeNonSolid                        = {0}", features.FillModeNonSolid);
            output.WriteLine("\tdepthBounds                             = {0}", features.DepthBounds);
            output.WriteLine("\twideLines                               = {0}", features.WideLines);
            output.WriteLine("\tlargePoints                             = {0}", features.LargePoints);
            output.WriteLine("\ttextureCompressionETC2                  = {0}", features.TextureCompressionETC2);
            output.WriteLine("\ttextureCompressionASTC_LDR              = {0}", features.TextureCompressionASTCLdr);
            output.WriteLine("\ttextureCompressionBC                    = {0}", features.TextureCompressionBC);
            output.WriteLine("\tocclusionQueryPrecise                   = {0}", features.OcclusionQueryPrecise);
            output.WriteLine("\tpipelineStatisticsQuery                 = {0}", features.PipelineStatisticsQuery);
            output.WriteLine("\tvertexSideEffects                       = {0}", features.VertexPipelineStoresAndAtomics);
            output.WriteLine("\ttessellationSideEffects                 = {0}", features.FragmentStoresAndAtomics);
            output.WriteLine("\tgeometrySideEffects                     = {0}", features.ShaderTessellationAndGeometryPointSize);
            output.WriteLine("\tshaderImageGatherExtended               = {0}", features.ShaderImageGatherExtended);
            output.WriteLine("\tshaderStorageImageExtendedFormats       = {0}", features.ShaderStorageImageExtendedFormats);
            output.WriteLine("\tshaderStorageImageMultisample           = {0}", features.ShaderStorageImageMultisample);
            output.WriteLine("\tshaderStorageImageReadWithoutFormat     = {0}", features.ShaderStorageImageReadWithoutFormat);
            output.WriteLine("\tshaderStorageImageWriteWithoutFormat    = {0}", features.ShaderStorageImageWriteWithoutFormat);
            output.WriteLine("\tshaderUniformBufferArrayDynamicIndexing = {0}", features.ShaderUniformBufferArrayDynamicIndexing);
            output.WriteLine("\tshaderSampledImageArrayDynamicIndexing  = {0}", features.ShaderSampledImageArrayDynamicIndexing);
            output.WriteLine("\tshaderStorageBufferArrayDynamicIndexing = {0}", features.ShaderStorageBufferArrayDynamicIndexing);
            output.WriteLine("\tshaderStorageImageArrayDynamicIndexing  = {0}", features.ShaderStorageImageArrayDynamicIndexing);
            output.WriteLine("\tshaderClipDistance                      = {0}", features.ShaderClipDistance);
            output.WriteLine("\tshaderCullDistance                      = {0}", features.ShaderCullDistance);
            output.WriteLine("\tshaderFloat64                           = {0}", features.ShaderFloat64);
            output.WriteLine("\tshaderInt64                             = {0}", features.ShaderInt64);
            output.WriteLine("\tshaderInt16                             = {0}", features.ShaderInt16);
            output.WriteLine("\tshaderResourceResidency                 = {0}", features.ShaderResourceResidency);
            output.WriteLine("\tshaderResourceMinLod                    = {0}", features.ShaderResourceMinLod);
            output.WriteLine("\talphaToOne                              = {0}", features.AlphaToOne);
            output.WriteLine("\tsparseBinding                           = {0}", features.SparseBinding);
            output.WriteLine("\tsparseResidencyBuffer                   = {0}", features.SparseResidencyBuffer);
            output.WriteLine("\tsparseResidencyImage2D                  = {0}", features.SparseResidencyImage2D);
            output.WriteLine("\tsparseResidencyImage3D                  = {0}", features.SparseResidencyImage3D);
            output.WriteLine("\tsparseResidency2Samples                 = {0}", features.SparseResidency2Samples);
            output.WriteLine("\tsparseResidency4Samples                 = {0}", features.SparseResidency4Samples);
            output.WriteLine("\tsparseResidency8Samples                 = {0}", features.SparseResidency8Samples);
            output.WriteLine("\tsparseResidency16Samples                = {0}", features.SparseResidency16Samples);
            output.WriteLine("\tsparseResidencyAliased                  = {0}", features.SparseResidencyAliased);
            output.WriteLine("\tvariableMultisampleRate                 = {0}", features.VariableMultisampleRate);
            output.WriteLine("\tiheritedQueries                         = {0}", features.InheritedQueries);
        }

        void AppDumpSparseProps(PhysicalDeviceSparseProperties sparseProps, StreamWriter output)
        {
            output.WriteLine("\tVkPhysicalDeviceSparseProperties:");
            output.WriteLine("\t---------------------------------");

            output.WriteLine("\t\tresidencyStandard2DBlockShape            = {0}", sparseProps.ResidencyStandard2DBlockShape);
            output.WriteLine("\t\tresidencyStandard2DMultisampleBlockShape = {0}", sparseProps.ResidencyStandard2DMultisampleBlockShape);
            output.WriteLine("\t\tresidencyStandard3DBlockShape            = {0}", sparseProps.ResidencyStandard3DBlockShape);
            output.WriteLine("\t\tresidencyAlignedMipSize                  = {0}", sparseProps.ResidencyAlignedMipSize);
            output.WriteLine("\t\tresidencyNonResidentStrict               = {0}", sparseProps.ResidencyNonResidentStrict);
        }

        void AppDumpLimits(PhysicalDeviceLimits limits, StreamWriter output)
        {
            output.WriteLine("\tVkPhysicalDeviceLimits:");
            output.WriteLine("\t-----------------------");

            output.WriteLine("\t\tmaxImageDimension1D                     = 0x{0:x}", limits.MaxImageDimension1D);
            output.WriteLine("\t\tmaxImageDimension2D                     = 0x{0:x}", limits.MaxImageDimension2D);
            output.WriteLine("\t\tmaxImageDimension3D                     = 0x{0:x}", limits.MaxImageDimension3D);
            output.WriteLine("\t\tmaxImageDimensionCube                   = 0x{0:x}", limits.MaxImageDimensionCube);
            output.WriteLine("\t\tmaxImageArrayLayers                     = 0x{0:x}", limits.MaxImageArrayLayers);
            output.WriteLine("\t\tmaxTexelBufferElements                  = 0x{0:x}", limits.MaxTexelBufferElements);
            output.WriteLine("\t\tmaxUniformBufferRange                   = 0x{0:x}", limits.MaxUniformBufferRange);
            output.WriteLine("\t\tmaxStorageBufferRange                   = 0x{0:x}", limits.MaxStorageBufferRange);
            output.WriteLine("\t\tmaxPushConstantsSize                    = 0x{0:x}", limits.MaxPushConstantsSize);
            output.WriteLine("\t\tmaxMemoryAllocationCount                = 0x{0:x}", limits.MaxMemoryAllocationCount);
            output.WriteLine("\t\tmaxSamplerAllocationCount               = 0x{0:x}", limits.MaxSamplerAllocationCount);
            output.WriteLine("\t\tbufferImageGranularity                  = 0x{0:x}", limits.BufferImageGranularity);
            output.WriteLine("\t\tsparseAddressSpaceSize                  = 0x{0:x}", (ulong)limits.SparseAddressSpaceSize);
            output.WriteLine("\t\tmaxBoundDescriptorSets                  = 0x{0:x}", limits.MaxBoundDescriptorSets);
            output.WriteLine("\t\tmaxPerStageDescriptorSamplers           = 0x{0:x}", limits.MaxPerStageDescriptorSamplers);
            output.WriteLine("\t\tmaxPerStageDescriptorUniformBuffers     = 0x{0:x}", limits.MaxPerStageDescriptorUniformBuffers);
            output.WriteLine("\t\tmaxPerStageDescriptorStorageBuffers     = 0x{0:x}", limits.MaxPerStageDescriptorStorageBuffers);
            output.WriteLine("\t\tmaxPerStageDescriptorSampledImages      = 0x{0:x}", limits.MaxPerStageDescriptorSampledImages);
            output.WriteLine("\t\tmaxPerStageDescriptorStorageImages      = 0x{0:x}", limits.MaxPerStageDescriptorStorageImages);
            output.WriteLine("\t\tmaxPerStageDescriptorInputAttachments   = 0x{0:x}", limits.MaxPerStageDescriptorInputAttachments);
            output.WriteLine("\t\tmaxPerStageResources                    = 0x{0:x}", limits.MaxPerStageResources);
            output.WriteLine("\t\tmaxDescriptorSetSamplers                = 0x{0:x}", limits.MaxDescriptorSetSamplers);
            output.WriteLine("\t\tmaxDescriptorSetUniformBuffers          = 0x{0:x}", limits.MaxDescriptorSetUniformBuffers);
            output.WriteLine("\t\tmaxDescriptorSetUniformBuffersDynamic   = 0x{0:x}", limits.MaxDescriptorSetUniformBuffersDynamic);
            output.WriteLine("\t\tmaxDescriptorSetStorageBuffers          = 0x{0:x}", limits.MaxDescriptorSetStorageBuffers);
            output.WriteLine("\t\tmaxDescriptorSetStorageBuffersDynamic   = 0x{0:x}", limits.MaxDescriptorSetStorageBuffersDynamic);
            output.WriteLine("\t\tmaxDescriptorSetSampledImages           = 0x{0:x}", limits.MaxDescriptorSetSampledImages);
            output.WriteLine("\t\tmaxDescriptorSetStorageImages           = 0x{0:x}", limits.MaxDescriptorSetStorageImages);
            output.WriteLine("\t\tmaxDescriptorSetInputAttachments        = 0x{0:x}", limits.MaxDescriptorSetInputAttachments);
            output.WriteLine("\t\tmaxVertexInputAttributes                = 0x{0:x}", limits.MaxVertexInputAttributes);
            output.WriteLine("\t\tmaxVertexInputBindings                  = 0x{0:x}", limits.MaxVertexInputBindings);
            output.WriteLine("\t\tmaxVertexInputAttributeOffset           = 0x{0:x}", limits.MaxVertexInputAttributeOffset);
            output.WriteLine("\t\tmaxVertexInputBindingStride             = 0x{0:x}", limits.MaxVertexInputBindingStride);
            output.WriteLine("\t\tmaxVertexOutputComponents               = 0x{0:x}", limits.MaxVertexOutputComponents);
            output.WriteLine("\t\tmaxTessellationGenerationLevel          = 0x{0:x}", limits.MaxTessellationGenerationLevel);
            output.WriteLine("\t\tmaxTessellationPatchSize                        = 0x{0:x}", limits.MaxTessellationPatchSize);
            output.WriteLine("\t\tmaxTessellationControlPerVertexInputComponents  = 0x{0:x}", limits.MaxTessellationControlPerVertexInputComponents);
            output.WriteLine("\t\tmaxTessellationControlPerVertexOutputComponents = 0x{0:x}", limits.MaxTessellationControlPerVertexOutputComponents);
            output.WriteLine("\t\tmaxTessellationControlPerPatchOutputComponents  = 0x{0:x}", limits.MaxTessellationControlPerPatchOutputComponents);
            output.WriteLine("\t\tmaxTessellationControlTotalOutputComponents     = 0x{0:x}", limits.MaxTessellationControlTotalOutputComponents);
            output.WriteLine("\t\tmaxTessellationEvaluationInputComponents        = 0x{0:x}", limits.MaxTessellationEvaluationInputComponents);
            output.WriteLine("\t\tmaxTessellationEvaluationOutputComponents       = 0x{0:x}", limits.MaxTessellationEvaluationOutputComponents);
            output.WriteLine("\t\tmaxGeometryShaderInvocations            = 0x{0:x}", limits.MaxGeometryShaderInvocations);
            output.WriteLine("\t\tmaxGeometryInputComponents              = 0x{0:x}", limits.MaxGeometryInputComponents);
            output.WriteLine("\t\tmaxGeometryOutputComponents             = 0x{0:x}", limits.MaxGeometryOutputComponents);
            output.WriteLine("\t\tmaxGeometryOutputVertices               = 0x{0:x}", limits.MaxGeometryOutputVertices);
            output.WriteLine("\t\tmaxGeometryTotalOutputComponents        = 0x{0:x}", limits.MaxGeometryTotalOutputComponents);
            output.WriteLine("\t\tmaxFragmentInputComponents              = 0x{0:x}", limits.MaxFragmentInputComponents);
            output.WriteLine("\t\tmaxFragmentOutputAttachments            = 0x{0:x}", limits.MaxFragmentOutputAttachments);
            output.WriteLine("\t\tmaxFragmentDualSrcAttachments           = 0x{0:x}", limits.MaxFragmentDualSrcAttachments);
            output.WriteLine("\t\tmaxFragmentCombinedOutputResources      = 0x{0:x}", limits.MaxFragmentCombinedOutputResources);
            output.WriteLine("\t\tmaxComputeSharedMemorySize              = 0x{0:x}", limits.MaxComputeSharedMemorySize);
            output.WriteLine("\t\tmaxComputeWorkGroupCount[0]             = 0x{0:x}", limits.MaxComputeWorkGroupCount[0]);
            output.WriteLine("\t\tmaxComputeWorkGroupCount[1]             = 0x{0:x}", limits.MaxComputeWorkGroupCount[1]);
            output.WriteLine("\t\tmaxComputeWorkGroupCount[2]             = 0x{0:x}", limits.MaxComputeWorkGroupCount[2]);
            output.WriteLine("\t\tmaxComputeWorkGroupInvocations          = 0x{0:x}", limits.MaxComputeWorkGroupInvocations);
            output.WriteLine("\t\tmaxComputeWorkGroupSize[0]              = 0x{0:x}", limits.MaxComputeWorkGroupSize[0]);
            output.WriteLine("\t\tmaxComputeWorkGroupSize[1]              = 0x{0:x}", limits.MaxComputeWorkGroupSize[1]);
            output.WriteLine("\t\tmaxComputeWorkGroupSize[2]              = 0x{0:x}", limits.MaxComputeWorkGroupSize[2]);
            output.WriteLine("\t\tsubPixelPrecisionBits                   = 0x{0:x}", limits.SubPixelPrecisionBits);
            output.WriteLine("\t\tsubTexelPrecisionBits                   = 0x{0:x}", limits.SubTexelPrecisionBits);
            output.WriteLine("\t\tmipmapPrecisionBits                     = 0x{0:x}", limits.MipmapPrecisionBits);
            output.WriteLine("\t\tmaxDrawIndexedIndexValue                = 0x{0:x}", limits.MaxDrawIndexedIndexValue);
            output.WriteLine("\t\tmaxDrawIndirectCount                    = 0x{0:x}", limits.MaxDrawIndirectCount);
            output.WriteLine("\t\tmaxSamplerLodBias                       = {0:0.000000}", limits.MaxSamplerLodBias);
            output.WriteLine("\t\tmaxSamplerAnisotropy                    = {0:0.000000}", limits.MaxSamplerAnisotropy);
            output.WriteLine("\t\tmaxViewports                            = 0x{0:x}", limits.MaxViewports);
            output.WriteLine("\t\tmaxViewportDimensions[0]                = 0x{0:x}", limits.MaxViewportDimensions[0]);
            output.WriteLine("\t\tmaxViewportDimensions[1]                = 0x{0:x}", limits.MaxViewportDimensions[1]);
            output.WriteLine("\t\tviewportBoundsRange[0]                  = {0:0.000000}", limits.ViewportBoundsRange[0]);
            output.WriteLine("\t\tviewportBoundsRange[1]                  = {0:0.000000}", limits.ViewportBoundsRange[1]);
            output.WriteLine("\t\tviewportSubPixelBits                    = 0x{0:x}", limits.ViewportSubPixelBits);
            output.WriteLine("\t\tminMemoryMapAlignment                   = {0}", limits.MinMemoryMapAlignment);
            output.WriteLine("\t\tminTexelBufferOffsetAlignment           = 0x{0:x}", limits.MinTexelBufferOffsetAlignment);
            output.WriteLine("\t\tminUniformBufferOffsetAlignment         = 0x{0:x}", limits.MinUniformBufferOffsetAlignment);
            output.WriteLine("\t\tminStorageBufferOffsetAlignment         = 0x{0:x}", limits.MinStorageBufferOffsetAlignment);
            output.WriteLine("\t\tminTexelOffset                          = 0x{0:x}", limits.MinTexelOffset);
            output.WriteLine("\t\tmaxTexelOffset                          = 0x{0:x}", limits.MaxTexelOffset);
            output.WriteLine("\t\tminTexelGatherOffset                    = 0x{0:x}", limits.MinTexelGatherOffset);
            output.WriteLine("\t\tmaxTexelGatherOffset                    = 0x{0:x}", limits.MaxTexelGatherOffset);
            output.WriteLine("\t\tminInterpolationOffset                  = {0:0.000000}", limits.MinInterpolationOffset);
            output.WriteLine("\t\tmaxInterpolationOffset                  = {0:0.000000}", limits.MaxInterpolationOffset);
            output.WriteLine("\t\tsubPixelInterpolationOffsetBits         = 0x{0:x}", limits.SubPixelInterpolationOffsetBits);
            output.WriteLine("\t\tmaxFramebufferWidth                     = 0x{0:x}", limits.MaxFramebufferWidth);
            output.WriteLine("\t\tmaxFramebufferHeight                    = 0x{0:x}", limits.MaxFramebufferHeight);
            output.WriteLine("\t\tmaxFramebufferLayers                    = 0x{0:x}", limits.MaxFramebufferLayers);
            output.WriteLine("\t\tframebufferColorSampleCounts            = 0x{0:x}", limits.FramebufferColorSampleCounts);
            output.WriteLine("\t\tframebufferDepthSampleCounts            = 0x{0:x}", limits.FramebufferDepthSampleCounts);
            output.WriteLine("\t\tframebufferStencilSampleCounts          = 0x{0:x}", limits.FramebufferStencilSampleCounts);
            output.WriteLine("\t\tmaxColorAttachments                     = 0x{0:x}", limits.MaxColorAttachments);
            output.WriteLine("\t\tsampledImageColorSampleCounts           = 0x{0:x}", limits.SampledImageColorSampleCounts);
            output.WriteLine("\t\tsampledImageDepthSampleCounts           = 0x{0:x}", limits.SampledImageDepthSampleCounts);
            output.WriteLine("\t\tsampledImageStencilSampleCounts         = 0x{0:x}", limits.SampledImageStencilSampleCounts);
            output.WriteLine("\t\tsampledImageIntegerSampleCounts         = 0x{0:x}", limits.SampledImageIntegerSampleCounts);
            output.WriteLine("\t\tstorageImageSampleCounts                = 0x{0:x}", limits.StorageImageSampleCounts);
            output.WriteLine("\t\tmaxSampleMaskWords                      = 0x{0:x}", limits.MaxSampleMaskWords);
            output.WriteLine("\t\ttimestampComputeAndGraphics             = {0}", Convert.ToInt32(limits.TimestampComputeAndGraphics));
            output.WriteLine("\t\ttimestampPeriod                         = {0:0.000000}", limits.TimestampPeriod);
            output.WriteLine("\t\tmaxClipDistances                        = 0x{0:x}", limits.MaxClipDistances);
            output.WriteLine("\t\tmaxCullDistances                        = 0x{0:x}", limits.MaxCullDistances);
            output.WriteLine("\t\tmaxCombinedClipAndCullDistances         = 0x{0:x}", limits.MaxCombinedClipAndCullDistances);
            output.WriteLine("\t\tpointSizeRange[0]                       = {0:0.000000}", limits.PointSizeRange[0]);
            output.WriteLine("\t\tpointSizeRange[1]                       = {0:0.000000}", limits.PointSizeRange[1]);
            output.WriteLine("\t\tlineWidthRange[0]                       = {0:0.000000}", limits.LineWidthRange[0]);
            output.WriteLine("\t\tlineWidthRange[1]                       = {0:0.000000}", limits.LineWidthRange[1]);
            output.WriteLine("\t\tpointSizeGranularity                    = {0:0.000000}", limits.PointSizeGranularity);
            output.WriteLine("\t\tlineWidthGranularity                    = {0:0.000000}", limits.LineWidthGranularity);
            output.WriteLine("\t\tstrictLines                             = {0}", Convert.ToInt32(limits.StrictLines));
            output.WriteLine("\t\tstandardSampleLocations                 = {0}", Convert.ToInt32(limits.StandardSampleLocations));
            output.WriteLine("\t\toptimalBufferCopyOffsetAlignment        = 0x{0:x}", limits.OptimalBufferCopyOffsetAlignment);
            output.WriteLine("\t\toptimalBufferCopyRowPitchAlignment      = 0x{0:x}", limits.OptimalBufferCopyRowPitchAlignment);
            output.WriteLine("\t\tnonCoherentAtomSize                     = 0x{0:x}", limits.NonCoherentAtomSize);
        }

        void AppGpuDumpProps(AppGpu gpu, StreamWriter output)
        {
            PhysicalDeviceProperties props = gpu.Props;

            output.WriteLine("VkPhysicalDeviceProperties:");
            output.WriteLine("===========================");
            output.WriteLine("\tapiVersion     = {0}", props.ApiVersion);
            output.WriteLine("\tdriverVersion  = {0}", props.DriverVersion);
            output.WriteLine("\tvendorID       = 0x{0:x}", props.VendorID);
            output.WriteLine("\tdeviceID       = 0x{0:x}", props.DeviceID);
            output.WriteLine("\tdeviceType     = {0}", GetVkName(props.DeviceType.ToString(), "", ""));
            output.WriteLine("\tdeviceName     = {0}", props.DeviceName);

            AppDumpLimits(props.Limits, output);
            AppDumpSparseProps(props.SparseProperties, output);;
        }

        void AppDumpExtensions(string ident, string layerName, ExtensionProperties[] extensionProperties, StreamWriter output)
        {
            if (!string.IsNullOrEmpty(layerName))
            {
                output.Write("{0}{1} Extensions", ident, layerName);
            }
            else
            {
                output.Write("Extensions");
            }

            output.WriteLine("\tcount = {0}", extensionProperties.Length);
            foreach (ExtensionProperties extProp in extensionProperties)
            {
                output.Write("{0}\t", ident);
                output.WriteLine("{0,-32}: extension revision {1,2}", extProp.ExtensionName, extProp.SpecVersion);
            }

            output.WriteLine();
        }
        
        void AppGpuDumpQueuProps(AppGpu gpu, uint id, StreamWriter output)
        {
            QueueFamilyProperties props = gpu.QueueProps[id];

            output.WriteLine("VkQueueFamilyProperties[{0}]:", id);
            output.WriteLine("============================");
            output.WriteLine("\tqueueFlags         = {0}{1}{2}",
                   ((QueueFlags)props.QueueFlags & QueueFlags.Graphics) == QueueFlags.Graphics ? 'G' : '.',
                   ((QueueFlags)props.QueueFlags & QueueFlags.Compute) == QueueFlags.Compute ? 'C' : '.',
                   ((QueueFlags)props.QueueFlags & QueueFlags.Transfer) == QueueFlags.Transfer ? 'D' : '.');
                   //((QueueFlags)props.QueueFlags & QueueFlags.SparseBinding) == QueueFlags.SparseBinding ? 'S' : '.'); // Add this option, not pressent in original
            output.WriteLine("\tqueueCount         = {0}", props.QueueCount);
            output.WriteLine("\ttimestampValidBits = {0}", props.TimestampValidBits);
            output.WriteLine("\tminImageTransferGranularity = ({0}, {1}, {2})",
                   props.MinImageTransferGranularity.Width,
                   props.MinImageTransferGranularity.Height,
                   props.MinImageTransferGranularity.Depth);
        }

        void AppGpuDumpMemoryProps(AppGpu gpu, StreamWriter output)
        {
            PhysicalDeviceMemoryProperties props = gpu.MemoryProps;

            output.WriteLine("VkPhysicalDeviceMemoryProperties:");
            output.WriteLine("=================================");
            output.WriteLine("\tmemoryTypeCount       = {0}", props.MemoryTypeCount);
            for(int i = 0; i < props.MemoryTypeCount; i++)
            {
                MemoryType memoryType = props.MemoryTypes[i];

                output.WriteLine("\tmemoryTypes[{0}] : ", i);
                output.WriteLine("\t\tpropertyFlags = {0}", memoryType.PropertyFlags);
                output.WriteLine("\t\theapIndex     = {0}", memoryType.HeapIndex);
            }
            output.WriteLine("\tmemoryHeapCount       = {0}", props.MemoryHeapCount);

            for (int i = 0; i < props.MemoryHeapCount; i++)
            {
                MemoryHeap memoryHeap = props.MemoryHeaps[i];

                output.WriteLine("\tmemoryHeaps[{0}] : ", i);
                output.WriteLine("\t\tsize          = {0}", memoryHeap.Size);
            }
        }

        void AppGpuDump(AppGpu gpu, StreamWriter output)
        {
            output.WriteLine("Device Extensions and layers:");
            output.WriteLine("=============================");
            output.WriteLine("GPU{0}", gpu.Id);
            AppGpuDumpProps(gpu, output);
            output.WriteLine();
            AppDumpExtensions("", "Device", gpu.DeviceExtensions, output);

            output.WriteLine();
            output.WriteLine("Layers\tcount = {0}", gpu.DeviceLayers.Length);
            foreach (LayerExtensionList layerInfo in gpu.DeviceLayers)
            {
                uint major, minor, patch;

                ExtractVersion(layerInfo.LayerProperties.SpecVersion, out major, out minor, out patch);
                string specVersion = string.Format("{0}.{1}.{2}", major, minor, patch);
                string layerVersion = string.Format("{0}", layerInfo.LayerProperties.ImplementationVersion);

                output.WriteLine("\t{0} ({1}) Vulkan version {2}, layer version {3}",
                   layerInfo.LayerProperties.LayerName,
                   layerInfo.LayerProperties.Description, specVersion,
                   layerVersion);

                AppDumpExtensions("\t", layerInfo.LayerProperties.LayerName, layerInfo.ExtensionProperties, output);
            }

            output.WriteLine();
                        
            for (uint i = 0; i < gpu.QueueProps.Length; i++)
            {
                AppGpuDumpQueuProps(gpu, i, output);
                output.WriteLine();
            }

            AppGpuDumpMemoryProps(gpu, output);
            output.WriteLine();
            AppGpuDumpFeatures(gpu, output);
            output.WriteLine();
            AppDevDump(gpu.Device, output);
        }

        public void DumpInfo(StreamWriter output)
        {
            uint apiVersion = Vulkan.Version.Make(1, 0, 0);

            DumpHeader(apiVersion, output);

            AppInstance instance = AppCreateInstance(apiVersion);
            output.WriteLine("Instance Extensions and layers:");
            output.WriteLine("===============================");
            AppDumpExtensions("", "Instance", instance.Extensions, output);

            output.WriteLine("Instance Layers\tcount = {0}", instance.Layers.Length);
            foreach (LayerExtensionList layer in instance.Layers)
            {
                LayerProperties layerProp = layer.LayerProperties;

                uint major, minor, patch;

                ExtractVersion(layerProp.SpecVersion, out major, out minor, out patch);
                string specVersion = string.Format("{0}.{1}.{2}", major, minor, patch);
                string layerVersion = string.Format("{0}", layerProp.ImplementationVersion);

                output.WriteLine("\t{0} ({1}) Vulkan version {2}, layer version {3}",
                    layerProp.LayerName, layerProp.Description,
                    specVersion, layerVersion);

                AppDumpExtensions("\t", layerProp.LayerName, layer.ExtensionProperties, output);
            }
            
            PhysicalDevice[] objs = instance.Instance.EnumeratePhysicalDevices();
            AppGpu[] gpus = new AppGpu[objs.Length];

            for(uint i = 0; i < objs.Length; i++)
            {
                gpus[i] = AppGpuInit(i, objs[i]);
                AppGpuDump(gpus[i], output);
                output.WriteLine();
                output.WriteLine();
            }

            for (uint i = 0; i < gpus.Length; i++)
            {
                AppGpuDestroy(gpus[i]);
            }

            AppDestroyInstance(instance);
            output.Flush();
        }

        void DumpHeader(uint apiVersion, StreamWriter output)
        {
            uint major, minor, patch;

            ExtractVersion(apiVersion, out major, out minor, out patch);

            output.WriteLine("===========");
            output.WriteLine("VULKAN INFO");
            output.WriteLine("===========\n");
            output.WriteLine("Vulkan API Version: {0}.{1}.{2}\n", major, minor, patch);
        }

        class AppInstance // app_instance 
        {
            public Instance Instance;
            public LayerExtensionList[] Layers;
            public ExtensionProperties[] Extensions;
        }

        class LayerExtensionList // layer_extension_list 
        {
            public LayerProperties LayerProperties;
            public ExtensionProperties[] ExtensionProperties;
        }

        class AppGpu // app_gpu 
        {
            public uint Id;
            public PhysicalDevice Obj;
            public PhysicalDeviceProperties Props;

            public QueueFamilyProperties[] QueueProps;
            public DeviceQueueCreateInfo[] QueueReqs;

            public PhysicalDeviceMemoryProperties MemoryProps;
            public PhysicalDeviceFeatures Features;
            public PhysicalDeviceLimits Limits;

            public LayerExtensionList[] DeviceLayers;
            public ExtensionProperties[] DeviceExtensions;

            public AppDev Device;
        }

        class AppDev // app_dev 
        {
            public AppGpu Gpu;
            public Device Obj;
            public FormatProperties[] FormatProbs; /*VK_FORMAT_RANGE_SIZE*/
        }

        struct Feature
        {
            public string Name;
            public FormatFeatureFlags Flags;
        }       
    }
}
