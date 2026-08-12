using Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Evergine.Bindings.Tracy;
using Evergine.Common.Graphics;
using Evergine.Mathematics;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace DrawTriangleProfiled
{
    // DrawTriangleTest instrumented with Tracy through the low-level layer alone. Every
    // added line is tagged ">>> Tracy"; everything else is the original sample. One
    // instrumentation, every backend: WriteTimestamp and ReadData are the same calls
    // whatever the API, and only the context type labels the track in the viewer.
    //
    // Measured on this machine (AMD Radeon 890M, 120 Hz, EVERGINE_BACKEND selects):
    //
    //   DirectX12  2314 zones  triangle pass mean 164 us (p50 144, p90 202)
    //   Vulkan     1862 zones  triangle pass mean  73 us (p50  54, p90 102)
    //   DirectX11  unusable, see below
    //   OpenGL     the sample cannot run, see below
    //
    // Two backends do not produce a measurement, and neither reason is the profiler's:
    //
    // DirectX11 resolves 2.8% of its queries. DX11QueryHeap.ReadData asks the driver with
    // AsyncGetDataFlags.DoNotFlush, so a query that is not already resolved reports "not
    // ready" and the FIFO stalls; by the time the ring wraps, the slots are reused and the
    // few pairs that do come back mix a begin from one frame with an end from another. The
    // giveaway is in the numbers: 267 ms per zone with 0.04 ms of deviation, which is a
    // constant offset wearing a duration's clothes rather than anything the GPU did.
    //
    // OpenGL crashes with 0xC0000005 during startup, with no profiler involved at all —
    // reproduced by running the untouched DrawTriangle sample forced to that backend.
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

        private const int QueryCapacity = 64;

        // >>> Tracy: the query heap the engine side owns, and the emission context.
        //
        // The readback array is heap-sized, and `readbackIsAbsolute` exists because
        // ReadData(start, count, results) does NOT agree across backends, measured on
        // Evergine.Common 2026.5.26: DirectX12 and DirectX11 write into
        // results[start..start+count-1] (absolute slot indexing) while Vulkan writes into
        // results[0..count-1] (compacted). One convention has to be detected rather than
        // assumed; the probe is in InternalLoad.
        //
        // Timestamps are also not ready in the same frame on every backend, so resolved
        // pairs are drained from a FIFO instead of read inline — which is what a real
        // engine does anyway.
        private QueryHeap queryHeap;
        private GpuProfilerContext tracyGpu;
        private ulong[] readback = new ulong[QueryCapacity];
        private bool readbackIsAbsolute = true;
        private readonly Queue<(ushort Begin, ushort End)> pending = new();
        private int frameIndex;

        // >>> Tracy: TRACY_GPU=0 keeps the CPU zones and drops the GPU side, so a backend
        // that cannot do timestamp queries can still be profiled and compared.
        private static readonly bool GpuEnabled = Environment.GetEnvironmentVariable("TRACY_GPU") != "0";

        public DrawTriangleTest()
        {
            // >>> Tracy: the backend under measurement. Settable from the environment so the
            // same binary can be run against each one in turn — the whole point of the
            // low-level route is that nothing else in this file changes.
            this.GraphicsBackend = Enum.TryParse<GraphicsBackend>(
                Environment.GetEnvironmentVariable("EVERGINE_BACKEND"), true, out var backend)
                ? backend
                : GraphicsBackend.DirectX12;
        }

        // >>> Tracy: the context type only labels the track in the viewer.
        private static TracyGpuContextType ContextTypeFor(GraphicsBackend backend) => backend switch
        {
            GraphicsBackend.DirectX11 => TracyGpuContextType.Direct3D11,
            GraphicsBackend.DirectX12 => TracyGpuContextType.Direct3D12,
            GraphicsBackend.OpenGL => TracyGpuContextType.OpenGl,
            GraphicsBackend.Vulkan => TracyGpuContextType.Vulkan,
            _ => TracyGpuContextType.Custom,
        };

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

            if (!GpuEnabled)
            {
                Console.WriteLine($"backend={this.graphicsContext.BackendType} gpu=off");
                this.MarkAsLoaded();
                return;
            }

            var queryDesc = new QueryHeapDescription { Type = QueryType.Timestamp, QueryCount = QueryCapacity };
            this.queryHeap = this.graphicsContext.Factory.CreateQueryHeap(ref queryDesc);

            // Probe: write one timestamp to a slot that is neither 0 nor 1 and see where the
            // value lands. This is what tells absolute indexing from compacted, and it
            // doubles as the context's initial GPU timestamp.
            const uint probeSlot = 5;
            var boot = this.commandQueue.CommandBuffer();
            boot.Begin();
            boot.WriteTimestamp(this.queryHeap, probeSlot);
            boot.End();
            boot.Commit();
            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();

            this.queryHeap.ReadData(probeSlot, 1, this.readback);
            this.readbackIsAbsolute = this.readback[probeSlot] != 0;
            ulong initialTimestamp = this.readbackIsAbsolute ? this.readback[probeSlot] : this.readback[0];

            float periodNs = 1e9f / this.graphicsContext.TimestampFrequency;
            this.tracyGpu = GpuProfilerContext.Create(
                $"DrawTriangle GPU ({this.graphicsContext.BackendType})",
                ContextTypeFor(this.graphicsContext.BackendType),
                (long)initialTimestamp,
                periodNs,
                queryCapacity: QueryCapacity);

            // Recorded so a comparison across backends can tell a real difference from a
            // clock-scaling artefact: the tick frequency is not the same on all of them.
            Profiler.AppInfo($"{this.graphicsContext.BackendType} timestampFrequency={this.graphicsContext.TimestampFrequency} periodNs={periodNs}");
            Console.WriteLine($"backend={this.graphicsContext.BackendType} freq={this.graphicsContext.TimestampFrequency} periodNs={periodNs}");

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            using var cpuZone = Profiler.BeginZone("frame");                       // >>> Tracy

            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();

            // >>> Tracy: open the GPU zone at record time; the two WriteTimestamp calls
            // are what the driver fills with the real GPU clock.
            GpuZone gpuZone = default;
            if (GpuEnabled)
            {
                gpuZone = this.tracyGpu.BeginZone("triangle pass");
                commandBuffer.WriteTimestamp(this.queryHeap, gpuZone.BeginQueryId);
            }

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.All, Color.CornflowerBlue));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);
            commandBuffer.SetGraphicsPipelineState(this.pipelineState);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);

            commandBuffer.Draw((uint)this.vertexData.Length / 2);

            commandBuffer.EndRenderPass();

            if (GpuEnabled)
            {
                commandBuffer.WriteTimestamp(this.queryHeap, gpuZone.EndQueryId);   // >>> Tracy
                gpuZone.End();                                                      // >>> Tracy
            }

            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();

            if (GpuEnabled)
            {
                this.pending.Enqueue((gpuZone.BeginQueryId, gpuZone.EndQueryId));   // >>> Tracy
                this.DrainResolvedTimestamps();                                     // >>> Tracy
            }

            this.frameIndex++;
            Profiler.FrameMark();                                                  // >>> Tracy
        }

        /// <summary>
        /// Hands Tracy every timestamp pair the GPU has finished writing, oldest first.
        /// Draining in order matters: the ids are a ring, so a pair left unresolved while
        /// later ones are submitted would eventually have its slots reused underneath it.
        /// A false return from ReadData is "not ready yet", not an error.
        /// </summary>
        private void DrainResolvedTimestamps()
        {
            while (this.pending.Count > 0)
            {
                var (begin, end) = this.pending.Peek();

                // Two reads of one query each, rather than one read of two: begin and end are
                // adjacent except across a ring wrap, and a single read spanning the wrap
                // would ask for a range that does not exist.
                if (!this.queryHeap.ReadData(begin, 1, this.readback))
                {
                    return;
                }

                long beginTime = (long)this.readback[this.readbackIsAbsolute ? begin : 0];

                if (!this.queryHeap.ReadData(end, 1, this.readback))
                {
                    return;
                }

                long endTime = (long)this.readback[this.readbackIsAbsolute ? end : 0];

                this.pending.Dequeue();
                this.tracyGpu.SubmitTime(begin, beginTime);
                this.tracyGpu.SubmitTime(end, endTime);

                // >>> Tracy: uncalibrated context — re-anchor the GPU clock now and then.
                if (this.frameIndex % 240 == 0)
                {
                    this.tracyGpu.TimeSync(endTime);
                }
            }
        }
    }
}
