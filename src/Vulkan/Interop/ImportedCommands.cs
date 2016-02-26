using System;
using System.Runtime.InteropServices;

namespace Vulkan.Interop
{
	internal static class NativeMethods
	{
		const string VulkanLibrary = "vulkan";

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateInstance (InstanceCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pInstance);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyInstance (Instance instance, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumeratePhysicalDevices (Instance instance, out UInt32* PhysicalDeviceCount, out IntPtr pPhysicalDevices);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern IntPtr vkGetDeviceProcAddr (Device device, IntPtr Name);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern IntPtr vkGetInstanceProcAddr (Instance instance, IntPtr Name);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceProperties (PhysicalDevice physicalDevice, PhysicalDeviceProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceQueueFamilyProperties (PhysicalDevice physicalDevice, out UInt32* QueueFamilyPropertyCount, QueueFamilyProperties* QueueFamilyProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceMemoryProperties (PhysicalDevice physicalDevice, PhysicalDeviceMemoryProperties* MemoryProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceFeatures (PhysicalDevice physicalDevice, PhysicalDeviceFeatures* Features);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceFormatProperties (PhysicalDevice physicalDevice, Format format, FormatProperties* FormatProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceImageFormatProperties (PhysicalDevice physicalDevice, Format format, ImageType type, ImageTiling tiling, ImageUsageFlags usage, ImageCreateFlags flags, ImageFormatProperties* ImageFormatProperties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDevice (PhysicalDevice physicalDevice, DeviceCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pDevice);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDevice (Device device, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateInstanceLayerProperties (out UInt32* PropertyCount, LayerProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateInstanceExtensionProperties (IntPtr LayerName, out UInt32* PropertyCount, ExtensionProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateDeviceLayerProperties (PhysicalDevice physicalDevice, out UInt32* PropertyCount, LayerProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEnumerateDeviceExtensionProperties (PhysicalDevice physicalDevice, IntPtr LayerName, out UInt32* PropertyCount, ExtensionProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetDeviceQueue (Device device, UInt32 queueFamilyIndex, UInt32 queueIndex, out IntPtr pQueue);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueueSubmit (Queue queue, UInt32 submitCount, SubmitInfo* Submits, Fence fence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueueWaitIdle (Queue queue);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkDeviceWaitIdle (Device device);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAllocateMemory (Device device, MemoryAllocateInfo* AllocateInfo, AllocationCallbacks* Allocator, out IntPtr pMemory);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkFreeMemory (Device device, DeviceMemory memory, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkMapMemory (Device device, DeviceMemory memory, DeviceSize offset, DeviceSize size, UInt32 flags, out IntPtr pData);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkUnmapMemory (Device device, DeviceMemory memory);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkFlushMappedMemoryRanges (Device device, UInt32 memoryRangeCount, MappedMemoryRange* MemoryRanges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkInvalidateMappedMemoryRanges (Device device, UInt32 memoryRangeCount, MappedMemoryRange* MemoryRanges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetDeviceMemoryCommitment (Device device, DeviceMemory memory, out DeviceSize* CommittedMemoryInBytes);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetBufferMemoryRequirements (Device device, Buffer buffer, MemoryRequirements* MemoryRequirements);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkBindBufferMemory (Device device, Buffer buffer, DeviceMemory memory, DeviceSize memoryOffset);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetImageMemoryRequirements (Device device, Image image, MemoryRequirements* MemoryRequirements);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkBindImageMemory (Device device, Image image, DeviceMemory memory, DeviceSize memoryOffset);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetImageSparseMemoryRequirements (Device device, Image image, out UInt32* SparseMemoryRequirementCount, SparseImageMemoryRequirements* SparseMemoryRequirements);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetPhysicalDeviceSparseImageFormatProperties (PhysicalDevice physicalDevice, Format format, ImageType type, SampleCountFlags samples, ImageUsageFlags usage, ImageTiling tiling, out UInt32* PropertyCount, SparseImageFormatProperties* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueueBindSparse (Queue queue, UInt32 bindInfoCount, BindSparseInfo* BindInfo, Fence fence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateFence (Device device, FenceCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pFence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyFence (Device device, Fence fence, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetFences (Device device, UInt32 fenceCount, IntPtr pFences);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetFenceStatus (Device device, Fence fence);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkWaitForFences (Device device, UInt32 fenceCount, IntPtr pFences, Bool32 waitAll, UInt64 timeout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSemaphore (Device device, SemaphoreCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pSemaphore);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySemaphore (Device device, Semaphore semaphore, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateEvent (Device device, EventCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pEvent);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyEvent (Device device, Event @event, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetEventStatus (Device device, Event @event);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkSetEvent (Device device, Event @event);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetEvent (Device device, Event @event);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateQueryPool (Device device, QueryPoolCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pQueryPool);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyQueryPool (Device device, QueryPool queryPool, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetQueryPoolResults (Device device, QueryPool queryPool, UInt32 firstQuery, UInt32 queryCount, UIntPtr dataSize, out IntPtr Data, DeviceSize stride, QueryResultFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateBuffer (Device device, BufferCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pBuffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyBuffer (Device device, Buffer buffer, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateBufferView (Device device, BufferViewCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pView);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyBufferView (Device device, BufferView bufferView, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateImage (Device device, ImageCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pImage);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyImage (Device device, Image image, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetImageSubresourceLayout (Device device, Image image, ImageSubresource* Subresource, SubresourceLayout* Layout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateImageView (Device device, ImageViewCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pView);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyImageView (Device device, ImageView imageView, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateShaderModule (Device device, ShaderModuleCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pShaderModule);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyShaderModule (Device device, ShaderModule shaderModule, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreatePipelineCache (Device device, PipelineCacheCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pPipelineCache);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyPipelineCache (Device device, PipelineCache pipelineCache, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPipelineCacheData (Device device, PipelineCache pipelineCache, out UIntPtr* DataSize, out IntPtr Data);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkMergePipelineCaches (Device device, PipelineCache dstCache, UInt32 srcCacheCount, IntPtr pSrcCaches);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateGraphicsPipelines (Device device, PipelineCache pipelineCache, UInt32 createInfoCount, GraphicsPipelineCreateInfo* CreateInfos, AllocationCallbacks* Allocator, out IntPtr pPipelines);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateComputePipelines (Device device, PipelineCache pipelineCache, UInt32 createInfoCount, ComputePipelineCreateInfo* CreateInfos, AllocationCallbacks* Allocator, out IntPtr pPipelines);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyPipeline (Device device, Pipeline pipeline, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreatePipelineLayout (Device device, PipelineLayoutCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pPipelineLayout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyPipelineLayout (Device device, PipelineLayout pipelineLayout, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSampler (Device device, SamplerCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pSampler);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySampler (Device device, Sampler sampler, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDescriptorSetLayout (Device device, DescriptorSetLayoutCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pSetLayout);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDescriptorSetLayout (Device device, DescriptorSetLayout descriptorSetLayout, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDescriptorPool (Device device, DescriptorPoolCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pDescriptorPool);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDescriptorPool (Device device, DescriptorPool descriptorPool, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetDescriptorPool (Device device, DescriptorPool descriptorPool, UInt32 flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAllocateDescriptorSets (Device device, DescriptorSetAllocateInfo* AllocateInfo, out IntPtr pDescriptorSets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkFreeDescriptorSets (Device device, DescriptorPool descriptorPool, UInt32 descriptorSetCount, IntPtr pDescriptorSets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkUpdateDescriptorSets (Device device, UInt32 descriptorWriteCount, WriteDescriptorSet* DescriptorWrites, UInt32 descriptorCopyCount, CopyDescriptorSet* DescriptorCopies);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateFramebuffer (Device device, FramebufferCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pFramebuffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyFramebuffer (Device device, Framebuffer framebuffer, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateRenderPass (Device device, RenderPassCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pRenderPass);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyRenderPass (Device device, RenderPass renderPass, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkGetRenderAreaGranularity (Device device, RenderPass renderPass, Extent2D* Granularity);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateCommandPool (Device device, CommandPoolCreateInfo* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pCommandPool);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyCommandPool (Device device, CommandPool commandPool, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetCommandPool (Device device, CommandPool commandPool, CommandPoolResetFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAllocateCommandBuffers (Device device, CommandBufferAllocateInfo* AllocateInfo, out IntPtr pCommandBuffers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkFreeCommandBuffers (Device device, CommandPool commandPool, UInt32 commandBufferCount, IntPtr pCommandBuffers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkBeginCommandBuffer (CommandBuffer commandBuffer, CommandBufferBeginInfo* BeginInfo);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkEndCommandBuffer (CommandBuffer commandBuffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkResetCommandBuffer (CommandBuffer commandBuffer, CommandBufferResetFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindPipeline (CommandBuffer commandBuffer, PipelineBindPoint pipelineBindPoint, Pipeline pipeline);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetViewport (CommandBuffer commandBuffer, UInt32 firstViewport, UInt32 viewportCount, Viewport* Viewports);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetScissor (CommandBuffer commandBuffer, UInt32 firstScissor, UInt32 scissorCount, Rect2D* Scissors);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetLineWidth (CommandBuffer commandBuffer, float lineWidth);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetDepthBias (CommandBuffer commandBuffer, float depthBiasConstantFactor, float depthBiasClamp, float depthBiasSlopeFactor);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetBlendConstants (CommandBuffer commandBuffer, float blendConstants);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetDepthBounds (CommandBuffer commandBuffer, float minDepthBounds, float maxDepthBounds);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetStencilCompareMask (CommandBuffer commandBuffer, StencilFaceFlags faceMask, UInt32 compareMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetStencilWriteMask (CommandBuffer commandBuffer, StencilFaceFlags faceMask, UInt32 writeMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetStencilReference (CommandBuffer commandBuffer, StencilFaceFlags faceMask, UInt32 reference);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindDescriptorSets (CommandBuffer commandBuffer, PipelineBindPoint pipelineBindPoint, PipelineLayout layout, UInt32 firstSet, UInt32 descriptorSetCount, IntPtr pDescriptorSets, UInt32 dynamicOffsetCount, UInt32* DynamicOffsets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindIndexBuffer (CommandBuffer commandBuffer, Buffer buffer, DeviceSize offset, IndexType indexType);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBindVertexBuffers (CommandBuffer commandBuffer, UInt32 firstBinding, UInt32 bindingCount, IntPtr pBuffers, DeviceSize* Offsets);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDraw (CommandBuffer commandBuffer, UInt32 vertexCount, UInt32 instanceCount, UInt32 firstVertex, UInt32 firstInstance);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDrawIndexed (CommandBuffer commandBuffer, UInt32 indexCount, UInt32 instanceCount, UInt32 firstIndex, Int32 vertexOffset, UInt32 firstInstance);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDrawIndirect (CommandBuffer commandBuffer, Buffer buffer, DeviceSize offset, UInt32 drawCount, UInt32 stride);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDrawIndexedIndirect (CommandBuffer commandBuffer, Buffer buffer, DeviceSize offset, UInt32 drawCount, UInt32 stride);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDispatch (CommandBuffer commandBuffer, UInt32 x, UInt32 y, UInt32 z);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdDispatchIndirect (CommandBuffer commandBuffer, Buffer buffer, DeviceSize offset);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyBuffer (CommandBuffer commandBuffer, Buffer srcBuffer, Buffer dstBuffer, UInt32 regionCount, BufferCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyImage (CommandBuffer commandBuffer, Image srcImage, ImageLayout srcImageLayout, Image dstImage, ImageLayout dstImageLayout, UInt32 regionCount, ImageCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBlitImage (CommandBuffer commandBuffer, Image srcImage, ImageLayout srcImageLayout, Image dstImage, ImageLayout dstImageLayout, UInt32 regionCount, ImageBlit* Regions, Filter filter);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyBufferToImage (CommandBuffer commandBuffer, Buffer srcBuffer, Image dstImage, ImageLayout dstImageLayout, UInt32 regionCount, BufferImageCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyImageToBuffer (CommandBuffer commandBuffer, Image srcImage, ImageLayout srcImageLayout, Buffer dstBuffer, UInt32 regionCount, BufferImageCopy* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdUpdateBuffer (CommandBuffer commandBuffer, Buffer dstBuffer, DeviceSize dstOffset, DeviceSize dataSize, UInt32* Data);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdFillBuffer (CommandBuffer commandBuffer, Buffer dstBuffer, DeviceSize dstOffset, DeviceSize size, UInt32 data);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdClearColorImage (CommandBuffer commandBuffer, Image image, ImageLayout imageLayout, ClearColorValue* Color, UInt32 rangeCount, ImageSubresourceRange* Ranges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdClearDepthStencilImage (CommandBuffer commandBuffer, Image image, ImageLayout imageLayout, ClearDepthStencilValue* DepthStencil, UInt32 rangeCount, ImageSubresourceRange* Ranges);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdClearAttachments (CommandBuffer commandBuffer, UInt32 attachmentCount, ClearAttachment* Attachments, UInt32 rectCount, ClearRect* Rects);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdResolveImage (CommandBuffer commandBuffer, Image srcImage, ImageLayout srcImageLayout, Image dstImage, ImageLayout dstImageLayout, UInt32 regionCount, ImageResolve* Regions);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdSetEvent (CommandBuffer commandBuffer, Event @event, PipelineStageFlags stageMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdResetEvent (CommandBuffer commandBuffer, Event @event, PipelineStageFlags stageMask);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdWaitEvents (CommandBuffer commandBuffer, UInt32 eventCount, IntPtr pEvents, PipelineStageFlags srcStageMask, PipelineStageFlags dstStageMask, UInt32 memoryBarrierCount, MemoryBarrier* MemoryBarriers, UInt32 bufferMemoryBarrierCount, BufferMemoryBarrier* BufferMemoryBarriers, UInt32 imageMemoryBarrierCount, ImageMemoryBarrier* ImageMemoryBarriers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdPipelineBarrier (CommandBuffer commandBuffer, PipelineStageFlags srcStageMask, PipelineStageFlags dstStageMask, DependencyFlags dependencyFlags, UInt32 memoryBarrierCount, MemoryBarrier* MemoryBarriers, UInt32 bufferMemoryBarrierCount, BufferMemoryBarrier* BufferMemoryBarriers, UInt32 imageMemoryBarrierCount, ImageMemoryBarrier* ImageMemoryBarriers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBeginQuery (CommandBuffer commandBuffer, QueryPool queryPool, UInt32 query, QueryControlFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdEndQuery (CommandBuffer commandBuffer, QueryPool queryPool, UInt32 query);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdResetQueryPool (CommandBuffer commandBuffer, QueryPool queryPool, UInt32 firstQuery, UInt32 queryCount);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdWriteTimestamp (CommandBuffer commandBuffer, PipelineStageFlags pipelineStage, QueryPool queryPool, UInt32 query);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdCopyQueryPoolResults (CommandBuffer commandBuffer, QueryPool queryPool, UInt32 firstQuery, UInt32 queryCount, Buffer dstBuffer, DeviceSize dstOffset, DeviceSize stride, QueryResultFlags flags);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdPushConstants (CommandBuffer commandBuffer, PipelineLayout layout, ShaderStageFlags stageFlags, UInt32 offset, UInt32 size, IntPtr Values);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdBeginRenderPass (CommandBuffer commandBuffer, RenderPassBeginInfo* RenderPassBegin, SubpassContents contents);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdNextSubpass (CommandBuffer commandBuffer, SubpassContents contents);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdEndRenderPass (CommandBuffer commandBuffer);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkCmdExecuteCommands (CommandBuffer commandBuffer, UInt32 commandBufferCount, IntPtr pCommandBuffers);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceDisplayPropertiesKHR (PhysicalDevice physicalDevice, out UInt32* PropertyCount, DisplayPropertiesKhr* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceDisplayPlanePropertiesKHR (PhysicalDevice physicalDevice, out UInt32* PropertyCount, DisplayPlanePropertiesKhr* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetDisplayPlaneSupportedDisplaysKHR (PhysicalDevice physicalDevice, UInt32 planeIndex, out UInt32* DisplayCount, out IntPtr pDisplays);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetDisplayModePropertiesKHR (PhysicalDevice physicalDevice, DisplayKhr display, out UInt32* PropertyCount, DisplayModePropertiesKhr* Properties);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDisplayModeKHR (PhysicalDevice physicalDevice, DisplayKhr display, DisplayModeCreateInfoKhr* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pMode);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetDisplayPlaneCapabilitiesKHR (PhysicalDevice physicalDevice, DisplayModeKhr mode, UInt32 planeIndex, DisplayPlaneCapabilitiesKhr* Capabilities);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDisplayPlaneSurfaceKHR (Instance instance, DisplaySurfaceCreateInfoKhr* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pSurface);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSharedSwapchainsKHR (Device device, UInt32 swapchainCount, SwapchainCreateInfoKhr* CreateInfos, AllocationCallbacks* Allocator, out IntPtr pSwapchains);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySurfaceKHR (Instance instance, SurfaceKhr surface, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfaceSupportKHR (PhysicalDevice physicalDevice, UInt32 queueFamilyIndex, SurfaceKhr surface, out Bool32* Supported);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfaceCapabilitiesKHR (PhysicalDevice physicalDevice, SurfaceKhr surface, SurfaceCapabilitiesKhr* SurfaceCapabilities);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfaceFormatsKHR (PhysicalDevice physicalDevice, SurfaceKhr surface, out UInt32* SurfaceFormatCount, SurfaceFormatKhr* SurfaceFormats);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetPhysicalDeviceSurfacePresentModesKHR (PhysicalDevice physicalDevice, SurfaceKhr surface, out UInt32* PresentModeCount, out PresentModeKhr* PresentModes);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateSwapchainKHR (Device device, SwapchainCreateInfoKhr* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pSwapchain);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroySwapchainKHR (Device device, SwapchainKhr swapchain, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkGetSwapchainImagesKHR (Device device, SwapchainKhr swapchain, out UInt32* SwapchainImageCount, out IntPtr pSwapchainImages);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkAcquireNextImageKHR (Device device, SwapchainKhr swapchain, UInt64 timeout, Semaphore semaphore, Fence fence, out UInt32* ImageIndex);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkQueuePresentKHR (Queue queue, PresentInfoKhr* PresentInfo);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Bool32 vkGetPhysicalDeviceWin32PresentationSupportKHR (PhysicalDevice physicalDevice, UInt32 queueFamilyIndex);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern Result vkCreateDebugReportCallbackEXT (Instance instance, DebugReportCallbackCreateInfoExt* CreateInfo, AllocationCallbacks* Allocator, out IntPtr pCallback);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDestroyDebugReportCallbackEXT (Instance instance, DebugReportCallbackExt callback, AllocationCallbacks* Allocator);

		[DllImport (VulkanLibrary, CallingConvention = CallingConvention.Cdecl)]
		internal static unsafe extern void vkDebugReportMessageEXT (Instance instance, DebugReportFlagsExt flags, DebugReportObjectTypeExt objectType, UInt64 @object, UIntPtr location, Int32 messageCode, IntPtr LayerPrefix, IntPtr Message);
	}
}
