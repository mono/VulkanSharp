# VulkanSharp
[![Build Status][build-icon]][build-status]

[build-icon]:https://jenkins.mono-project.com/view/Urho/job/VulkanSharp/badge/icon
[build-status]:https://jenkins.mono-project.com/view/Urho/job/VulkanSharp/

This project provides a .NET binding for the
[Vulkan](https://www.khronos.org/vulkan/) API.

Nuget [package](https://www.nuget.org/packages/VulkanSharp/)

Check our [samples](https://github.com/mono/VulkanSharp/tree/master/samples) to see examples of using VulkanSharp

Regular Jenkins [builds](https://jenkins.mono-project.com/view/All/job/VulkanSharp/) with every push/PR

## Tutorials

How to use [Validation layers](https://github.com/mono/VulkanSharp/blob/master/docs/development/ValidationLayers.md)

## Building

### Windows

To build VulkanSharp, open VulkanSharp.sln in Visual Studio and build the solution. Alternatively you can also build it on the command line, run the `msbuild VulkanSharp.sln` command. It should download the needed dependencies.

### Mac/Linux

To build VulkanSharp, run the `make` command, which will download
the needed dependencies.

## Vulkan information

Specification from the Khronos group
[xhtml](https://www.khronos.org/registry/vulkan/specs/1.0/xhtml/vkspec.html),
[pdf](https://www.khronos.org/registry/vulkan/specs/1.0/pdf/vkspec.pdf)

To learn more about Vulkan, you can check [Vulkan in 30
minutes](https://renderdoc.org/vulkan-in-30-minutes.html)

### Vulkan on Android

Vulkan on Android (NVIDIA's devices)
[samples](https://developer.nvidia.com/vulkan-android)

More Android Vulkan samples
[googlesamples](https://github.com/googlesamples/android-vulkan-tutorials),
[examples and demos](https://github.com/SaschaWillems/Vulkan)
