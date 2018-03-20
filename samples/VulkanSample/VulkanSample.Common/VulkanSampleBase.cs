using Vulkan;

namespace VulkanSample.Common
{
	public abstract class VulkanSampleBase : IVulkanSample
	{
		protected Device device;
		protected Queue queue;
		protected SwapchainKhr swapchain;
		protected Semaphore semaphore;
		protected SurfaceCapabilitiesKhr surfaceCapabilities;
		protected RenderPass renderPass;

		protected Fence fence;
		protected Image [] images;
		protected Framebuffer [] framebuffers;

		protected bool initialized;

		protected SurfaceFormatKhr SelectFormat (PhysicalDevice physicalDevice, SurfaceKhr surface)
		{
			foreach (var f in physicalDevice.GetSurfaceFormatsKHR (surface))
				if (f.Format == Format.R8G8B8A8Unorm || f.Format == Format.B8G8R8A8Unorm)
					return f;

			throw new System.Exception ("didn't find the R8G8B8A8Unorm or B8G8R8A8Unorm format");
		}

		protected SwapchainKhr CreateSwapchain (SurfaceKhr surface, SurfaceFormatKhr surfaceFormat)
		{
			var compositeAlpha = surfaceCapabilities.SupportedCompositeAlpha.HasFlag (CompositeAlphaFlagsKhr.Inherit)
				? CompositeAlphaFlagsKhr.Inherit
				: CompositeAlphaFlagsKhr.Opaque;

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
				CompositeAlpha = compositeAlpha
			};

			return device.CreateSwapchainKHR (swapchainInfo);
		}

		protected Framebuffer [] CreateFramebuffers (Image [] images, SurfaceFormatKhr surfaceFormat)
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

		protected RenderPass CreateRenderPass (SurfaceFormatKhr surfaceFormat)
		{
			var attDesc = new AttachmentDescription {
				Format = surfaceFormat.Format,
				Samples = SampleCountFlags.Count1,
				LoadOp = AttachmentLoadOp.Clear,
				StoreOp = AttachmentStoreOp.Store,
				StencilLoadOp = AttachmentLoadOp.DontCare,
				StencilStoreOp = AttachmentStoreOp.DontCare,
				InitialLayout = ImageLayout.Undefined,
				FinalLayout = ImageLayout.PresentSrcKhr
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

		public abstract void DrawFrame ();

		public virtual void Initialize (PhysicalDevice physicalDevice, SurfaceKhr surface)
		{
			var queueFamilyProperties = physicalDevice.GetQueueFamilyProperties ();

			uint queueFamilyUsedIndex;
			for (queueFamilyUsedIndex = 0; queueFamilyUsedIndex < queueFamilyProperties.Length; ++queueFamilyUsedIndex) {
				if (!physicalDevice.GetSurfaceSupportKHR (queueFamilyUsedIndex, surface)) continue;

				if (queueFamilyProperties [queueFamilyUsedIndex].QueueFlags.HasFlag (QueueFlags.Graphics)) break;
			}

			var queueInfo = new DeviceQueueCreateInfo { QueuePriorities = new float [] { 1.0f }, QueueFamilyIndex = queueFamilyUsedIndex };

			var deviceInfo = new DeviceCreateInfo {
				EnabledExtensionNames = new string [] { "VK_KHR_swapchain" },
				QueueCreateInfos = new DeviceQueueCreateInfo [] { queueInfo }
			};

			device = physicalDevice.CreateDevice (deviceInfo);
			queue = device.GetQueue (0, 0);
			surfaceCapabilities = physicalDevice.GetSurfaceCapabilitiesKHR (surface);
			var surfaceFormat = SelectFormat (physicalDevice, surface);
			swapchain = CreateSwapchain (surface, surfaceFormat);
			images = device.GetSwapchainImagesKHR (swapchain);
			renderPass = CreateRenderPass (surfaceFormat);
			framebuffers = CreateFramebuffers (images, surfaceFormat);
			var fenceInfo = new FenceCreateInfo ();
			fence = device.CreateFence (fenceInfo);
			var semaphoreInfo = new SemaphoreCreateInfo ();
			semaphore = device.CreateSemaphore (semaphoreInfo);
			initialized = true;
		}
	}
}