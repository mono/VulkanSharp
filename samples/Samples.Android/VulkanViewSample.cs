using Android.Content;
using Android.Graphics;
using Vulkan;
using Vulkan.Android;

namespace Samples.Android
{
    public class VulkanViewSample : VulkanView
    {
        private readonly IVulkanSample _vulkanSample;
        private PhysicalDevice _physicalDevice;
        private SurfaceKhr _surface;
        
        public VulkanViewSample(Context context, IVulkanSample vulkanSample) : base(context)
        {
            _vulkanSample = vulkanSample;
        }

        protected override void NativeWindowAcquired()
        {
            _physicalDevice = Instance.EnumeratePhysicalDevices()[0];
            _surface = Instance.CreateAndroidSurfaceKHR(new AndroidSurfaceCreateInfoKhr { Window = aNativeWindow });
            _vulkanSample.Initialize(_physicalDevice, _surface);

            base.NativeWindowAcquired();
        }

        protected override void OnDraw(Canvas canvas)
        {
            _vulkanSample.DrawFrame();
        }
    }
}

