using System;

namespace Vulkan.Interop
{
	public struct Offset2D
	{
		public Int32 X;
		public Int32 Y;
	}

	public struct Offset3D
	{
		public Int32 X;
		public Int32 Y;
		public Int32 Z;
	}

	public struct Extent2D
	{
		public UInt32 Width;
		public UInt32 Height;
	}

	public struct Extent3D
	{
		public UInt32 Width;
		public UInt32 Height;
		public UInt32 Depth;
	}

	public struct Viewport
	{
		public float X;
		public float Y;
		public float Width;
		public float Height;
		public float MinDepth;
		public float MaxDepth;
	}

	public struct Rect2D
	{
		public IntPtr Offset;
		public IntPtr Extent;
	}

	public struct Rect3D
	{
		public IntPtr Offset;
		public IntPtr Extent;
	}

	public struct ClearRect
	{
		public IntPtr Rect;
		public UInt32 BaseArrayLayer;
		public UInt32 LayerCount;
	}

	public struct ComponentMapping
	{
		public ComponentSwizzle R;
		public ComponentSwizzle G;
		public ComponentSwizzle B;
		public ComponentSwizzle A;
	}

	public struct PhysicalDeviceProperties
	{
		public UInt32 ApiVersion;
		public UInt32 DriverVersion;
		public UInt32 VendorID;
		public UInt32 DeviceID;
		public PhysicalDeviceType DeviceType;
		public char DeviceName;
		public Byte PipelineCacheUUID;
		public IntPtr Limits;
		public IntPtr SparseProperties;
	}

	public struct ExtensionProperties
	{
		public char ExtensionName;
		public UInt32 SpecVersion;
	}

	public struct LayerProperties
	{
		public char LayerName;
		public UInt32 SpecVersion;
		public UInt32 ImplementationVersion;
		public char Description;
	}

	public struct ApplicationInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr ApplicationName;
		public UInt32 ApplicationVersion;
		public IntPtr EngineName;
		public UInt32 EngineVersion;
		public UInt32 ApiVersion;
	}

	public struct AllocationCallbacks
	{
		public IntPtr UserData;
		public IntPtr PfnAllocation;
		public IntPtr PfnReallocation;
		public IntPtr PfnFree;
		public IntPtr PfnInternalAllocation;
		public IntPtr PfnInternalFree;
	}

	public struct DeviceQueueCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 QueueFamilyIndex;
		public UInt32 QueueCount;
		public float QueuePriorities;
	}

	public struct DeviceCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 QueueCreateInfoCount;
		public IntPtr QueueCreateInfos;
		public UInt32 EnabledLayerCount;
		public IntPtr PEnabledLayerNames;
		public UInt32 EnabledExtensionCount;
		public IntPtr PEnabledExtensionNames;
		public IntPtr EnabledFeatures;
	}

	public struct InstanceCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr ApplicationInfo;
		public UInt32 EnabledLayerCount;
		public IntPtr PEnabledLayerNames;
		public UInt32 EnabledExtensionCount;
		public IntPtr PEnabledExtensionNames;
	}

	public struct QueueFamilyProperties
	{
		public UInt32 QueueFlags;
		public UInt32 QueueCount;
		public UInt32 TimestampValidBits;
		public IntPtr MinImageTransferGranularity;
	}

	public struct PhysicalDeviceMemoryProperties
	{
		public UInt32 MemoryTypeCount;
		public IntPtr MemoryTypes;
		public UInt32 MemoryHeapCount;
		public IntPtr MemoryHeaps;
	}

	public struct MemoryAllocateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public DeviceSize AllocationSize;
		public UInt32 MemoryTypeIndex;
	}

	public struct MemoryRequirements
	{
		public DeviceSize Size;
		public DeviceSize Alignment;
		public UInt32 MemoryTypeBits;
	}

	public struct SparseImageFormatProperties
	{
		public UInt32 AspectMask;
		public IntPtr ImageGranularity;
		public UInt32 Flags;
	}

	public struct SparseImageMemoryRequirements
	{
		public IntPtr FormatProperties;
		public UInt32 ImageMipTailFirstLod;
		public DeviceSize ImageMipTailSize;
		public DeviceSize ImageMipTailOffset;
		public DeviceSize ImageMipTailStride;
	}

	public struct MemoryType
	{
		public UInt32 PropertyFlags;
		public UInt32 HeapIndex;
	}

	public struct MemoryHeap
	{
		public DeviceSize Size;
		public UInt32 Flags;
	}

	public struct MappedMemoryRange
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr Memory;
		public DeviceSize Offset;
		public DeviceSize Size;
	}

	public struct FormatProperties
	{
		public UInt32 LinearTilingFeatures;
		public UInt32 OptimalTilingFeatures;
		public UInt32 BufferFeatures;
	}

	public struct ImageFormatProperties
	{
		public IntPtr MaxExtent;
		public UInt32 MaxMipLevels;
		public UInt32 MaxArrayLayers;
		public UInt32 SampleCounts;
		public DeviceSize MaxResourceSize;
	}

	public struct DescriptorBufferInfo
	{
		public IntPtr Buffer;
		public DeviceSize Offset;
		public DeviceSize Range;
	}

	public struct DescriptorImageInfo
	{
		public IntPtr Sampler;
		public IntPtr ImageView;
		public ImageLayout ImageLayout;
	}

	public struct WriteDescriptorSet
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr DstSet;
		public UInt32 DstBinding;
		public UInt32 DstArrayElement;
		public UInt32 DescriptorCount;
		public DescriptorType DescriptorType;
		public IntPtr ImageInfo;
		public IntPtr BufferInfo;
		public IntPtr TexelBufferView;
	}

	public struct CopyDescriptorSet
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr SrcSet;
		public UInt32 SrcBinding;
		public UInt32 SrcArrayElement;
		public IntPtr DstSet;
		public UInt32 DstBinding;
		public UInt32 DstArrayElement;
		public UInt32 DescriptorCount;
	}

	public struct BufferCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public DeviceSize Size;
		public UInt32 Usage;
		public SharingMode SharingMode;
		public UInt32 QueueFamilyIndexCount;
		public UInt32 QueueFamilyIndices;
	}

	public struct BufferViewCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr Buffer;
		public Format Format;
		public DeviceSize Offset;
		public DeviceSize Range;
	}

	public struct ImageSubresource
	{
		public UInt32 AspectMask;
		public UInt32 MipLevel;
		public UInt32 ArrayLayer;
	}

	public struct ImageSubresourceLayers
	{
		public UInt32 AspectMask;
		public UInt32 MipLevel;
		public UInt32 BaseArrayLayer;
		public UInt32 LayerCount;
	}

	public struct ImageSubresourceRange
	{
		public UInt32 AspectMask;
		public UInt32 BaseMipLevel;
		public UInt32 LevelCount;
		public UInt32 BaseArrayLayer;
		public UInt32 LayerCount;
	}

	public struct MemoryBarrier
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 SrcAccessMask;
		public UInt32 DstAccessMask;
	}

	public struct BufferMemoryBarrier
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 SrcAccessMask;
		public UInt32 DstAccessMask;
		public UInt32 SrcQueueFamilyIndex;
		public UInt32 DstQueueFamilyIndex;
		public IntPtr Buffer;
		public DeviceSize Offset;
		public DeviceSize Size;
	}

	public struct ImageMemoryBarrier
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 SrcAccessMask;
		public UInt32 DstAccessMask;
		public ImageLayout OldLayout;
		public ImageLayout NewLayout;
		public UInt32 SrcQueueFamilyIndex;
		public UInt32 DstQueueFamilyIndex;
		public IntPtr Image;
		public IntPtr SubresourceRange;
	}

	public struct ImageCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public ImageType ImageType;
		public Format Format;
		public IntPtr Extent;
		public UInt32 MipLevels;
		public UInt32 ArrayLayers;
		public UInt32 Samples;
		public ImageTiling Tiling;
		public UInt32 Usage;
		public SharingMode SharingMode;
		public UInt32 QueueFamilyIndexCount;
		public UInt32 QueueFamilyIndices;
		public ImageLayout InitialLayout;
	}

	public struct SubresourceLayout
	{
		public DeviceSize Offset;
		public DeviceSize Size;
		public DeviceSize RowPitch;
		public DeviceSize ArrayPitch;
		public DeviceSize DepthPitch;
	}

	public struct ImageViewCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr Image;
		public ImageViewType ViewType;
		public Format Format;
		public IntPtr Components;
		public IntPtr SubresourceRange;
	}

	public struct BufferCopy
	{
		public DeviceSize SrcOffset;
		public DeviceSize DstOffset;
		public DeviceSize Size;
	}

	public struct SparseMemoryBind
	{
		public DeviceSize ResourceOffset;
		public DeviceSize Size;
		public IntPtr Memory;
		public DeviceSize MemoryOffset;
		public UInt32 Flags;
	}

	public struct SparseImageMemoryBind
	{
		public IntPtr Subresource;
		public IntPtr Offset;
		public IntPtr Extent;
		public IntPtr Memory;
		public DeviceSize MemoryOffset;
		public UInt32 Flags;
	}

	public struct SparseBufferMemoryBindInfo
	{
		public IntPtr Buffer;
		public UInt32 BindCount;
		public IntPtr Binds;
	}

	public struct SparseImageOpaqueMemoryBindInfo
	{
		public IntPtr Image;
		public UInt32 BindCount;
		public IntPtr Binds;
	}

	public struct SparseImageMemoryBindInfo
	{
		public IntPtr Image;
		public UInt32 BindCount;
		public IntPtr Binds;
	}

	public struct BindSparseInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 WaitSemaphoreCount;
		public IntPtr WaitSemaphores;
		public UInt32 BufferBindCount;
		public IntPtr BufferBinds;
		public UInt32 ImageOpaqueBindCount;
		public IntPtr ImageOpaqueBinds;
		public UInt32 ImageBindCount;
		public IntPtr ImageBinds;
		public UInt32 SignalSemaphoreCount;
		public IntPtr SignalSemaphores;
	}

	public struct ImageCopy
	{
		public IntPtr SrcSubresource;
		public IntPtr SrcOffset;
		public IntPtr DstSubresource;
		public IntPtr DstOffset;
		public IntPtr Extent;
	}

	public struct ImageBlit
	{
		public IntPtr SrcSubresource;
		public IntPtr SrcOffsets;
		public IntPtr DstSubresource;
		public IntPtr DstOffsets;
	}

	public struct BufferImageCopy
	{
		public DeviceSize BufferOffset;
		public UInt32 BufferRowLength;
		public UInt32 BufferImageHeight;
		public IntPtr ImageSubresource;
		public IntPtr ImageOffset;
		public IntPtr ImageExtent;
	}

	public struct ImageResolve
	{
		public IntPtr SrcSubresource;
		public IntPtr SrcOffset;
		public IntPtr DstSubresource;
		public IntPtr DstOffset;
		public IntPtr Extent;
	}

	public struct ShaderModuleCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UIntPtr CodeSize;
		public UInt32 Code;
	}

	public struct DescriptorSetLayoutBinding
	{
		public UInt32 Binding;
		public DescriptorType DescriptorType;
		public UInt32 DescriptorCount;
		public UInt32 StageFlags;
		public IntPtr ImmutableSamplers;
	}

	public struct DescriptorSetLayoutCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 BindingCount;
		public IntPtr Bindings;
	}

	public struct DescriptorPoolSize
	{
		public DescriptorType Type;
		public UInt32 DescriptorCount;
	}

	public struct DescriptorPoolCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 MaxSets;
		public UInt32 PoolSizeCount;
		public IntPtr PoolSizes;
	}

	public struct DescriptorSetAllocateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr DescriptorPool;
		public UInt32 DescriptorSetCount;
		public IntPtr SetLayouts;
	}

	public struct SpecializationMapEntry
	{
		public UInt32 ConstantID;
		public UInt32 Offset;
		public UIntPtr Size;
	}

	public struct SpecializationInfo
	{
		public UInt32 MapEntryCount;
		public IntPtr MapEntries;
		public UIntPtr DataSize;
		public IntPtr Data;
	}

	public struct PipelineShaderStageCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 Stage;
		public IntPtr Module;
		public IntPtr Name;
		public IntPtr SpecializationInfo;
	}

	public struct ComputePipelineCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr Stage;
		public IntPtr Layout;
		public IntPtr BasePipelineHandle;
		public Int32 BasePipelineIndex;
	}

	public struct VertexInputBindingDescription
	{
		public UInt32 Binding;
		public UInt32 Stride;
		public VertexInputRate InputRate;
	}

	public struct VertexInputAttributeDescription
	{
		public UInt32 Location;
		public UInt32 Binding;
		public Format Format;
		public UInt32 Offset;
	}

	public struct PipelineVertexInputStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 VertexBindingDescriptionCount;
		public IntPtr VertexBindingDescriptions;
		public UInt32 VertexAttributeDescriptionCount;
		public IntPtr VertexAttributeDescriptions;
	}

	public struct PipelineInputAssemblyStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public PrimitiveTopology Topology;
		public Bool32 PrimitiveRestartEnable;
	}

	public struct PipelineTessellationStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 PatchControlPoints;
	}

	public struct PipelineViewportStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 ViewportCount;
		public IntPtr Viewports;
		public UInt32 ScissorCount;
		public IntPtr Scissors;
	}

	public struct PipelineRasterizationStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public Bool32 DepthClampEnable;
		public Bool32 RasterizerDiscardEnable;
		public PolygonMode PolygonMode;
		public UInt32 CullMode;
		public FrontFace FrontFace;
		public Bool32 DepthBiasEnable;
		public float DepthBiasConstantFactor;
		public float DepthBiasClamp;
		public float DepthBiasSlopeFactor;
		public float LineWidth;
	}

	public struct PipelineMultisampleStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 RasterizationSamples;
		public Bool32 SampleShadingEnable;
		public float MinSampleShading;
		public UInt32 SampleMask;
		public Bool32 AlphaToCoverageEnable;
		public Bool32 AlphaToOneEnable;
	}

	public struct PipelineColorBlendAttachmentState
	{
		public Bool32 BlendEnable;
		public BlendFactor SrcColorBlendFactor;
		public BlendFactor DstColorBlendFactor;
		public BlendOp ColorBlendOp;
		public BlendFactor SrcAlphaBlendFactor;
		public BlendFactor DstAlphaBlendFactor;
		public BlendOp AlphaBlendOp;
		public UInt32 ColorWriteMask;
	}

	public struct PipelineColorBlendStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public Bool32 LogicOpEnable;
		public LogicOp LogicOp;
		public UInt32 AttachmentCount;
		public IntPtr Attachments;
		public float BlendConstants;
	}

	public struct PipelineDynamicStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 DynamicStateCount;
		public DynamicState DynamicStates;
	}

	public struct StencilOpState
	{
		public StencilOp FailOp;
		public StencilOp PassOp;
		public StencilOp DepthFailOp;
		public CompareOp CompareOp;
		public UInt32 CompareMask;
		public UInt32 WriteMask;
		public UInt32 Reference;
	}

	public struct PipelineDepthStencilStateCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public Bool32 DepthTestEnable;
		public Bool32 DepthWriteEnable;
		public CompareOp DepthCompareOp;
		public Bool32 DepthBoundsTestEnable;
		public Bool32 StencilTestEnable;
		public IntPtr Front;
		public IntPtr Back;
		public float MinDepthBounds;
		public float MaxDepthBounds;
	}

	public struct GraphicsPipelineCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 StageCount;
		public IntPtr Stages;
		public IntPtr VertexInputState;
		public IntPtr InputAssemblyState;
		public IntPtr TessellationState;
		public IntPtr ViewportState;
		public IntPtr RasterizationState;
		public IntPtr MultisampleState;
		public IntPtr DepthStencilState;
		public IntPtr ColorBlendState;
		public IntPtr DynamicState;
		public IntPtr Layout;
		public IntPtr RenderPass;
		public UInt32 Subpass;
		public IntPtr BasePipelineHandle;
		public Int32 BasePipelineIndex;
	}

	public struct PipelineCacheCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UIntPtr InitialDataSize;
		public IntPtr InitialData;
	}

	public struct PushConstantRange
	{
		public UInt32 StageFlags;
		public UInt32 Offset;
		public UInt32 Size;
	}

	public struct PipelineLayoutCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 SetLayoutCount;
		public IntPtr SetLayouts;
		public UInt32 PushConstantRangeCount;
		public IntPtr PushConstantRanges;
	}

	public struct SamplerCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public Filter MagFilter;
		public Filter MinFilter;
		public SamplerMipmapMode MipmapMode;
		public SamplerAddressMode AddressModeU;
		public SamplerAddressMode AddressModeV;
		public SamplerAddressMode AddressModeW;
		public float MipLodBias;
		public Bool32 AnisotropyEnable;
		public float MaxAnisotropy;
		public Bool32 CompareEnable;
		public CompareOp CompareOp;
		public float MinLod;
		public float MaxLod;
		public BorderColor BorderColor;
		public Bool32 UnnormalizedCoordinates;
	}

	public struct CommandPoolCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 QueueFamilyIndex;
	}

	public struct CommandBufferAllocateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr CommandPool;
		public CommandBufferLevel Level;
		public UInt32 CommandBufferCount;
	}

	public struct CommandBufferInheritanceInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr RenderPass;
		public UInt32 Subpass;
		public IntPtr Framebuffer;
		public Bool32 OcclusionQueryEnable;
		public UInt32 QueryFlags;
		public UInt32 PipelineStatistics;
	}

	public struct CommandBufferBeginInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr InheritanceInfo;
	}

	public struct RenderPassBeginInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr RenderPass;
		public IntPtr Framebuffer;
		public IntPtr RenderArea;
		public UInt32 ClearValueCount;
		public IntPtr ClearValues;
	}

	public struct ClearDepthStencilValue
	{
		public float Depth;
		public UInt32 Stencil;
	}

	public struct ClearAttachment
	{
		public UInt32 AspectMask;
		public UInt32 ColorAttachment;
		public IntPtr ClearValue;
	}

	public struct AttachmentDescription
	{
		public UInt32 Flags;
		public Format Format;
		public UInt32 Samples;
		public AttachmentLoadOp LoadOp;
		public AttachmentStoreOp StoreOp;
		public AttachmentLoadOp StencilLoadOp;
		public AttachmentStoreOp StencilStoreOp;
		public ImageLayout InitialLayout;
		public ImageLayout FinalLayout;
	}

	public struct AttachmentReference
	{
		public UInt32 Attachment;
		public ImageLayout Layout;
	}

	public struct SubpassDescription
	{
		public UInt32 Flags;
		public PipelineBindPoint PipelineBindPoint;
		public UInt32 InputAttachmentCount;
		public IntPtr InputAttachments;
		public UInt32 ColorAttachmentCount;
		public IntPtr ColorAttachments;
		public IntPtr ResolveAttachments;
		public IntPtr DepthStencilAttachment;
		public UInt32 PreserveAttachmentCount;
		public UInt32 PreserveAttachments;
	}

	public struct SubpassDependency
	{
		public UInt32 SrcSubpass;
		public UInt32 DstSubpass;
		public UInt32 SrcStageMask;
		public UInt32 DstStageMask;
		public UInt32 SrcAccessMask;
		public UInt32 DstAccessMask;
		public UInt32 DependencyFlags;
	}

	public struct RenderPassCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public UInt32 AttachmentCount;
		public IntPtr Attachments;
		public UInt32 SubpassCount;
		public IntPtr Subpasses;
		public UInt32 DependencyCount;
		public IntPtr Dependencies;
	}

	public struct EventCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
	}

	public struct FenceCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
	}

	public struct PhysicalDeviceFeatures
	{
		public Bool32 RobustBufferAccess;
		public Bool32 FullDrawIndexUint32;
		public Bool32 ImageCubeArray;
		public Bool32 IndependentBlend;
		public Bool32 GeometryShader;
		public Bool32 TessellationShader;
		public Bool32 SampleRateShading;
		public Bool32 DualSrcBlend;
		public Bool32 LogicOp;
		public Bool32 MultiDrawIndirect;
		public Bool32 DrawIndirectFirstInstance;
		public Bool32 DepthClamp;
		public Bool32 DepthBiasClamp;
		public Bool32 FillModeNonSolid;
		public Bool32 DepthBounds;
		public Bool32 WideLines;
		public Bool32 LargePoints;
		public Bool32 AlphaToOne;
		public Bool32 MultiViewport;
		public Bool32 SamplerAnisotropy;
		public Bool32 TextureCompressionETC2;
		public Bool32 TextureCompressionASTCLdr;
		public Bool32 TextureCompressionBC;
		public Bool32 OcclusionQueryPrecise;
		public Bool32 PipelineStatisticsQuery;
		public Bool32 VertexPipelineStoresAndAtomics;
		public Bool32 FragmentStoresAndAtomics;
		public Bool32 ShaderTessellationAndGeometryPointSize;
		public Bool32 ShaderImageGatherExtended;
		public Bool32 ShaderStorageImageExtendedFormats;
		public Bool32 ShaderStorageImageMultisample;
		public Bool32 ShaderStorageImageReadWithoutFormat;
		public Bool32 ShaderStorageImageWriteWithoutFormat;
		public Bool32 ShaderUniformBufferArrayDynamicIndexing;
		public Bool32 ShaderSampledImageArrayDynamicIndexing;
		public Bool32 ShaderStorageBufferArrayDynamicIndexing;
		public Bool32 ShaderStorageImageArrayDynamicIndexing;
		public Bool32 ShaderClipDistance;
		public Bool32 ShaderCullDistance;
		public Bool32 ShaderFloat64;
		public Bool32 ShaderInt64;
		public Bool32 ShaderInt16;
		public Bool32 ShaderResourceResidency;
		public Bool32 ShaderResourceMinLod;
		public Bool32 SparseBinding;
		public Bool32 SparseResidencyBuffer;
		public Bool32 SparseResidencyImage2D;
		public Bool32 SparseResidencyImage3D;
		public Bool32 SparseResidency2Samples;
		public Bool32 SparseResidency4Samples;
		public Bool32 SparseResidency8Samples;
		public Bool32 SparseResidency16Samples;
		public Bool32 SparseResidencyAliased;
		public Bool32 VariableMultisampleRate;
		public Bool32 InheritedQueries;
	}

	public struct PhysicalDeviceSparseProperties
	{
		public Bool32 ResidencyStandard2DBlockShape;
		public Bool32 ResidencyStandard2DMultisampleBlockShape;
		public Bool32 ResidencyStandard3DBlockShape;
		public Bool32 ResidencyAlignedMipSize;
		public Bool32 ResidencyNonResidentStrict;
	}

	public struct PhysicalDeviceLimits
	{
		public UInt32 MaxImageDimension1D;
		public UInt32 MaxImageDimension2D;
		public UInt32 MaxImageDimension3D;
		public UInt32 MaxImageDimensionCube;
		public UInt32 MaxImageArrayLayers;
		public UInt32 MaxTexelBufferElements;
		public UInt32 MaxUniformBufferRange;
		public UInt32 MaxStorageBufferRange;
		public UInt32 MaxPushConstantsSize;
		public UInt32 MaxMemoryAllocationCount;
		public UInt32 MaxSamplerAllocationCount;
		public DeviceSize BufferImageGranularity;
		public DeviceSize SparseAddressSpaceSize;
		public UInt32 MaxBoundDescriptorSets;
		public UInt32 MaxPerStageDescriptorSamplers;
		public UInt32 MaxPerStageDescriptorUniformBuffers;
		public UInt32 MaxPerStageDescriptorStorageBuffers;
		public UInt32 MaxPerStageDescriptorSampledImages;
		public UInt32 MaxPerStageDescriptorStorageImages;
		public UInt32 MaxPerStageDescriptorInputAttachments;
		public UInt32 MaxPerStageResources;
		public UInt32 MaxDescriptorSetSamplers;
		public UInt32 MaxDescriptorSetUniformBuffers;
		public UInt32 MaxDescriptorSetUniformBuffersDynamic;
		public UInt32 MaxDescriptorSetStorageBuffers;
		public UInt32 MaxDescriptorSetStorageBuffersDynamic;
		public UInt32 MaxDescriptorSetSampledImages;
		public UInt32 MaxDescriptorSetStorageImages;
		public UInt32 MaxDescriptorSetInputAttachments;
		public UInt32 MaxVertexInputAttributes;
		public UInt32 MaxVertexInputBindings;
		public UInt32 MaxVertexInputAttributeOffset;
		public UInt32 MaxVertexInputBindingStride;
		public UInt32 MaxVertexOutputComponents;
		public UInt32 MaxTessellationGenerationLevel;
		public UInt32 MaxTessellationPatchSize;
		public UInt32 MaxTessellationControlPerVertexInputComponents;
		public UInt32 MaxTessellationControlPerVertexOutputComponents;
		public UInt32 MaxTessellationControlPerPatchOutputComponents;
		public UInt32 MaxTessellationControlTotalOutputComponents;
		public UInt32 MaxTessellationEvaluationInputComponents;
		public UInt32 MaxTessellationEvaluationOutputComponents;
		public UInt32 MaxGeometryShaderInvocations;
		public UInt32 MaxGeometryInputComponents;
		public UInt32 MaxGeometryOutputComponents;
		public UInt32 MaxGeometryOutputVertices;
		public UInt32 MaxGeometryTotalOutputComponents;
		public UInt32 MaxFragmentInputComponents;
		public UInt32 MaxFragmentOutputAttachments;
		public UInt32 MaxFragmentDualSrcAttachments;
		public UInt32 MaxFragmentCombinedOutputResources;
		public UInt32 MaxComputeSharedMemorySize;
		public UInt32 MaxComputeWorkGroupCount;
		public UInt32 MaxComputeWorkGroupInvocations;
		public UInt32 MaxComputeWorkGroupSize;
		public UInt32 SubPixelPrecisionBits;
		public UInt32 SubTexelPrecisionBits;
		public UInt32 MipmapPrecisionBits;
		public UInt32 MaxDrawIndexedIndexValue;
		public UInt32 MaxDrawIndirectCount;
		public float MaxSamplerLodBias;
		public float MaxSamplerAnisotropy;
		public UInt32 MaxViewports;
		public UInt32 MaxViewportDimensions;
		public float ViewportBoundsRange;
		public UInt32 ViewportSubPixelBits;
		public UIntPtr MinMemoryMapAlignment;
		public DeviceSize MinTexelBufferOffsetAlignment;
		public DeviceSize MinUniformBufferOffsetAlignment;
		public DeviceSize MinStorageBufferOffsetAlignment;
		public Int32 MinTexelOffset;
		public UInt32 MaxTexelOffset;
		public Int32 MinTexelGatherOffset;
		public UInt32 MaxTexelGatherOffset;
		public float MinInterpolationOffset;
		public float MaxInterpolationOffset;
		public UInt32 SubPixelInterpolationOffsetBits;
		public UInt32 MaxFramebufferWidth;
		public UInt32 MaxFramebufferHeight;
		public UInt32 MaxFramebufferLayers;
		public UInt32 FramebufferColorSampleCounts;
		public UInt32 FramebufferDepthSampleCounts;
		public UInt32 FramebufferStencilSampleCounts;
		public UInt32 FramebufferNoAttachmentsSampleCounts;
		public UInt32 MaxColorAttachments;
		public UInt32 SampledImageColorSampleCounts;
		public UInt32 SampledImageIntegerSampleCounts;
		public UInt32 SampledImageDepthSampleCounts;
		public UInt32 SampledImageStencilSampleCounts;
		public UInt32 StorageImageSampleCounts;
		public UInt32 MaxSampleMaskWords;
		public Bool32 TimestampComputeAndGraphics;
		public float TimestampPeriod;
		public UInt32 MaxClipDistances;
		public UInt32 MaxCullDistances;
		public UInt32 MaxCombinedClipAndCullDistances;
		public UInt32 DiscreteQueuePriorities;
		public float PointSizeRange;
		public float LineWidthRange;
		public float PointSizeGranularity;
		public float LineWidthGranularity;
		public Bool32 StrictLines;
		public Bool32 StandardSampleLocations;
		public DeviceSize OptimalBufferCopyOffsetAlignment;
		public DeviceSize OptimalBufferCopyRowPitchAlignment;
		public DeviceSize NonCoherentAtomSize;
	}

	public struct SemaphoreCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
	}

	public struct QueryPoolCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public QueryType QueryType;
		public UInt32 QueryCount;
		public UInt32 PipelineStatistics;
	}

	public struct FramebufferCreateInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr RenderPass;
		public UInt32 AttachmentCount;
		public IntPtr Attachments;
		public UInt32 Width;
		public UInt32 Height;
		public UInt32 Layers;
	}

	public struct DrawIndirectCommand
	{
		public UInt32 VertexCount;
		public UInt32 InstanceCount;
		public UInt32 FirstVertex;
		public UInt32 FirstInstance;
	}

	public struct DrawIndexedIndirectCommand
	{
		public UInt32 IndexCount;
		public UInt32 InstanceCount;
		public UInt32 FirstIndex;
		public Int32 VertexOffset;
		public UInt32 FirstInstance;
	}

	public struct DispatchIndirectCommand
	{
		public UInt32 X;
		public UInt32 Y;
		public UInt32 Z;
	}

	public struct SubmitInfo
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 WaitSemaphoreCount;
		public IntPtr WaitSemaphores;
		public UInt32 WaitDstStageMask;
		public UInt32 CommandBufferCount;
		public IntPtr CommandBuffers;
		public UInt32 SignalSemaphoreCount;
		public IntPtr SignalSemaphores;
	}

	public struct DisplayPropertiesKhr
	{
		public IntPtr Display;
		public IntPtr DisplayName;
		public IntPtr PhysicalDimensions;
		public IntPtr PhysicalResolution;
		public SurfaceTransformFlagsKhr SupportedTransforms;
		public Bool32 PlaneReorderPossible;
		public Bool32 PersistentContent;
	}

	public struct DisplayPlanePropertiesKhr
	{
		public IntPtr CurrentDisplay;
		public UInt32 CurrentStackIndex;
	}

	public struct DisplayModeParametersKhr
	{
		public IntPtr VisibleRegion;
		public UInt32 RefreshRate;
	}

	public struct DisplayModePropertiesKhr
	{
		public IntPtr DisplayMode;
		public IntPtr Parameters;
	}

	public struct DisplayModeCreateInfoKhr
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr Parameters;
	}

	public struct DisplayPlaneCapabilitiesKhr
	{
		public DisplayPlaneAlphaFlagsKhr SupportedAlpha;
		public IntPtr MinSrcPosition;
		public IntPtr MaxSrcPosition;
		public IntPtr MinSrcExtent;
		public IntPtr MaxSrcExtent;
		public IntPtr MinDstPosition;
		public IntPtr MaxDstPosition;
		public IntPtr MinDstExtent;
		public IntPtr MaxDstExtent;
	}

	public struct DisplaySurfaceCreateInfoKhr
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr DisplayMode;
		public UInt32 PlaneIndex;
		public UInt32 PlaneStackIndex;
		public SurfaceTransformFlagsKhr Transform;
		public float GlobalAlpha;
		public DisplayPlaneAlphaFlagsKhr AlphaMode;
		public IntPtr ImageExtent;
	}

	public struct DisplayPresentInfoKhr
	{
		public StructureType SType;
		public IntPtr Next;
		public IntPtr SrcRect;
		public IntPtr DstRect;
		public Bool32 Persistent;
	}

	public struct SurfaceCapabilitiesKhr
	{
		public UInt32 MinImageCount;
		public UInt32 MaxImageCount;
		public IntPtr CurrentExtent;
		public IntPtr MinImageExtent;
		public IntPtr MaxImageExtent;
		public UInt32 MaxImageArrayLayers;
		public SurfaceTransformFlagsKhr SupportedTransforms;
		public SurfaceTransformFlagsKhr CurrentTransform;
		public CompositeAlphaFlagsKhr SupportedCompositeAlpha;
		public UInt32 SupportedUsageFlags;
	}

	public struct SurfaceFormatKhr
	{
		public Format Format;
		public ColorSpaceKhr ColorSpace;
	}

	public struct SwapchainCreateInfoKhr
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 Flags;
		public IntPtr Surface;
		public UInt32 MinImageCount;
		public Format ImageFormat;
		public ColorSpaceKhr ImageColorSpace;
		public IntPtr ImageExtent;
		public UInt32 ImageArrayLayers;
		public UInt32 ImageUsage;
		public SharingMode ImageSharingMode;
		public UInt32 QueueFamilyIndexCount;
		public UInt32 QueueFamilyIndices;
		public SurfaceTransformFlagsKhr PreTransform;
		public CompositeAlphaFlagsKhr CompositeAlpha;
		public PresentModeKhr PresentMode;
		public Bool32 Clipped;
		public IntPtr OldSwapchain;
	}

	public struct PresentInfoKhr
	{
		public StructureType SType;
		public IntPtr Next;
		public UInt32 WaitSemaphoreCount;
		public IntPtr WaitSemaphores;
		public UInt32 SwapchainCount;
		public IntPtr Swapchains;
		public UInt32 ImageIndices;
		public Result Results;
	}

	public struct DebugReportCallbackCreateInfoExt
	{
		public StructureType SType;
		public IntPtr Next;
		public DebugReportFlagsExt Flags;
		public IntPtr PfnCallback;
		public IntPtr UserData;
	}
}
