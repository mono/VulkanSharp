using Vulkan;
using VulkanSample.Common;

namespace ClearView.Common
{
	public class ClearViewSample : VulkanSampleBase
	{
		CommandBuffer [] commandBuffers;

		CommandBuffer [] CreateCommandBuffers (Image [] images, Framebuffer [] framebuffers, RenderPass renderPass, SurfaceCapabilitiesKhr surfaceCapabilities)
		{
			var createPoolInfo = new CommandPoolCreateInfo { Flags = CommandPoolCreateFlags.ResetCommandBuffer };
			var commandPool = device.CreateCommandPool (createPoolInfo);
			var commandBufferAllocateInfo = new CommandBufferAllocateInfo {
				Level = CommandBufferLevel.Primary,
				CommandPool = commandPool,
				CommandBufferCount = (uint)images.Length
			};
			var buffers = device.AllocateCommandBuffers (commandBufferAllocateInfo);
			for (int i = 0; i < images.Length; i++) {

				var commandBufferBeginInfo = new CommandBufferBeginInfo ();
				buffers [i].Begin (commandBufferBeginInfo);
				var renderPassBeginInfo = new RenderPassBeginInfo {
					Framebuffer = framebuffers [i],
					RenderPass = renderPass,
					ClearValues = new ClearValue [] { new ClearValue { Color = new ClearColorValue (new float [] { 0.9f, 0.7f, 0.0f, 1.0f }) } },
					RenderArea = new Rect2D { Extent = surfaceCapabilities.CurrentExtent }
				};
				buffers [i].CmdBeginRenderPass (renderPassBeginInfo, SubpassContents.Inline);
				buffers [i].CmdEndRenderPass ();
				buffers [i].End ();
			}
			return buffers;
		}

		public override void Initialize (PhysicalDevice physicalDevice, SurfaceKhr surface)
		{
			base.Initialize (physicalDevice, surface);
			commandBuffers = CreateCommandBuffers (images, framebuffers, renderPass, surfaceCapabilities);
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