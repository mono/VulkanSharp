using System;
using Vulkan.Android;
using Vulkan;

namespace ClearView
{
	public class ClearView : VulkanView
	{
		Device device;
		Queue queue;
		SwapchainKhr swapchain;
		Semaphore semaphore;
		Fence fence;
		CommandBuffer [] commandBuffers;
		bool initialized = false;

		public ClearView (Android.Content.Context context) : base (context)
		{
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

			throw new Exception ("didn't find the R8g8b8a8Unorm format");
		}

		public void InitializeVulkan ()
		{
			var devices = Instance.EnumeratePhysicalDevices ();
			var surface = Instance.CreateAndroidSurfaceKHR (new AndroidSurfaceCreateInfoKhr () { Window = aNativeWindow }, null);
			var queueInfo = new DeviceQueueCreateInfo { QueuePriorities = new float [] { 1.0f } };
			var deviceInfo = new DeviceCreateInfo {
				EnabledExtensionNames = new string [] { "VK_KHR_swapchain", "VK_KHR_display_swapchain" },
				QueueCreateInfos = new DeviceQueueCreateInfo [] { queueInfo }
			};
			var physicalDevice = devices [0];
			device = physicalDevice.CreateDevice (deviceInfo, null);
			queue = device.GetQueue (0, 0);
			var surfaceCapabilities = physicalDevice.GetSurfaceCapabilitiesKHR (surface);
			var format = SelectFormat (physicalDevice, surface);
			var swapchainInfo = new SwapchainCreateInfoKhr {
				Surface = surface,
				MinImageCount = surfaceCapabilities.MinImageCount,
				ImageFormat = format.Format,
				ImageColorSpace = format.ColorSpace,
				ImageExtent = surfaceCapabilities.CurrentExtent,
				ImageUsage = (uint)ImageUsageFlags.ColorAttachment,
				PreTransform = SurfaceTransformFlagsKhr.Identity,
				ImageArrayLayers = 1,
				ImageSharingMode = SharingMode.Exclusive,
				QueueFamilyIndexCount = 1,
				QueueFamilyIndices = new uint [] { 0 },
				PresentMode = PresentModeKhr.Fifo,
				CompositeAlpha = CompositeAlphaFlagsKhr.Inherit
			};
			swapchain = device.CreateSwapchainKHR (swapchainInfo, null);
			var images = device.GetSwapchainImagesKHR (swapchain);
			var attDesc = new AttachmentDescription () {
				Format = format.Format,
				Samples = (uint)SampleCountFlags.Count1,
				LoadOp = AttachmentLoadOp.Clear,
				StoreOp = AttachmentStoreOp.Store,
				StencilLoadOp = AttachmentLoadOp.DontCare,
				StencilStoreOp = AttachmentStoreOp.DontCare,
				InitialLayout = ImageLayout.ColorAttachmentOptimal,
				FinalLayout = ImageLayout.ColorAttachmentOptimal
			};
			var attRef = new AttachmentReference () { Layout = ImageLayout.ColorAttachmentOptimal };
			var subpassDesc = new SubpassDescription () {
				PipelineBindPoint = PipelineBindPoint.Graphics,
				ColorAttachmentCount = 1,
				ColorAttachments = new AttachmentReference [] { attRef },
			};
			var renderPassCreateInfo = new RenderPassCreateInfo () {
				AttachmentCount = 1,
				Attachments = new AttachmentDescription [] { attDesc },
				SubpassCount = 1,
				Subpasses = new SubpassDescription [] { subpassDesc },
			};
			var renderPass = device.CreateRenderPass (renderPassCreateInfo, null);
			var images2 = device.GetSwapchainImagesKHR (swapchain);
			var displayViews = new ImageView [images2.Count];
			for (int i = 0; i < images2.Count; i++) {
				var viewCreateInfo = new ImageViewCreateInfo () {
					Image = images2 [i],
					ViewType = ImageViewType.View2D,
					Format = format.Format,
					Components = new ComponentMapping () {
						R = ComponentSwizzle.R,
						G = ComponentSwizzle.G,
						B = ComponentSwizzle.B,
						A = ComponentSwizzle.A
					},
					SubresourceRange = new ImageSubresourceRange () {
						AspectMask = (uint)ImageAspectFlags.Color,
						LevelCount = 1,
						LayerCount = 1
					}
				};
				displayViews [i] = device.CreateImageView (viewCreateInfo, null);
			}
			var framebuffers = new Framebuffer [images.Count];
			for (int i = 0; i < images.Count; i++) {
				var frameBufferCreateInfo = new FramebufferCreateInfo () {
					Layers = 1,
					RenderPass = renderPass,
					Attachments = new ImageView [] { displayViews [i] },
					Width = surfaceCapabilities.CurrentExtent.Width,
					Height = surfaceCapabilities.CurrentExtent.Height
				};
				framebuffers [i] = device.CreateFramebuffer (frameBufferCreateInfo, null);
			}
			var createPoolInfo = new CommandPoolCreateInfo () { Flags = (uint)CommandPoolCreateFlags.ResetCommandBuffer };
			var commandPool = device.CreateCommandPool (createPoolInfo, null);
			commandBuffers = new CommandBuffer [images.Count];
			for (int i = 0; i < images.Count; i++) {
				var commandBufferAllocateInfo = new CommandBufferAllocateInfo () {
					Level = CommandBufferLevel.Primary,
					CommandPool = commandPool,
					CommandBufferCount = 1
				};
				commandBuffers [i] = device.AllocateCommandBuffers (commandBufferAllocateInfo);

				var commandBufferBeginInfo = new CommandBufferBeginInfo ();
				commandBuffers [i].Begin (commandBufferBeginInfo);
				var renderPassBeginInfo = new RenderPassBeginInfo () {
					Framebuffer = framebuffers [i],
					RenderPass = renderPass,
					ClearValues = new ClearValue [] { new ClearValue () { Color = new ClearColorValue (new float [4] { 0.9f, 0.7f, 0.0f, 1.0f }) } },
					RenderArea = new Rect2D () { Extent = surfaceCapabilities.CurrentExtent }
				};
				commandBuffers [i].CmdBeginRenderPass (renderPassBeginInfo, SubpassContents.Inline);
				commandBuffers [i].CmdEndRenderPass ();
				commandBuffers [i].End ();
			}
			var fenceInfo = new FenceCreateInfo ();
			fence = device.CreateFence (fenceInfo, null);
			var semaphoreInfo = new SemaphoreCreateInfo ();
			semaphore = device.CreateSemaphore (semaphoreInfo, null);
			initialized = true;
		}

		protected override void OnDraw (global::Android.Graphics.Canvas canvas)
		{
			if (initialized)
				DrawFrame ();
		}

		void DrawFrame ()
		{
			uint nextIndex = device.AcquireNextImageKHR (swapchain, UInt64.MaxValue, semaphore, fence);
			device.ResetFences (1, fence);
			var submitInfo = new SubmitInfo () {
				WaitSemaphores = new Semaphore [] { semaphore },
				CommandBuffers = new CommandBuffer [] { commandBuffers [nextIndex] }
			};
			queue.Submit (1, submitInfo, fence);
			device.WaitForFences (1, fence, true, 100000000);
			var presentInfo = new PresentInfoKhr () {
				Swapchains = new SwapchainKhr [] { swapchain },
				ImageIndices = new uint [] { nextIndex },
			};
			queue.PresentKHR (presentInfo);
		}
	}
}

