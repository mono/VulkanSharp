# Validation layers

Validation layers are special layers used to check the API usage and report debug information during the runtime. You can read more about them [here](https://github.com/KhronosGroup/Vulkan-LoaderAndValidationLayers/blob/master/layers/README.md).

## Enabling the validation layers

You can enable these layers, when creating an Instance. Here's an example:

~~~~
var instanceLayers = new string[] {
        "VK_LAYER_GOOGLE_threading",
        "VK_LAYER_LUNARG_parameter_validation",
        "VK_LAYER_LUNARG_device_limits",
        "VK_LAYER_LUNARG_object_tracker",
        "VK_LAYER_LUNARG_image",
        "VK_LAYER_LUNARG_core_validation",
        "VK_LAYER_LUNARG_swapchain",
        "VK_LAYER_GOOGLE_unique_objects"
};

var instance = new Instance (new InstanceCreateInfo () {
        ApplicationInfo = new Vulkan.ApplicationInfo () {
                ApplicationName = "...",
                ApiVersion = Vulkan.Version.Make (1, 0, 0)
        },
        EnabledExtensionNames = instanceExtensions,
        EnabledLayerNames = instanceLayers
});
~~~~

## Debug report extension

The layers interact through the debug report extension, so it is good idea to enable it as well.

~~~~
var instanceExtensions = new string[] {
        "VK_EXT_debug_report"
};
~~~~

To use the debug report extension, you will need to provide a debug report callback. You can do it like this:

~~~~
static Bool32 DebugReportCallback (DebugReportFlagsExt flags, DebugReportObjectTypeExt objectType, ulong objectHandle, IntPtr location, int messageCode, IntPtr layerPrefix, IntPtr message, IntPtr userData)
{
        string layerString = System.Runtime.InteropServices.Marshal.PtrToStringAnsi (layerPrefix);
        string messageString = System.Runtime.InteropServices.Marshal.PtrToStringAnsi (message);

        System.Console.WriteLine ("DebugReport layer: {0} message: {1}", layerString, messageString);

        return false;
}
Instance.DebugReportCallbackDelegate debugDelegate = new Instance.DebugReportCallbackDelegate (DebugReportCallback);
~~~~

## Enabling debug report

And finally pass the callback to your Instance:

~~~~
instance.EnableDebug (debugDelegate);
~~~~

## Android

On Android you will need to add validation layers shared libraries to your project from Android NDK and set their build action to AndroidNativeLibrary, so that the shared libraries are included in the apk.

Note that you need to build the layers inside the NDK to get the shared libraries. The libraries look like this (only arm64-v8a ABI listed here, they are available for other ABIs as well):

~~~~
libs/arm64-v8a
libs/arm64-v8a/libVkLayer_core_validation.so
libs/arm64-v8a/libVkLayer_device_limits.so
libs/arm64-v8a/libVkLayer_image.so
libs/arm64-v8a/libVkLayer_object_tracker.so
libs/arm64-v8a/libVkLayer_parameter_validation.so
libs/arm64-v8a/libVkLayer_swapchain.so
libs/arm64-v8a/libVkLayer_threading.so
libs/arm64-v8a/libVkLayer_unique_objects.so
~~~~
