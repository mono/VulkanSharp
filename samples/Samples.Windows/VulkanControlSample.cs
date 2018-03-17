using System;
using System.Windows.Forms;
using Vulkan;
using Vulkan.Windows;

namespace Samples.Windows
{
    public class VulkanControlSample : VulkanControl
    {
        private IVulkanSample _vulkanSample;
        private PhysicalDevice _physicalDevice;

        public VulkanControlSample(IVulkanSample vulkanSample)
        {
            _vulkanSample = vulkanSample;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _physicalDevice = Instance.EnumeratePhysicalDevices()[0];
            _vulkanSample.Initialize(_physicalDevice, Surface);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            _vulkanSample.DrawFrame();
        }
    }
}
