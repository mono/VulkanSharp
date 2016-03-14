using System;
using System.Runtime.InteropServices;

namespace Vulkan.Interop
{
	internal static class NativeMethods
	{
		const string VulkanLibrary = "vulkan";

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateInstance (InstanceCreateInfo* CreateInfo, AllocationCallbacks* Allocator, IntPtr* Instance);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyInstance (IntPtr instance, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumeratePhysicalDevices (IntPtr instance, UInt32* PhysicalDeviceCount, IntPtr* PhysicalDevices);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern IntPtr vkGetDeviceProcAddr (IntPtr device, string pName);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern IntPtr vkGetInstanceProcAddr (IntPtr instance, string pName);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceProperties (IntPtr physicalDevice, PhysicalDeviceProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceQueueFamilyProperties (IntPtr physicalDevice, UInt32* QueueFamilyPropertyCount, QueueFamilyProperties* QueueFamilyProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceMemoryProperties (IntPtr physicalDevice, PhysicalDeviceMemoryProperties* MemoryProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceFeatures (IntPtr physicalDevice, PhysicalDeviceFeatures* Features);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceFormatProperties (IntPtr physicalDevice, Format format, FormatProperties* FormatProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceImageFormatProperties (IntPtr physicalDevice, Format format, ImageType type, ImageTiling tiling, ImageUsageFlags usage, ImageCreateFlags flags, ImageFormatProperties* ImageFormatProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDevice (IntPtr physicalDevice, DeviceCreateInfo* CreateInfo, AllocationCallbacks* Allocator, IntPtr* Device);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDevice (IntPtr device, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateInstanceLayerProperties (UInt32* PropertyCount, LayerProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateInstanceExtensionProperties (string pLayerName, UInt32* PropertyCount, ExtensionProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateDeviceLayerProperties (IntPtr physicalDevice, UInt32* PropertyCount, LayerProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateDeviceExtensionProperties (IntPtr physicalDevice, string pLayerName, UInt32* PropertyCount, ExtensionProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetDeviceQueue (IntPtr device, UInt32 queueFamilyIndex, UInt32 queueIndex, IntPtr* Queue);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueueSubmit (IntPtr queue, UInt32 submitCount, SubmitInfo* Submits, UInt64 fence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueueWaitIdle (IntPtr queue);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkDeviceWaitIdle (IntPtr device);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAllocateMemory (IntPtr device, MemoryAllocateInfo* AllocateInfo, AllocationCallbacks* Allocator, UInt64* Memory);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkFreeMemory (IntPtr device, UInt64 memory, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkMapMemory (IntPtr device, UInt64 memory, DeviceSize offset, DeviceSize size, UInt32 flags, IntPtr* pData);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkUnmapMemory (IntPtr device, UInt64 memory);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkFlushMappedMemoryRanges (IntPtr device, UInt32 memoryRangeCount, MappedMemoryRange* MemoryRanges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkInvalidateMappedMemoryRanges (IntPtr device, UInt32 memoryRangeCount, MappedMemoryRange* MemoryRanges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetDeviceMemoryCommitment (IntPtr device, UInt64 memory, DeviceSize* CommittedMemoryInBytes);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetBufferMemoryRequirements (IntPtr device, UInt64 buffer, MemoryRequirements* MemoryRequirements);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkBindBufferMemory (IntPtr device, UInt64 buffer, UInt64 memory, DeviceSize memoryOffset);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetImageMemoryRequirements (IntPtr device, UInt64 image, MemoryRequirements* MemoryRequirements);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkBindImageMemory (IntPtr device, UInt64 image, UInt64 memory, DeviceSize memoryOffset);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetImageSparseMemoryRequirements (IntPtr device, UInt64 image, UInt32* SparseMemoryRequirementCount, SparseImageMemoryRequirements* SparseMemoryRequirements);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceSparseImageFormatProperties (IntPtr physicalDevice, Format format, ImageType type, SampleCountFlags samples, ImageUsageFlags usage, ImageTiling tiling, UInt32* PropertyCount, SparseImageFormatProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueueBindSparse (IntPtr queue, UInt32 bindInfoCount, BindSparseInfo* BindInfo, UInt64 fence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateFence (IntPtr device, FenceCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* Fence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyFence (IntPtr device, UInt64 fence, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetFences (IntPtr device, UInt32 fenceCount, UInt64* Fences);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetFenceStatus (IntPtr device, UInt64 fence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkWaitForFences (IntPtr device, UInt32 fenceCount, UInt64* Fences, Bool32 waitAll, UInt64 timeout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSemaphore (IntPtr device, SemaphoreCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* Semaphore);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySemaphore (IntPtr device, UInt64 semaphore, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateEvent (IntPtr device, EventCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* Event);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyEvent (IntPtr device, UInt64 @event, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetEventStatus (IntPtr device, UInt64 @event);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkSetEvent (IntPtr device, UInt64 @event);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetEvent (IntPtr device, UInt64 @event);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateQueryPool (IntPtr device, QueryPoolCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* QueryPool);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyQueryPool (IntPtr device, UInt64 queryPool, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetQueryPoolResults (IntPtr device, UInt64 queryPool, UInt32 firstQuery, UInt32 queryCount, UIntPtr dataSize, IntPtr Data, DeviceSize stride, QueryResultFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateBuffer (IntPtr device, BufferCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* Buffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyBuffer (IntPtr device, UInt64 buffer, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateBufferView (IntPtr device, BufferViewCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* View);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyBufferView (IntPtr device, UInt64 bufferView, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateImage (IntPtr device, ImageCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* Image);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyImage (IntPtr device, UInt64 image, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetImageSubresourceLayout (IntPtr device, UInt64 image, ImageSubresource* Subresource, SubresourceLayout* Layout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateImageView (IntPtr device, ImageViewCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* View);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyImageView (IntPtr device, UInt64 imageView, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateShaderModule (IntPtr device, ShaderModuleCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* ShaderModule);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyShaderModule (IntPtr device, UInt64 shaderModule, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreatePipelineCache (IntPtr device, PipelineCacheCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* PipelineCache);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyPipelineCache (IntPtr device, UInt64 pipelineCache, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPipelineCacheData (IntPtr device, UInt64 pipelineCache, UIntPtr* DataSize, IntPtr Data);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkMergePipelineCaches (IntPtr device, UInt64 dstCache, UInt32 srcCacheCount, UInt64* SrcCaches);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateGraphicsPipelines (IntPtr device, UInt64 pipelineCache, UInt32 createInfoCount, GraphicsPipelineCreateInfo* CreateInfos, AllocationCallbacks* Allocator, UInt64* Pipelines);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateComputePipelines (IntPtr device, UInt64 pipelineCache, UInt32 createInfoCount, ComputePipelineCreateInfo* CreateInfos, AllocationCallbacks* Allocator, UInt64* Pipelines);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyPipeline (IntPtr device, UInt64 pipeline, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreatePipelineLayout (IntPtr device, PipelineLayoutCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* PipelineLayout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyPipelineLayout (IntPtr device, UInt64 pipelineLayout, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSampler (IntPtr device, SamplerCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* Sampler);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySampler (IntPtr device, UInt64 sampler, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDescriptorSetLayout (IntPtr device, DescriptorSetLayoutCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* SetLayout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDescriptorSetLayout (IntPtr device, UInt64 descriptorSetLayout, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDescriptorPool (IntPtr device, DescriptorPoolCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* DescriptorPool);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDescriptorPool (IntPtr device, UInt64 descriptorPool, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetDescriptorPool (IntPtr device, UInt64 descriptorPool, UInt32 flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAllocateDescriptorSets (IntPtr device, DescriptorSetAllocateInfo* AllocateInfo, UInt64* DescriptorSets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkFreeDescriptorSets (IntPtr device, UInt64 descriptorPool, UInt32 descriptorSetCount, UInt64* DescriptorSets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkUpdateDescriptorSets (IntPtr device, UInt32 descriptorWriteCount, WriteDescriptorSet* DescriptorWrites, UInt32 descriptorCopyCount, CopyDescriptorSet* DescriptorCopies);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateFramebuffer (IntPtr device, FramebufferCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* Framebuffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyFramebuffer (IntPtr device, UInt64 framebuffer, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateRenderPass (IntPtr device, RenderPassCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* RenderPass);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyRenderPass (IntPtr device, UInt64 renderPass, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetRenderAreaGranularity (IntPtr device, UInt64 renderPass, Extent2D* Granularity);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateCommandPool (IntPtr device, CommandPoolCreateInfo* CreateInfo, AllocationCallbacks* Allocator, UInt64* CommandPool);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyCommandPool (IntPtr device, UInt64 commandPool, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetCommandPool (IntPtr device, UInt64 commandPool, CommandPoolResetFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAllocateCommandBuffers (IntPtr device, CommandBufferAllocateInfo* AllocateInfo, IntPtr* CommandBuffers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkFreeCommandBuffers (IntPtr device, UInt64 commandPool, UInt32 commandBufferCount, IntPtr* CommandBuffers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkBeginCommandBuffer (IntPtr commandBuffer, CommandBufferBeginInfo* BeginInfo);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEndCommandBuffer (IntPtr commandBuffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetCommandBuffer (IntPtr commandBuffer, CommandBufferResetFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindPipeline (IntPtr commandBuffer, PipelineBindPoint pipelineBindPoint, UInt64 pipeline);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetViewport (IntPtr commandBuffer, UInt32 firstViewport, UInt32 viewportCount, Viewport* Viewports);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetScissor (IntPtr commandBuffer, UInt32 firstScissor, UInt32 scissorCount, Rect2D* Scissors);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetLineWidth (IntPtr commandBuffer, float lineWidth);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetDepthBias (IntPtr commandBuffer, float depthBiasConstantFactor, float depthBiasClamp, float depthBiasSlopeFactor);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetBlendConstants (IntPtr commandBuffer, float blendConstants);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetDepthBounds (IntPtr commandBuffer, float minDepthBounds, float maxDepthBounds);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetStencilCompareMask (IntPtr commandBuffer, StencilFaceFlags faceMask, UInt32 compareMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetStencilWriteMask (IntPtr commandBuffer, StencilFaceFlags faceMask, UInt32 writeMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetStencilReference (IntPtr commandBuffer, StencilFaceFlags faceMask, UInt32 reference);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindDescriptorSets (IntPtr commandBuffer, PipelineBindPoint pipelineBindPoint, UInt64 layout, UInt32 firstSet, UInt32 descriptorSetCount, UInt64* DescriptorSets, UInt32 dynamicOffsetCount, UInt32* DynamicOffsets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindIndexBuffer (IntPtr commandBuffer, UInt64 buffer, DeviceSize offset, IndexType indexType);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindVertexBuffers (IntPtr commandBuffer, UInt32 firstBinding, UInt32 bindingCount, UInt64* Buffers, DeviceSize* Offsets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDraw (IntPtr commandBuffer, UInt32 vertexCount, UInt32 instanceCount, UInt32 firstVertex, UInt32 firstInstance);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDrawIndexed (IntPtr commandBuffer, UInt32 indexCount, UInt32 instanceCount, UInt32 firstIndex, Int32 vertexOffset, UInt32 firstInstance);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDrawIndirect (IntPtr commandBuffer, UInt64 buffer, DeviceSize offset, UInt32 drawCount, UInt32 stride);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDrawIndexedIndirect (IntPtr commandBuffer, UInt64 buffer, DeviceSize offset, UInt32 drawCount, UInt32 stride);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDispatch (IntPtr commandBuffer, UInt32 x, UInt32 y, UInt32 z);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDispatchIndirect (IntPtr commandBuffer, UInt64 buffer, DeviceSize offset);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyBuffer (IntPtr commandBuffer, UInt64 srcBuffer, UInt64 dstBuffer, UInt32 regionCount, BufferCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyImage (IntPtr commandBuffer, UInt64 srcImage, ImageLayout srcImageLayout, UInt64 dstImage, ImageLayout dstImageLayout, UInt32 regionCount, ImageCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBlitImage (IntPtr commandBuffer, UInt64 srcImage, ImageLayout srcImageLayout, UInt64 dstImage, ImageLayout dstImageLayout, UInt32 regionCount, ImageBlit* Regions, Filter filter);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyBufferToImage (IntPtr commandBuffer, UInt64 srcBuffer, UInt64 dstImage, ImageLayout dstImageLayout, UInt32 regionCount, BufferImageCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyImageToBuffer (IntPtr commandBuffer, UInt64 srcImage, ImageLayout srcImageLayout, UInt64 dstBuffer, UInt32 regionCount, BufferImageCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdUpdateBuffer (IntPtr commandBuffer, UInt64 dstBuffer, DeviceSize dstOffset, DeviceSize dataSize, UInt32* Data);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdFillBuffer (IntPtr commandBuffer, UInt64 dstBuffer, DeviceSize dstOffset, DeviceSize size, UInt32 data);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdClearColorImage (IntPtr commandBuffer, UInt64 image, ImageLayout imageLayout, ClearColorValue* Color, UInt32 rangeCount, ImageSubresourceRange* Ranges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdClearDepthStencilImage (IntPtr commandBuffer, UInt64 image, ImageLayout imageLayout, ClearDepthStencilValue* DepthStencil, UInt32 rangeCount, ImageSubresourceRange* Ranges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdClearAttachments (IntPtr commandBuffer, UInt32 attachmentCount, ClearAttachment* Attachments, UInt32 rectCount, ClearRect* Rects);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdResolveImage (IntPtr commandBuffer, UInt64 srcImage, ImageLayout srcImageLayout, UInt64 dstImage, ImageLayout dstImageLayout, UInt32 regionCount, ImageResolve* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetEvent (IntPtr commandBuffer, UInt64 @event, PipelineStageFlags stageMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdResetEvent (IntPtr commandBuffer, UInt64 @event, PipelineStageFlags stageMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdWaitEvents (IntPtr commandBuffer, UInt32 eventCount, UInt64* Events, PipelineStageFlags srcStageMask, PipelineStageFlags dstStageMask, UInt32 memoryBarrierCount, MemoryBarrier* MemoryBarriers, UInt32 bufferMemoryBarrierCount, BufferMemoryBarrier* BufferMemoryBarriers, UInt32 imageMemoryBarrierCount, ImageMemoryBarrier* ImageMemoryBarriers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdPipelineBarrier (IntPtr commandBuffer, PipelineStageFlags srcStageMask, PipelineStageFlags dstStageMask, DependencyFlags dependencyFlags, UInt32 memoryBarrierCount, MemoryBarrier* MemoryBarriers, UInt32 bufferMemoryBarrierCount, BufferMemoryBarrier* BufferMemoryBarriers, UInt32 imageMemoryBarrierCount, ImageMemoryBarrier* ImageMemoryBarriers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBeginQuery (IntPtr commandBuffer, UInt64 queryPool, UInt32 query, QueryControlFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdEndQuery (IntPtr commandBuffer, UInt64 queryPool, UInt32 query);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdResetQueryPool (IntPtr commandBuffer, UInt64 queryPool, UInt32 firstQuery, UInt32 queryCount);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdWriteTimestamp (IntPtr commandBuffer, PipelineStageFlags pipelineStage, UInt64 queryPool, UInt32 query);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyQueryPoolResults (IntPtr commandBuffer, UInt64 queryPool, UInt32 firstQuery, UInt32 queryCount, UInt64 dstBuffer, DeviceSize dstOffset, DeviceSize stride, QueryResultFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdPushConstants (IntPtr commandBuffer, UInt64 layout, ShaderStageFlags stageFlags, UInt32 offset, UInt32 size, IntPtr Values);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBeginRenderPass (IntPtr commandBuffer, RenderPassBeginInfo* RenderPassBegin, SubpassContents contents);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdNextSubpass (IntPtr commandBuffer, SubpassContents contents);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdEndRenderPass (IntPtr commandBuffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdExecuteCommands (IntPtr commandBuffer, UInt32 commandBufferCount, IntPtr* CommandBuffers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceDisplayPropertiesKHR (IntPtr physicalDevice, UInt32* PropertyCount, DisplayPropertiesKhr* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceDisplayPlanePropertiesKHR (IntPtr physicalDevice, UInt32* PropertyCount, DisplayPlanePropertiesKhr* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetDisplayPlaneSupportedDisplaysKHR (IntPtr physicalDevice, UInt32 planeIndex, UInt32* DisplayCount, UInt64* Displays);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetDisplayModePropertiesKHR (IntPtr physicalDevice, UInt64 display, UInt32* PropertyCount, DisplayModePropertiesKhr* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDisplayModeKHR (IntPtr physicalDevice, UInt64 display, DisplayModeCreateInfoKhr* CreateInfo, AllocationCallbacks* Allocator, UInt64* Mode);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetDisplayPlaneCapabilitiesKHR (IntPtr physicalDevice, UInt64 mode, UInt32 planeIndex, DisplayPlaneCapabilitiesKhr* Capabilities);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDisplayPlaneSurfaceKHR (IntPtr instance, DisplaySurfaceCreateInfoKhr* CreateInfo, AllocationCallbacks* Allocator, UInt64* Surface);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSharedSwapchainsKHR (IntPtr device, UInt32 swapchainCount, SwapchainCreateInfoKhr* CreateInfos, AllocationCallbacks* Allocator, UInt64* Swapchains);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySurfaceKHR (IntPtr instance, UInt64 surface, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfaceSupportKHR (IntPtr physicalDevice, UInt32 queueFamilyIndex, UInt64 surface, Bool32* Supported);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfaceCapabilitiesKHR (IntPtr physicalDevice, UInt64 surface, SurfaceCapabilitiesKhr* SurfaceCapabilities);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfaceFormatsKHR (IntPtr physicalDevice, UInt64 surface, UInt32* SurfaceFormatCount, SurfaceFormatKhr* SurfaceFormats);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfacePresentModesKHR (IntPtr physicalDevice, UInt64 surface, UInt32* PresentModeCount, PresentModeKhr* PresentModes);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSwapchainKHR (IntPtr device, SwapchainCreateInfoKhr* CreateInfo, AllocationCallbacks* Allocator, UInt64* Swapchain);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySwapchainKHR (IntPtr device, UInt64 swapchain, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetSwapchainImagesKHR (IntPtr device, UInt64 swapchain, UInt32* SwapchainImageCount, UInt64* SwapchainImages);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAcquireNextImageKHR (IntPtr device, UInt64 swapchain, UInt64 timeout, UInt64 semaphore, UInt64 fence, UInt32* ImageIndex);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueuePresentKHR (IntPtr queue, PresentInfoKhr* PresentInfo);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Bool32 vkGetPhysicalDeviceWin32PresentationSupportKHR (IntPtr physicalDevice, UInt32 queueFamilyIndex);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDebugReportCallbackEXT (IntPtr instance, DebugReportCallbackCreateInfoExt* CreateInfo, AllocationCallbacks* Allocator, UInt64* Callback);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDebugReportCallbackEXT (IntPtr instance, UInt64 callback, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDebugReportMessageEXT (IntPtr instance, DebugReportFlagsExt flags, DebugReportObjectTypeExt objectType, UInt64 @object, UIntPtr location, Int32 messageCode, string pLayerPrefix, string pMessage);
	}
}
