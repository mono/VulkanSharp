using System.Collections.Generic;
using Vulkan;
using Vulkan.Android;

namespace XLogo
{
	public class XLogoView : VulkanView
	{
		Device device;
		Queue queue;
		SwapchainKhr swapchain;
		Semaphore semaphore;
		Fence fence;
		CommandBuffer [] commandBuffers;
		DescriptorSet [] descriptorSets;
		SurfaceCapabilitiesKhr surfaceCapabilities;
		RenderPass renderPass;
		DescriptorSetLayout descriptorSetLayout;
		PipelineLayout pipelineLayout;
		Buffer uniformBuffer;
		bool initialized;

		public XLogoView (Android.Content.Context context) : base (context, CreateInstance ())
		{
		}

		static Instance CreateInstance ()
		{
			var instanceExtensions = new List<string> {
				"VK_KHR_surface",
				"VK_KHR_android_surface",
				"VK_KHR_display"
			};

			return new Instance (new InstanceCreateInfo {
				ApplicationInfo = new ApplicationInfo {
					ApplicationName = "Vulkan Android XLogo",
					ApiVersion = Version.Make (1, 0, 0)
				},
				EnabledExtensionNames = instanceExtensions.ToArray ()
			});
		}

		protected override void NativeWindowAcquired ()
		{
			InitializeVulkan ();
		}

		SurfaceFormatKhr SelectFormat (PhysicalDevice physicalDevice, SurfaceKhr surface)
		{
			foreach (var f in physicalDevice.GetSurfaceFormatsKHR (surface))
				if (f.Format == Format.R8g8b8a8Unorm)
					return f;

			throw new System.Exception ("didn't find the R8g8b8a8Unorm format");
		}

		SwapchainKhr CreateSwapchain (SurfaceKhr surface, SurfaceFormatKhr surfaceFormat)
		{
			var swapchainInfo = new SwapchainCreateInfoKhr {
				Surface = surface,
				MinImageCount = surfaceCapabilities.MinImageCount,
				ImageFormat = surfaceFormat.Format,
				ImageColorSpace = surfaceFormat.ColorSpace,
				ImageExtent = surfaceCapabilities.CurrentExtent,
				ImageUsage = ImageUsageFlags.ColorAttachment,
				PreTransform = SurfaceTransformFlagsKhr.Identity,
				ImageArrayLayers = 1,
				ImageSharingMode = SharingMode.Exclusive,
				QueueFamilyIndices = new uint [] { 0 },
				PresentMode = PresentModeKhr.Fifo,
				CompositeAlpha = CompositeAlphaFlagsKhr.Inherit
			};

			return device.CreateSwapchainKHR (swapchainInfo);
		}

		Framebuffer [] CreateFramebuffers (Image [] images, SurfaceFormatKhr surfaceFormat)
		{
			var displayViews = new ImageView [images.Length];

			for (int i = 0; i < images.Length; i++) {
				var viewCreateInfo = new ImageViewCreateInfo {
					Image = images [i],
					ViewType = ImageViewType.View2D,
					Format = surfaceFormat.Format,
					Components = new ComponentMapping {
						R = ComponentSwizzle.R,
						G = ComponentSwizzle.G,
						B = ComponentSwizzle.B,
						A = ComponentSwizzle.A
					},
					SubresourceRange = new ImageSubresourceRange {
						AspectMask = ImageAspectFlags.Color,
						LevelCount = 1,
						LayerCount = 1
					}
				};
				displayViews [i] = device.CreateImageView (viewCreateInfo);
			}
			var framebuffers = new Framebuffer [images.Length];

			for (int i = 0; i < images.Length; i++) {
				var frameBufferCreateInfo = new FramebufferCreateInfo {
					Layers = 1,
					RenderPass = renderPass,
					Attachments = new ImageView [] { displayViews [i] },
					Width = surfaceCapabilities.CurrentExtent.Width,
					Height = surfaceCapabilities.CurrentExtent.Height
				};
				framebuffers [i] = device.CreateFramebuffer (frameBufferCreateInfo);
			}

			return framebuffers;
		}

		CommandBuffer [] CreateCommandBuffers (Image [] images, Framebuffer [] framebuffers, Pipeline pipeline, Buffer vertexBuffer, Buffer indexBuffer, uint indexLength)
		{
			var createPoolInfo = new CommandPoolCreateInfo { Flags = CommandPoolCreateFlags.ResetCommandBuffer };
			var commandPool = device.CreateCommandPool (createPoolInfo);
			var commandBufferAllocateInfo = new CommandBufferAllocateInfo {
				Level = CommandBufferLevel.Primary,
				CommandPool = commandPool,
				CommandBufferCount = (uint)images.Length
			};
			var buffers = device.AllocateCommandBuffers (commandBufferAllocateInfo);
			var commandBufferBeginInfo = new CommandBufferBeginInfo ();

			for (int i = 0; i < images.Length; i++) {
				buffers [i].Begin (commandBufferBeginInfo);
				var renderPassBeginInfo = new RenderPassBeginInfo {
					Framebuffer = framebuffers [i],
					RenderPass = renderPass,
					ClearValues = new ClearValue [] { new ClearValue { Color = new ClearColorValue (new float [] { 0.9f, 0.87f, 0.75f, 1.0f }) } },
					RenderArea = new Rect2D { Extent = surfaceCapabilities.CurrentExtent }
				};
				buffers [i].CmdBeginRenderPass (renderPassBeginInfo, SubpassContents.Inline);
				buffers [i].CmdBindDescriptorSets (PipelineBindPoint.Graphics, pipelineLayout, 0, 1, descriptorSets [0], 0, 0);
				buffers [i].CmdBindPipeline (PipelineBindPoint.Graphics, pipeline);
				buffers [i].CmdBindVertexBuffers (0, 1, vertexBuffer, 0);
				buffers [i].CmdBindIndexBuffer (indexBuffer, 0, IndexType.Uint16);
				buffers [i].CmdDrawIndexed (indexLength, 1, 0, 0, 0);
				buffers [i].CmdEndRenderPass ();
				buffers [i].End ();
			}

			return buffers;
		}

		RenderPass CreateRenderPass (SurfaceFormatKhr surfaceFormat)
		{
			var attDesc = new AttachmentDescription {
				Format = surfaceFormat.Format,
				Samples = SampleCountFlags.Count1,
				LoadOp = AttachmentLoadOp.Clear,
				StoreOp = AttachmentStoreOp.Store,
				StencilLoadOp = AttachmentLoadOp.DontCare,
				StencilStoreOp = AttachmentStoreOp.DontCare,
				InitialLayout = ImageLayout.ColorAttachmentOptimal,
				FinalLayout = ImageLayout.ColorAttachmentOptimal
			};
			var attRef = new AttachmentReference { Layout = ImageLayout.ColorAttachmentOptimal };
			var subpassDesc = new SubpassDescription {
				PipelineBindPoint = PipelineBindPoint.Graphics,
				ColorAttachments = new AttachmentReference [] { attRef }
			};
			var renderPassCreateInfo = new RenderPassCreateInfo {
				Attachments = new AttachmentDescription [] { attDesc },
				Subpasses = new SubpassDescription [] { subpassDesc }
			};

			return device.CreateRenderPass (renderPassCreateInfo);
		}

		byte [] LoadResource (string name)
		{
			System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream (name);
			byte [] bytes = new byte [stream.Length];
			stream.Read (bytes, 0, (int)stream.Length);

			return bytes;
		}

		Buffer CreateBuffer (PhysicalDevice physicalDevice, object values, BufferUsageFlags usageFlags, System.Type type)
		{
			var array = values as System.Array;
			var length = (array != null) ? array.Length : 1;
			var size = System.Runtime.InteropServices.Marshal.SizeOf (type) * length;
			var createBufferInfo = new BufferCreateInfo {
				Size = size,
				Usage = usageFlags,
				SharingMode = SharingMode.Exclusive,
				QueueFamilyIndices = new uint [] { 0 }
			};
			var buffer = device.CreateBuffer (createBufferInfo);
			var memoryReq = device.GetBufferMemoryRequirements (buffer);
			var allocInfo = new MemoryAllocateInfo { AllocationSize = memoryReq.Size };
			var memoryProperties = physicalDevice.GetMemoryProperties ();
			bool heapIndexSet = false;
			var memoryTypes = memoryProperties.MemoryTypes;

			for (uint i = 0; i < memoryProperties.MemoryTypeCount; i++) {
				if (((memoryReq.MemoryTypeBits >> (int)i) & 1) == 1 &&
				    (memoryTypes [i].PropertyFlags & MemoryPropertyFlags.HostVisible) == MemoryPropertyFlags.HostVisible) {
					allocInfo.MemoryTypeIndex = i;
					heapIndexSet = true;
				}
			}

			if (!heapIndexSet)
				allocInfo.MemoryTypeIndex = memoryProperties.MemoryTypes [0].HeapIndex;

			var deviceMemory = device.AllocateMemory (allocInfo);
			var memPtr = device.MapMemory (deviceMemory, 0, size, 0);

			if (type == typeof (float))
				System.Runtime.InteropServices.Marshal.Copy (values as float [], 0, memPtr, length);
			else if (type == typeof (short))
				System.Runtime.InteropServices.Marshal.Copy (values as short [], 0, memPtr, length);
			else if (type == typeof (AreaUniformBuffer))
				System.Runtime.InteropServices.Marshal.StructureToPtr (values, memPtr, false);

			device.UnmapMemory (deviceMemory);
			device.BindBufferMemory (buffer, deviceMemory, 0);

			return buffer;
		}

		struct AreaUniformBuffer
		{
			public float width;
			public float height;
		};

		DescriptorSetLayout CreateDescriptorSetLayout ()
		{
			var layoutBinding = new DescriptorSetLayoutBinding {
				DescriptorType = DescriptorType.UniformBuffer,
				DescriptorCount = 1,
				StageFlags = ShaderStageFlags.Vertex
			};
			var descriptorSetLayoutCreateInfo = new DescriptorSetLayoutCreateInfo {
				Bindings = new DescriptorSetLayoutBinding [] { layoutBinding }
			};

			return device.CreateDescriptorSetLayout (descriptorSetLayoutCreateInfo);
		}

		Pipeline[] CreatePipelines ()
		{
			var pipelineLayoutCreateInfo = new PipelineLayoutCreateInfo {
				SetLayouts = new DescriptorSetLayout [] { descriptorSetLayout }
			};
			pipelineLayout = device.CreatePipelineLayout (pipelineLayoutCreateInfo);
			var vertexShaderModule = device.CreateShaderModule (LoadResource ("XLogo.Shaders.shader.vert.spv"));
			var fragmentShaderModule = device.CreateShaderModule (LoadResource ("XLogo.Shaders.shader.frag.spv"));
			PipelineShaderStageCreateInfo [] pipelineShaderStages = {
				new PipelineShaderStageCreateInfo {
					Stage = ShaderStageFlags.Vertex,
					Module = vertexShaderModule,
					Name = "main"
				},
				new PipelineShaderStageCreateInfo {
					Stage = ShaderStageFlags.Fragment,
					Module = fragmentShaderModule,
					Name = "main"
				}
			};
			var viewport = new Viewport {
				MinDepth = 0,
				MaxDepth = 1.0f,
				Width = surfaceCapabilities.CurrentExtent.Width,
				Height = surfaceCapabilities.CurrentExtent.Height
			};
			var scissor = new Rect2D { Extent = surfaceCapabilities.CurrentExtent };
			var viewportCreateInfo = new PipelineViewportStateCreateInfo {
				Viewports = new Viewport [] { viewport },
				Scissors = new Rect2D [] { scissor }
			};

			var multisampleCreateInfo = new PipelineMultisampleStateCreateInfo {
				RasterizationSamples = SampleCountFlags.Count1,
				SampleMask = new uint [] { ~0u }
			};
			var colorBlendAttachmentState = new PipelineColorBlendAttachmentState {
				ColorWriteMask = ColorComponentFlags.R | ColorComponentFlags.G | ColorComponentFlags.B | ColorComponentFlags.A
			};
			var colorBlendStateCreatInfo = new PipelineColorBlendStateCreateInfo {
				LogicOp = LogicOp.Copy,
				Attachments = new PipelineColorBlendAttachmentState [] { colorBlendAttachmentState }
			};
			var rasterizationStateCreateInfo = new PipelineRasterizationStateCreateInfo {
				PolygonMode = PolygonMode.Fill,
				CullMode = (uint)CullModeFlags.None,
				FrontFace = FrontFace.Clockwise,
				LineWidth = 1.0f
			};
			var inputAssemblyStateCreateInfo = new PipelineInputAssemblyStateCreateInfo {
				Topology = PrimitiveTopology.TriangleList
			};
			var vertexInputBindingDescription = new VertexInputBindingDescription {
				Stride = 3 * sizeof (float),
				InputRate = VertexInputRate.Vertex
			};
			var vertexInputAttributeDescription = new VertexInputAttributeDescription {
				Format = Format.R32g32b32Sfloat
			};
			var vertexInputStateCreateInfo = new PipelineVertexInputStateCreateInfo {
				VertexBindingDescriptions = new VertexInputBindingDescription [] { vertexInputBindingDescription },
				VertexAttributeDescriptions = new VertexInputAttributeDescription [] { vertexInputAttributeDescription }
			};

			var pipelineCreateInfo = new GraphicsPipelineCreateInfo {
				Layout = pipelineLayout,
				ViewportState = viewportCreateInfo,
				Stages = pipelineShaderStages,
				MultisampleState = multisampleCreateInfo,
				ColorBlendState = colorBlendStateCreatInfo,
				RasterizationState = rasterizationStateCreateInfo,
				InputAssemblyState = inputAssemblyStateCreateInfo,
				VertexInputState = vertexInputStateCreateInfo,
				DynamicState = new PipelineDynamicStateCreateInfo (),
				RenderPass = renderPass
			};

			return device.CreateGraphicsPipelines (device.CreatePipelineCache (new PipelineCacheCreateInfo ()), 1, pipelineCreateInfo);
		}

		Buffer CreateUniformBuffer (PhysicalDevice physicalDevice)
		{
			var uniformBufferData = new AreaUniformBuffer {
				width = surfaceCapabilities.CurrentExtent.Width,
				height = surfaceCapabilities.CurrentExtent.Height
			};

			return CreateBuffer (physicalDevice, uniformBufferData, BufferUsageFlags.UniformBuffer, typeof (AreaUniformBuffer));
		}

		DescriptorSet[] CreateDescriptorSets ()
		{
			var typeCount = new DescriptorPoolSize {
				Type = DescriptorType.UniformBuffer,
				DescriptorCount = 1
			};
			var descriptorPoolCreateInfo = new DescriptorPoolCreateInfo {
				PoolSizes = new DescriptorPoolSize [] { typeCount },
				MaxSets = 1
			};
			var descriptorPool = device.CreateDescriptorPool (descriptorPoolCreateInfo);

			var descriptorSetAllocateInfo = new DescriptorSetAllocateInfo {
				SetLayouts = new DescriptorSetLayout [] { descriptorSetLayout },
				DescriptorPool = descriptorPool
			};

			return device.AllocateDescriptorSets (descriptorSetAllocateInfo);
		}

		void UpdateDescriptorSets ()
		{
			var uniformBufferInfo = new DescriptorBufferInfo {
				Buffer = uniformBuffer,
				Offset = 0,
				Range = 2 * sizeof (float)
			};
			var writeDescriptorSet = new WriteDescriptorSet {
				DstSet = descriptorSets [0],
				DescriptorType = DescriptorType.UniformBuffer,
				BufferInfo = new DescriptorBufferInfo [] { uniformBufferInfo }
			};

			device.UpdateDescriptorSets (1, writeDescriptorSet, 0, new CopyDescriptorSet ());
		}

		public void InitializeVulkan ()
		{
			var devices = Instance.EnumeratePhysicalDevices ();
			var surface = Instance.CreateAndroidSurfaceKHR (new AndroidSurfaceCreateInfoKhr { Window = aNativeWindow });
			var queueInfo = new DeviceQueueCreateInfo { QueuePriorities = new float [] { 1.0f } };
			var deviceInfo = new DeviceCreateInfo {
				EnabledExtensionNames = new string [] { "VK_KHR_swapchain", "VK_KHR_display_swapchain" },
				QueueCreateInfos = new DeviceQueueCreateInfo [] { queueInfo }
			};
			var physicalDevice = devices [0];
			device = physicalDevice.CreateDevice (deviceInfo);
			queue = device.GetQueue (0, 0);
			surfaceCapabilities = physicalDevice.GetSurfaceCapabilitiesKHR (surface);
			var surfaceFormat = SelectFormat (physicalDevice, surface);
			swapchain = CreateSwapchain (surface, surfaceFormat);
			var images = device.GetSwapchainImagesKHR (swapchain);
			renderPass = CreateRenderPass (surfaceFormat);

			var framebuffers = CreateFramebuffers (images, surfaceFormat);
			var vertexBuffer = CreateBuffer (physicalDevice, Logo.Vertices, BufferUsageFlags.VertexBuffer, typeof (float));
			var indexBuffer = CreateBuffer (physicalDevice, Logo.Indexes, BufferUsageFlags.IndexBuffer, typeof (short));
			uniformBuffer = CreateUniformBuffer (physicalDevice);
			descriptorSetLayout = CreateDescriptorSetLayout ();
			var pipelines = CreatePipelines ();
			descriptorSets = CreateDescriptorSets ();
			UpdateDescriptorSets ();

			commandBuffers = CreateCommandBuffers (images, framebuffers, pipelines [0], vertexBuffer, indexBuffer, (uint)Logo.Indexes.Length);
			var fenceInfo = new FenceCreateInfo ();
			fence = device.CreateFence (fenceInfo);
			var semaphoreInfo = new SemaphoreCreateInfo ();
			semaphore = device.CreateSemaphore (semaphoreInfo);
			initialized = true;
		}

		protected override void OnDraw (Android.Graphics.Canvas canvas)
		{
			if (initialized)
				DrawFrame ();
		}

		void DrawFrame ()
		{
			uint nextIndex = device.AcquireNextImageKHR (swapchain, ulong.MaxValue, semaphore, fence);
			device.ResetFences (1, fence);
			var submitInfo = new SubmitInfo {
				WaitSemaphores = new Semaphore [] { semaphore },
				CommandBuffers = new CommandBuffer [] { commandBuffers [nextIndex] }
			};
			queue.Submit (1, submitInfo, fence);
			device.WaitForFences (1, fence, true, 100000000);
			var presentInfo = new PresentInfoKhr {
				Swapchains = new SwapchainKhr [] { swapchain },
				ImageIndices = new uint [] { nextIndex }
			};
			queue.PresentKHR (presentInfo);
		}
	}
}
