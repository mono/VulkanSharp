using System;

namespace Vulkan
{
	public enum AttachmentLoadOp : int
	{
		Load = 0,
		Clear = 1,
		DontCare = 2,
	}

	public enum AttachmentStoreOp : int
	{
		Store = 0,
		DontCare = 1,
	}

	public enum BlendFactor : int
	{
		Zero = 0,
		One = 1,
		SrcColor = 2,
		OneMinusSrcColor = 3,
		DstColor = 4,
		OneMinusDstColor = 5,
		SrcAlpha = 6,
		OneMinusSrcAlpha = 7,
		DstAlpha = 8,
		OneMinusDstAlpha = 9,
		ConstantColor = 10,
		OneMinusConstantColor = 11,
		ConstantAlpha = 12,
		OneMinusConstantAlpha = 13,
		SrcAlphaSaturate = 14,
		Src1Color = 15,
		OneMinusSrc1Color = 16,
		Src1Alpha = 17,
		OneMinusSrc1Alpha = 18,
	}

	public enum BlendOp : int
	{
		Add = 0,
		Subtract = 1,
		ReverseSubtract = 2,
		Min = 3,
		Max = 4,
	}

	public enum BorderColor : int
	{
		FloatTransparentBlack = 0,
		IntTransparentBlack = 1,
		FloatOpaqueBlack = 2,
		IntOpaqueBlack = 3,
		FloatOpaqueWhite = 4,
		IntOpaqueWhite = 5,
	}

	public enum PipelineCacheHeaderVersion : int
	{
		One = 1,
	}

	[Flags]
	public enum BufferCreateFlagBits : int
	{
		SparseBinding = 0x1,
		SparseResidency = 0x2,
		SparseAliased = 0x4,
	}

	[Flags]
	public enum BufferUsageFlagBits : int
	{
		TransferSrc = 0x1,
		TransferDst = 0x2,
		UniformTexelBuffer = 0x4,
		StorageTexelBuffer = 0x8,
		UniformBuffer = 0x10,
		StorageBuffer = 0x20,
		IndexBuffer = 0x40,
		VertexBuffer = 0x80,
		IndirectBuffer = 0x100,
	}

	[Flags]
	public enum ColorComponentFlagBits : int
	{
		R = 0x1,
		G = 0x2,
		B = 0x4,
		A = 0x8,
	}

	public enum ComponentSwizzle : int
	{
		Identity = 0,
		Zero = 1,
		One = 2,
		R = 3,
		G = 4,
		B = 5,
		A = 6,
	}

	[Flags]
	public enum CommandPoolCreateFlagBits : int
	{
		Transient = 0x1,
		ResetCommandBuffer = 0x2,
	}

	[Flags]
	public enum CommandPoolResetFlagBits : int
	{
		ReleaseResources = 0x1,
	}

	[Flags]
	public enum CommandBufferResetFlagBits : int
	{
		ReleaseResources = 0x1,
	}

	public enum CommandBufferLevel : int
	{
		Primary = 0,
		Secondary = 1,
	}

	[Flags]
	public enum CommandBufferUsageFlagBits : int
	{
		OneTimeSubmit = 0x1,
		RenderPassContinue = 0x2,
		SimultaneousUse = 0x4,
	}

	public enum CompareOp : int
	{
		Never = 0,
		Less = 1,
		Equal = 2,
		LessOrEqual = 3,
		Greater = 4,
		NotEqual = 5,
		GreaterOrEqual = 6,
		Always = 7,
	}

	[Flags]
	public enum CullModeFlagBits : int
	{
		None = 0,
		Front = 0x1,
		Back = 0x2,
		FrontAndBack = 0x3,
	}

	public enum DescriptorType : int
	{
		Sampler = 0,
		CombinedImageSampler = 1,
		SampledImage = 2,
		StorageImage = 3,
		UniformTexelBuffer = 4,
		StorageTexelBuffer = 5,
		UniformBuffer = 6,
		StorageBuffer = 7,
		UniformBufferDynamic = 8,
		StorageBufferDynamic = 9,
		InputAttachment = 10,
	}

	public enum DynamicState : int
	{
		Viewport = 0,
		Scissor = 1,
		LineWidth = 2,
		DepthBias = 3,
		BlendConstants = 4,
		DepthBounds = 5,
		StencilCompareMask = 6,
		StencilWriteMask = 7,
		StencilReference = 8,
	}

	[Flags]
	public enum FenceCreateFlagBits : int
	{
		Signaled = 0x1,
	}

	public enum PolygonMode : int
	{
		Fill = 0,
		Line = 1,
		Point = 2,
	}

	public enum Format : int
	{
		Undefined = 0,
		R4g4UnormPack8 = 1,
		R4g4b4a4UnormPack16 = 2,
		B4g4r4a4UnormPack16 = 3,
		R5g6b5UnormPack16 = 4,
		B5g6r5UnormPack16 = 5,
		R5g5b5a1UnormPack16 = 6,
		B5g5r5a1UnormPack16 = 7,
		A1r5g5b5UnormPack16 = 8,
		R8Unorm = 9,
		R8Snorm = 10,
		R8Uscaled = 11,
		R8Sscaled = 12,
		R8Uint = 13,
		R8Sint = 14,
		R8Srgb = 15,
		R8g8Unorm = 16,
		R8g8Snorm = 17,
		R8g8Uscaled = 18,
		R8g8Sscaled = 19,
		R8g8Uint = 20,
		R8g8Sint = 21,
		R8g8Srgb = 22,
		R8g8b8Unorm = 23,
		R8g8b8Snorm = 24,
		R8g8b8Uscaled = 25,
		R8g8b8Sscaled = 26,
		R8g8b8Uint = 27,
		R8g8b8Sint = 28,
		R8g8b8Srgb = 29,
		B8g8r8Unorm = 30,
		B8g8r8Snorm = 31,
		B8g8r8Uscaled = 32,
		B8g8r8Sscaled = 33,
		B8g8r8Uint = 34,
		B8g8r8Sint = 35,
		B8g8r8Srgb = 36,
		R8g8b8a8Unorm = 37,
		R8g8b8a8Snorm = 38,
		R8g8b8a8Uscaled = 39,
		R8g8b8a8Sscaled = 40,
		R8g8b8a8Uint = 41,
		R8g8b8a8Sint = 42,
		R8g8b8a8Srgb = 43,
		B8g8r8a8Unorm = 44,
		B8g8r8a8Snorm = 45,
		B8g8r8a8Uscaled = 46,
		B8g8r8a8Sscaled = 47,
		B8g8r8a8Uint = 48,
		B8g8r8a8Sint = 49,
		B8g8r8a8Srgb = 50,
		A8b8g8r8UnormPack32 = 51,
		A8b8g8r8SnormPack32 = 52,
		A8b8g8r8UscaledPack32 = 53,
		A8b8g8r8SscaledPack32 = 54,
		A8b8g8r8UintPack32 = 55,
		A8b8g8r8SintPack32 = 56,
		A8b8g8r8SrgbPack32 = 57,
		A2r10g10b10UnormPack32 = 58,
		A2r10g10b10SnormPack32 = 59,
		A2r10g10b10UscaledPack32 = 60,
		A2r10g10b10SscaledPack32 = 61,
		A2r10g10b10UintPack32 = 62,
		A2r10g10b10SintPack32 = 63,
		A2b10g10r10UnormPack32 = 64,
		A2b10g10r10SnormPack32 = 65,
		A2b10g10r10UscaledPack32 = 66,
		A2b10g10r10SscaledPack32 = 67,
		A2b10g10r10UintPack32 = 68,
		A2b10g10r10SintPack32 = 69,
		R16Unorm = 70,
		R16Snorm = 71,
		R16Uscaled = 72,
		R16Sscaled = 73,
		R16Uint = 74,
		R16Sint = 75,
		R16Sfloat = 76,
		R16g16Unorm = 77,
		R16g16Snorm = 78,
		R16g16Uscaled = 79,
		R16g16Sscaled = 80,
		R16g16Uint = 81,
		R16g16Sint = 82,
		R16g16Sfloat = 83,
		R16g16b16Unorm = 84,
		R16g16b16Snorm = 85,
		R16g16b16Uscaled = 86,
		R16g16b16Sscaled = 87,
		R16g16b16Uint = 88,
		R16g16b16Sint = 89,
		R16g16b16Sfloat = 90,
		R16g16b16a16Unorm = 91,
		R16g16b16a16Snorm = 92,
		R16g16b16a16Uscaled = 93,
		R16g16b16a16Sscaled = 94,
		R16g16b16a16Uint = 95,
		R16g16b16a16Sint = 96,
		R16g16b16a16Sfloat = 97,
		R32Uint = 98,
		R32Sint = 99,
		R32Sfloat = 100,
		R32g32Uint = 101,
		R32g32Sint = 102,
		R32g32Sfloat = 103,
		R32g32b32Uint = 104,
		R32g32b32Sint = 105,
		R32g32b32Sfloat = 106,
		R32g32b32a32Uint = 107,
		R32g32b32a32Sint = 108,
		R32g32b32a32Sfloat = 109,
		R64Uint = 110,
		R64Sint = 111,
		R64Sfloat = 112,
		R64g64Uint = 113,
		R64g64Sint = 114,
		R64g64Sfloat = 115,
		R64g64b64Uint = 116,
		R64g64b64Sint = 117,
		R64g64b64Sfloat = 118,
		R64g64b64a64Uint = 119,
		R64g64b64a64Sint = 120,
		R64g64b64a64Sfloat = 121,
		B10g11r11UfloatPack32 = 122,
		E5b9g9r9UfloatPack32 = 123,
		D16Unorm = 124,
		X8D24UnormPack32 = 125,
		D32Sfloat = 126,
		S8Uint = 127,
		D16UnormS8Uint = 128,
		D24UnormS8Uint = 129,
		D32SfloatS8Uint = 130,
		Bc1RgbUnormBlock = 131,
		Bc1RgbSrgbBlock = 132,
		Bc1RgbaUnormBlock = 133,
		Bc1RgbaSrgbBlock = 134,
		Bc2UnormBlock = 135,
		Bc2SrgbBlock = 136,
		Bc3UnormBlock = 137,
		Bc3SrgbBlock = 138,
		Bc4UnormBlock = 139,
		Bc4SnormBlock = 140,
		Bc5UnormBlock = 141,
		Bc5SnormBlock = 142,
		Bc6hUfloatBlock = 143,
		Bc6hSfloatBlock = 144,
		Bc7UnormBlock = 145,
		Bc7SrgbBlock = 146,
		Etc2R8g8b8UnormBlock = 147,
		Etc2R8g8b8SrgbBlock = 148,
		Etc2R8g8b8a1UnormBlock = 149,
		Etc2R8g8b8a1SrgbBlock = 150,
		Etc2R8g8b8a8UnormBlock = 151,
		Etc2R8g8b8a8SrgbBlock = 152,
		EacR11UnormBlock = 153,
		EacR11SnormBlock = 154,
		EacR11g11UnormBlock = 155,
		EacR11g11SnormBlock = 156,
		Astc4x4UnormBlock = 157,
		Astc4x4SrgbBlock = 158,
		Astc5x4UnormBlock = 159,
		Astc5x4SrgbBlock = 160,
		Astc5x5UnormBlock = 161,
		Astc5x5SrgbBlock = 162,
		Astc6x5UnormBlock = 163,
		Astc6x5SrgbBlock = 164,
		Astc6x6UnormBlock = 165,
		Astc6x6SrgbBlock = 166,
		Astc8x5UnormBlock = 167,
		Astc8x5SrgbBlock = 168,
		Astc8x6UnormBlock = 169,
		Astc8x6SrgbBlock = 170,
		Astc8x8UnormBlock = 171,
		Astc8x8SrgbBlock = 172,
		Astc10x5UnormBlock = 173,
		Astc10x5SrgbBlock = 174,
		Astc10x6UnormBlock = 175,
		Astc10x6SrgbBlock = 176,
		Astc10x8UnormBlock = 177,
		Astc10x8SrgbBlock = 178,
		Astc10x10UnormBlock = 179,
		Astc10x10SrgbBlock = 180,
		Astc12x10UnormBlock = 181,
		Astc12x10SrgbBlock = 182,
		Astc12x12UnormBlock = 183,
		Astc12x12SrgbBlock = 184,
	}

	[Flags]
	public enum FormatFeatureFlagBits : int
	{
		SampledImage = 0x1,
		StorageImage = 0x2,
		StorageImageAtomic = 0x4,
		UniformTexelBuffer = 0x8,
		StorageTexelBuffer = 0x10,
		StorageTexelBufferAtomic = 0x20,
		VertexBuffer = 0x40,
		ColorAttachment = 0x80,
		ColorAttachmentBlend = 0x100,
		DepthStencilAttachment = 0x200,
		BlitSrc = 0x400,
		BlitDst = 0x800,
		SampledImageFilterLinear = 0x1000,
	}

	public enum FrontFace : int
	{
		CounterClockwise = 0,
		Clockwise = 1,
	}

	[Flags]
	public enum ImageAspectFlagBits : int
	{
		Color = 0x1,
		Depth = 0x2,
		Stencil = 0x4,
		Metadata = 0x8,
	}

	[Flags]
	public enum ImageCreateFlagBits : int
	{
		SparseBinding = 0x1,
		SparseResidency = 0x2,
		SparseAliased = 0x4,
		MutableFormat = 0x8,
		CubeCompatible = 0x10,
	}

	public enum ImageLayout : int
	{
		Undefined = 0,
		General = 1,
		ColorAttachmentOptimal = 2,
		DepthStencilAttachmentOptimal = 3,
		DepthStencilReadOnlyOptimal = 4,
		ShaderReadOnlyOptimal = 5,
		TransferSrcOptimal = 6,
		TransferDstOptimal = 7,
		Preinitialized = 8,
	}

	public enum ImageTiling : int
	{
		Optimal = 0,
		Linear = 1,
	}

	public enum ImageType : int
	{
		Image1D = 0,
		Image2D = 1,
		Image3D = 2,
	}

	[Flags]
	public enum ImageUsageFlagBits : int
	{
		TransferSrc = 0x1,
		TransferDst = 0x2,
		Sampled = 0x4,
		Storage = 0x8,
		ColorAttachment = 0x10,
		DepthStencilAttachment = 0x20,
		TransientAttachment = 0x40,
		InputAttachment = 0x80,
	}

	public enum ImageViewType : int
	{
		View1D = 0,
		View2D = 1,
		View3D = 2,
		Cube = 3,
		View1DArray = 4,
		View2DArray = 5,
		CubeArray = 6,
	}

	public enum SharingMode : int
	{
		Exclusive = 0,
		Concurrent = 1,
	}

	public enum IndexType : int
	{
		Uint16 = 0,
		Uint32 = 1,
	}

	public enum LogicOp : int
	{
		Clear = 0,
		And = 1,
		AndReverse = 2,
		Copy = 3,
		AndInverted = 4,
		NoOp = 5,
		Xor = 6,
		Or = 7,
		Nor = 8,
		Equivalent = 9,
		Invert = 10,
		OrReverse = 11,
		CopyInverted = 12,
		OrInverted = 13,
		Nand = 14,
		Set = 15,
	}

	[Flags]
	public enum MemoryHeapFlagBits : int
	{
		DeviceLocal = 0x1,
	}

	[Flags]
	public enum AccessFlagBits : int
	{
		IndirectCommandRead = 0x1,
		IndexRead = 0x2,
		VertexAttributeRead = 0x4,
		UniformRead = 0x8,
		InputAttachmentRead = 0x10,
		ShaderRead = 0x20,
		ShaderWrite = 0x40,
		ColorAttachmentRead = 0x80,
		ColorAttachmentWrite = 0x100,
		DepthStencilAttachmentRead = 0x200,
		DepthStencilAttachmentWrite = 0x400,
		TransferRead = 0x800,
		TransferWrite = 0x1000,
		HostRead = 0x2000,
		HostWrite = 0x4000,
		MemoryRead = 0x8000,
		MemoryWrite = 0x10000,
	}

	[Flags]
	public enum MemoryPropertyFlagBits : int
	{
		DeviceLocal = 0x1,
		HostVisible = 0x2,
		HostCoherent = 0x4,
		HostCached = 0x8,
		LazilyAllocated = 0x10,
	}

	public enum PhysicalDeviceType : int
	{
		Other = 0,
		IntegratedGpu = 1,
		DiscreteGpu = 2,
		VirtualGpu = 3,
		Cpu = 4,
	}

	public enum PipelineBindPoint : int
	{
		Graphics = 0,
		Compute = 1,
	}

	[Flags]
	public enum PipelineCreateFlagBits : int
	{
		DisableOptimization = 0x1,
		AllowDerivatives = 0x2,
		Derivative = 0x4,
	}

	public enum PrimitiveTopology : int
	{
		PointList = 0,
		LineList = 1,
		LineStrip = 2,
		TriangleList = 3,
		TriangleStrip = 4,
		TriangleFan = 5,
		LineListWithAdjacency = 6,
		LineStripWithAdjacency = 7,
		TriangleListWithAdjacency = 8,
		TriangleStripWithAdjacency = 9,
		PatchList = 10,
	}

	[Flags]
	public enum QueryControlFlagBits : int
	{
		Precise = 0x1,
	}

	[Flags]
	public enum QueryPipelineStatisticFlagBits : int
	{
		InputAssemblyVertices = 0x1,
		InputAssemblyPrimitives = 0x2,
		VertexShaderInvocations = 0x4,
		GeometryShaderInvocations = 0x8,
		GeometryShaderPrimitives = 0x10,
		ClippingInvocations = 0x20,
		ClippingPrimitives = 0x40,
		FragmentShaderInvocations = 0x80,
		TessellationControlShaderPatches = 0x100,
		TessellationEvaluationShaderInvocations = 0x200,
		ComputeShaderInvocations = 0x400,
	}

	[Flags]
	public enum QueryResultFlagBits : int
	{
		Result64 = 0x1,
		Wait = 0x2,
		WithAvailability = 0x4,
		Partial = 0x8,
	}

	public enum QueryType : int
	{
		Occlusion = 0,
		PipelineStatistics = 1,
		Timestamp = 2,
	}

	[Flags]
	public enum QueueFlagBits : int
	{
		Graphics = 0x1,
		Compute = 0x2,
		Transfer = 0x4,
		SparseBinding = 0x8,
	}

	public enum SubpassContents : int
	{
		Inline = 0,
		SecondaryCommandBuffers = 1,
	}

	public enum Result : int
	{
		Success = 0,
		NotReady = 1,
		Timeout = 2,
		EventSet = 3,
		EventReset = 4,
		Incomplete = 5,
		ErrorOutOfHostMemory = -1,
		ErrorOutOfDeviceMemory = -2,
		ErrorInitializationFailed = -3,
		ErrorDeviceLost = -4,
		ErrorMemoryMapFailed = -5,
		ErrorLayerNotPresent = -6,
		ErrorExtensionNotPresent = -7,
		ErrorFeatureNotPresent = -8,
		ErrorIncompatibleDriver = -9,
		ErrorTooManyObjects = -10,
		ErrorFormatNotSupported = -11,
	}

	[Flags]
	public enum ShaderStageFlagBits : int
	{
		Vertex = 0x1,
		TessellationControl = 0x2,
		TessellationEvaluation = 0x4,
		Geometry = 0x8,
		Fragment = 0x10,
		Compute = 0x20,
		AllGraphics = 0x1F,
		All = 0x7FFFFFFF,
	}

	[Flags]
	public enum SparseMemoryBindFlagBits : int
	{
		Metadata = 0x1,
	}

	[Flags]
	public enum StencilFaceFlagBits : int
	{
		Front = 0x1,
		Back = 0x2,
		StencilFrontAndBack = 0x3,
	}

	public enum StencilOp : int
	{
		Keep = 0,
		Zero = 1,
		Replace = 2,
		IncrementAndClamp = 3,
		DecrementAndClamp = 4,
		Invert = 5,
		IncrementAndWrap = 6,
		DecrementAndWrap = 7,
	}

	public enum StructureType : int
	{
		ApplicationInfo = 0,
		InstanceCreateInfo = 1,
		DeviceQueueCreateInfo = 2,
		DeviceCreateInfo = 3,
		SubmitInfo = 4,
		MemoryAllocateInfo = 5,
		MappedMemoryRange = 6,
		BindSparseInfo = 7,
		FenceCreateInfo = 8,
		SemaphoreCreateInfo = 9,
		EventCreateInfo = 10,
		QueryPoolCreateInfo = 11,
		BufferCreateInfo = 12,
		BufferViewCreateInfo = 13,
		ImageCreateInfo = 14,
		ImageViewCreateInfo = 15,
		ShaderModuleCreateInfo = 16,
		PipelineCacheCreateInfo = 17,
		PipelineShaderStageCreateInfo = 18,
		PipelineVertexInputStateCreateInfo = 19,
		PipelineInputAssemblyStateCreateInfo = 20,
		PipelineTessellationStateCreateInfo = 21,
		PipelineViewportStateCreateInfo = 22,
		PipelineRasterizationStateCreateInfo = 23,
		PipelineMultisampleStateCreateInfo = 24,
		PipelineDepthStencilStateCreateInfo = 25,
		PipelineColorBlendStateCreateInfo = 26,
		PipelineDynamicStateCreateInfo = 27,
		GraphicsPipelineCreateInfo = 28,
		ComputePipelineCreateInfo = 29,
		PipelineLayoutCreateInfo = 30,
		SamplerCreateInfo = 31,
		DescriptorSetLayoutCreateInfo = 32,
		DescriptorPoolCreateInfo = 33,
		DescriptorSetAllocateInfo = 34,
		WriteDescriptorSet = 35,
		CopyDescriptorSet = 36,
		FramebufferCreateInfo = 37,
		RenderPassCreateInfo = 38,
		CommandPoolCreateInfo = 39,
		CommandBufferAllocateInfo = 40,
		CommandBufferInheritanceInfo = 41,
		CommandBufferBeginInfo = 42,
		RenderPassBeginInfo = 43,
		BufferMemoryBarrier = 44,
		ImageMemoryBarrier = 45,
		MemoryBarrier = 46,
		LoaderInstanceCreateInfo = 47,
		LoaderDeviceCreateInfo = 48,
	}

	public enum SystemAllocationScope : int
	{
		Command = 0,
		Object = 1,
		Cache = 2,
		Device = 3,
		Instance = 4,
	}

	public enum InternalAllocationType : int
	{
		Executable = 0,
	}

	public enum SamplerAddressMode : int
	{
		Repeat = 0,
		MirroredRepeat = 1,
		ClampToEdge = 2,
		ClampToBorder = 3,
		MirrorClampToEdge = 4,
	}

	public enum Filter : int
	{
		Nearest = 0,
		Linear = 1,
	}

	public enum SamplerMipmapMode : int
	{
		Nearest = 0,
		Linear = 1,
	}

	public enum VertexInputRate : int
	{
		Vertex = 0,
		Instance = 1,
	}

	[Flags]
	public enum PipelineStageFlagBits : int
	{
		TopOfPipe = 0x1,
		DrawIndirect = 0x2,
		VertexInput = 0x4,
		VertexShader = 0x8,
		TessellationControlShader = 0x10,
		TessellationEvaluationShader = 0x20,
		GeometryShader = 0x40,
		FragmentShader = 0x80,
		EarlyFragmentTests = 0x100,
		LateFragmentTests = 0x200,
		ColorAttachmentOutput = 0x400,
		ComputeShader = 0x800,
		Transfer = 0x1000,
		BottomOfPipe = 0x2000,
		Host = 0x4000,
		AllGraphics = 0x8000,
		AllCommands = 0x10000,
	}

	[Flags]
	public enum SparseImageFormatFlagBits : int
	{
		SingleMiptail = 0x1,
		AlignedMipSize = 0x2,
		NonstandardBlockSize = 0x4,
	}

	[Flags]
	public enum SampleCountFlagBits : int
	{
		Count1 = 0x1,
		Count2 = 0x2,
		Count4 = 0x4,
		Count8 = 0x8,
		Count16 = 0x10,
		Count32 = 0x20,
		Count64 = 0x40,
	}

	[Flags]
	public enum AttachmentDescriptionFlagBits : int
	{
		MayAlias = 0x1,
	}

	[Flags]
	public enum DescriptorPoolCreateFlagBits : int
	{
		FreeDescriptorSet = 0x1,
	}

	[Flags]
	public enum DependencyFlagBits : int
	{
		ByRegion = 0x1,
	}

	public enum ColorSpaceKHR : int
	{
		ColorspaceSrgbNonlinearKhr = 0,
	}

	[Flags]
	public enum CompositeAlphaFlagBitsKHR : int
	{
		CompositeAlphaOpaqueBitKhr = 0x1,
		CompositeAlphaPreMultipliedBitKhr = 0x2,
		CompositeAlphaPostMultipliedBitKhr = 0x4,
		CompositeAlphaInheritBitKhr = 0x8,
	}

	[Flags]
	public enum DisplayPlaneAlphaFlagBitsKHR : int
	{
		DisplayPlaneAlphaOpaqueBitKhr = 0x1,
		DisplayPlaneAlphaGlobalBitKhr = 0x2,
		DisplayPlaneAlphaPerPixelBitKhr = 0x4,
		DisplayPlaneAlphaPerPixelPremultipliedBitKhr = 0x8,
	}

	public enum PresentModeKHR : int
	{
		PresentModeImmediateKhr = 0,
		PresentModeMailboxKhr = 1,
		PresentModeFifoKhr = 2,
		PresentModeFifoRelaxedKhr = 3,
	}

	[Flags]
	public enum SurfaceTransformFlagBitsKHR : int
	{
		SurfaceTransformIdentityBitKhr = 0x1,
		SurfaceTransformRotate90BitKhr = 0x2,
		SurfaceTransformRotate180BitKhr = 0x4,
		SurfaceTransformRotate270BitKhr = 0x8,
		SurfaceTransformHorizontalMirrorBitKhr = 0x10,
		SurfaceTransformHorizontalMirrorRotate90BitKhr = 0x20,
		SurfaceTransformHorizontalMirrorRotate180BitKhr = 0x40,
		SurfaceTransformHorizontalMirrorRotate270BitKhr = 0x80,
		SurfaceTransformInheritBitKhr = 0x100,
	}

	[Flags]
	public enum DebugReportFlagBitsEXT : int
	{
		DebugReportInformationBitExt = 0x1,
		DebugReportWarningBitExt = 0x2,
		DebugReportPerformanceWarningBitExt = 0x4,
		DebugReportErrorBitExt = 0x8,
		DebugReportDebugBitExt = 0x10,
	}

	public enum DebugReportObjectTypeEXT : int
	{
		DebugReportObjectTypeUnknownExt = 0,
		DebugReportObjectTypeInstanceExt = 1,
		DebugReportObjectTypePhysicalDeviceExt = 2,
		DebugReportObjectTypeDeviceExt = 3,
		DebugReportObjectTypeQueueExt = 4,
		DebugReportObjectTypeSemaphoreExt = 5,
		DebugReportObjectTypeCommandBufferExt = 6,
		DebugReportObjectTypeFenceExt = 7,
		DebugReportObjectTypeDeviceMemoryExt = 8,
		DebugReportObjectTypeBufferExt = 9,
		DebugReportObjectTypeImageExt = 10,
		DebugReportObjectTypeEventExt = 11,
		DebugReportObjectTypeQueryPoolExt = 12,
		DebugReportObjectTypeBufferViewExt = 13,
		DebugReportObjectTypeImageViewExt = 14,
		DebugReportObjectTypeShaderModuleExt = 15,
		DebugReportObjectTypePipelineCacheExt = 16,
		DebugReportObjectTypePipelineLayoutExt = 17,
		DebugReportObjectTypeRenderPassExt = 18,
		DebugReportObjectTypePipelineExt = 19,
		DebugReportObjectTypeDescriptorSetLayoutExt = 20,
		DebugReportObjectTypeSamplerExt = 21,
		DebugReportObjectTypeDescriptorPoolExt = 22,
		DebugReportObjectTypeDescriptorSetExt = 23,
		DebugReportObjectTypeFramebufferExt = 24,
		DebugReportObjectTypeCommandPoolExt = 25,
		DebugReportObjectTypeSurfaceKhrExt = 26,
		DebugReportObjectTypeSwapchainKhrExt = 27,
		DebugReportObjectTypeDebugReportExt = 28,
	}

	public enum DebugReportErrorEXT : int
	{
		DebugReportErrorNoneExt = 0,
		DebugReportErrorCallbackRefExt = 1,
	}
}
