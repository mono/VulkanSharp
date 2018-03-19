namespace Vulkan
{
    public interface IVulkanSample
    {
        void DrawFrame();
        void Initialize(PhysicalDevice physicalDevice, SurfaceKhr surface);
    }
}