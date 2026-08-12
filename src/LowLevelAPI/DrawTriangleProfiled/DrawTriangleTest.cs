using Common;
using System;
using System.Runtime.CompilerServices;
using Evergine.Bindings.Tracy;
using Evergine.Common.Graphics;
using Evergine.Mathematics;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace DrawTriangleProfiled
{
    // DrawTriangleTest instrumented with Tracy through the low-level layer alone. Every
    // added line is tagged ">>> Tracy"; everything else is the original sample. The point
    // being demonstrated: no per-backend code anywhere — QueryHeap, WriteTimestamp and
    // ReadData are the same calls on DX12, Vulkan and Metal, so one instrumentation serves
    // every backend and only the context type label changes.
    public class DrawTriangleTest : VisualTestDefinition
    {
        private Vector4[] vertexData = new Vector4[]
        {
            // TriangleList
            new Vector4(0f, 0.5f, 0.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
            new Vector4(0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
            new Vector4(-0.5f, -0.5f, 0.0f, 1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
        };

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private GraphicsPipelineState pipelineState;
        private Buffer[] vertexBuffers;

        // >>> Tracy: the query heap the engine side owns, and the emission context.
        // The readback array is heap-sized on purpose: Evergine's ReadData(start, count,
        // results) fills results[start..start+count-1] — absolute slot indexing, not
        // compacted to the front. A smaller array is written past its end.
        private QueryHeap queryHeap;
        private GpuProfilerContext tracyGpu;
        private ulong[] readback = new ulong[64];
        private int frameIndex;

        public DrawTriangleTest()
        {
            // >>> Tracy: the run under measurement is the DX12 backend.
            this.GraphicsBackend = GraphicsBackend.DirectX12;
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
        }

        protected override async void InternalLoad()
        {
            // Compile Vertex and Pixel shaders
            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "VertexShader", ShaderStages.Vertex, "VS");
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "FragmentShader", ShaderStages.Pixel, "PS");

            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            var vertexBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Vector4>() * (uint)this.vertexData.Length, BufferFlags.VertexBuffer, ResourceUsage.Default);
            var vertexBuffer = this.graphicsContext.Factory.CreateBuffer(this.vertexData, ref vertexBufferDescription);

            // Prepare Pipeline
            var vertexLayouts = new InputLayouts()
                  .Add(new LayoutDescription()
                              .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.Position))
                              .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.Color)));

            var pipelineDescription = new GraphicsPipelineDescription
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = vertexShader,
                    PixelShader = pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.CullBack,
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.pipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
            this.commandQueue = this.graphicsContext.Factory.CreateCommandQueue();

            var swapChainDescription = this.swapChain?.SwapChainDescription;
            var width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : this.surface.Width;
            var height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : this.surface.Height;

            this.viewports = new Viewport[1];
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors = new Rectangle[1];
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);

            this.vertexBuffers = new Buffer[1];
            this.vertexBuffers[0] = vertexBuffer;

            // >>> Tracy: a 64-slot timestamp heap. The GPU context needs one raw timestamp
            // and the tick period; one throwaway query at load time provides the former.
            Profiler.SetThreadName("main");

            var queryDesc = new QueryHeapDescription { Type = QueryType.Timestamp, QueryCount = 64 };
            this.queryHeap = this.graphicsContext.Factory.CreateQueryHeap(ref queryDesc);

            var boot = this.commandQueue.CommandBuffer();
            boot.Begin();
            boot.WriteTimestamp(this.queryHeap, 0);
            boot.End();
            boot.Commit();
            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();

            var t0 = new ulong[1];
            this.queryHeap.ReadData(0, 1, t0);

            float periodNs = 1e9f / this.graphicsContext.TimestampFrequency;
            this.tracyGpu = GpuProfilerContext.Create(
                "DrawTriangle GPU",
                TracyGpuContextType.Direct3D12,
                (long)t0[0],
                periodNs,
                queryCapacity: 64);

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            using var cpuZone = Profiler.BeginZone("frame");                       // >>> Tracy

            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();

            // >>> Tracy: open the GPU zone at record time; the two WriteTimestamp calls
            // are what the driver fills with the real GPU clock.
            var gpuZone = this.tracyGpu.BeginZone("triangle pass");
            commandBuffer.WriteTimestamp(this.queryHeap, gpuZone.BeginQueryId);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.All, Color.CornflowerBlue));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);
            commandBuffer.SetGraphicsPipelineState(this.pipelineState);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);

            commandBuffer.Draw((uint)this.vertexData.Length / 2);

            commandBuffer.EndRenderPass();

            commandBuffer.WriteTimestamp(this.queryHeap, gpuZone.EndQueryId);      // >>> Tracy
            gpuZone.End();                                                         // >>> Tracy

            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();

            // >>> Tracy: readback. This sample waits idle so results are ready in-frame;
            // a real engine drains N frames later and treats a false return as "retry".
            // Reading the whole heap keeps this correct when the id ring wraps and a
            // zone's two slots stop being contiguous.
            if (this.queryHeap.ReadData(0, 64, this.readback))
            {

                this.tracyGpu.SubmitTime(gpuZone.BeginQueryId, (long)this.readback[gpuZone.BeginQueryId]);
                this.tracyGpu.SubmitTime(gpuZone.EndQueryId, (long)this.readback[gpuZone.EndQueryId]);
            }

            // >>> Tracy: uncalibrated context — re-anchor the GPU clock now and then.
            if (++this.frameIndex % 240 == 0)
            {
                this.tracyGpu.TimeSync((long)this.readback[1]);
            }

            Profiler.FrameMark();                                                  // >>> Tracy
        }
    }
}
