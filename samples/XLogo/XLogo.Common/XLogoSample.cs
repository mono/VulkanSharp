using System.Reflection;
using Samples.Common;
using Vulkan;
using VulkanSample.Common;

namespace XLogo.Common
{
	public class XLogoSample : VulkanSampleBase
	{
		CommandBuffer [] commandBuffers;
		DescriptorSet [] descriptorSets;
		DescriptorSetLayout descriptorSetLayout;
		PipelineLayout pipelineLayout;
		Buffer uniformBuffer;

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
				buffers [i].CmdBindDescriptorSets (PipelineBindPoint.Graphics, pipelineLayout, 0, descriptorSets, null);
				buffers [i].CmdBindPipeline (PipelineBindPoint.Graphics, pipeline);
				buffers [i].CmdBindVertexBuffer (0, vertexBuffer, 0);
				buffers [i].CmdBindIndexBuffer (indexBuffer, 0, IndexType.Uint16);
				buffers [i].CmdDrawIndexed (indexLength, 1, 0, 0, 0);
				buffers [i].CmdEndRenderPass ();
				buffers [i].End ();
			}

			return buffers;
		}

		byte [] LoadResource (string name)
		{
			System.IO.Stream stream = typeof (XLogoSample).GetTypeInfo ().Assembly.GetManifestResourceStream (name);
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

		Pipeline [] CreatePipelines ()
		{
			var pipelineLayoutCreateInfo = new PipelineLayoutCreateInfo {
				SetLayouts = new DescriptorSetLayout [] { descriptorSetLayout }
			};
			pipelineLayout = device.CreatePipelineLayout (pipelineLayoutCreateInfo);
			var vertexShaderModule = device.CreateShaderModule (LoadResource ("XLogo.Common.Shaders.shader.vert.spv"));
			var fragmentShaderModule = device.CreateShaderModule (LoadResource ("XLogo.Common.Shaders.shader.frag.spv"));
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
				RasterizationSamples = SampleCountFlags.Count1
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
				Format = Format.R32G32B32Sfloat
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
				RenderPass = renderPass
			};

			return device.CreateGraphicsPipelines (device.CreatePipelineCache (new PipelineCacheCreateInfo ()), new GraphicsPipelineCreateInfo [] { pipelineCreateInfo });
		}

		Buffer CreateUniformBuffer (PhysicalDevice physicalDevice)
		{
			var uniformBufferData = new AreaUniformBuffer {
				width = surfaceCapabilities.CurrentExtent.Width,
				height = surfaceCapabilities.CurrentExtent.Height
			};

			return CreateBuffer (physicalDevice, uniformBufferData, BufferUsageFlags.UniformBuffer, typeof (AreaUniformBuffer));
		}

		DescriptorSet [] CreateDescriptorSets ()
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

			device.UpdateDescriptorSets (new WriteDescriptorSet [] { writeDescriptorSet }, null);
		}

		public override void Initialize (PhysicalDevice physicalDevice, SurfaceKhr surface)
		{
			base.Initialize (physicalDevice, surface);
			var vertexBuffer = CreateBuffer (physicalDevice, Logo.Vertices, BufferUsageFlags.VertexBuffer, typeof (float));
			var indexBuffer = CreateBuffer (physicalDevice, Logo.Indexes, BufferUsageFlags.IndexBuffer, typeof (short));
			uniformBuffer = CreateUniformBuffer (physicalDevice);
			descriptorSetLayout = CreateDescriptorSetLayout ();
			var pipelines = CreatePipelines ();
			descriptorSets = CreateDescriptorSets ();
			UpdateDescriptorSets ();

			commandBuffers = CreateCommandBuffers (images, framebuffers, pipelines [0], vertexBuffer, indexBuffer, (uint)Logo.Indexes.Length);
		}

		public override void DrawFrame ()
		{
			if (!initialized) return;

			uint nextIndex = device.AcquireNextImageKHR (swapchain, ulong.MaxValue, semaphore);
			device.ResetFence (fence);
			var submitInfo = new SubmitInfo {
				WaitSemaphores = new Semaphore [] { semaphore },
				WaitDstStageMask = new PipelineStageFlags [] { PipelineStageFlags.AllGraphics },
				CommandBuffers = new CommandBuffer [] { commandBuffers [nextIndex] }
			};
			queue.Submit (submitInfo, fence);
			device.WaitForFence (fence, true, 100000000);
			var presentInfo = new PresentInfoKhr {
				Swapchains = new SwapchainKhr [] { swapchain },
				ImageIndices = new uint [] { nextIndex }
			};
			queue.PresentKHR (presentInfo);
		}
	}
}
