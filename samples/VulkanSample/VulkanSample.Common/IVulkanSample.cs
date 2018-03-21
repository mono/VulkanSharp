using Vulkan;

namespace VulkanSample.Common
{
	public interface IVulkanSample
	{
		void DrawFrame ();
		void Initialize (PhysicalDevice physicalDevice, SurfaceKhr surface);
	}
}