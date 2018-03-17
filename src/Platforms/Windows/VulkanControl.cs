using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Vulkan.Windows
{
    public class VulkanControl : UserControl
    {
        public Instance Instance;
        public SurfaceKhr Surface;

        public VulkanControl(Instance instance = null) : base()
        {
            if (instance == null)
                CreateDefaultInstance();
            else
                Instance = instance;
        }

        protected void CreateDefaultInstance()
        {
            Instance = new Instance(new InstanceCreateInfo()
            {
                EnabledExtensionNames = new string[] { "VK_KHR_surface", "VK_KHR_win32_surface" },
                ApplicationInfo = new ApplicationInfo()
                {
                    ApiVersion = Vulkan.Version.Make(1, 0, 0)
                }
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Surface = Instance.CreateWin32SurfaceKHR(
                new Win32SurfaceCreateInfoKhr
                {
                    Hwnd = Handle,
                    Hinstance = Process.GetCurrentProcess().Handle
                });
        }
    }
}
