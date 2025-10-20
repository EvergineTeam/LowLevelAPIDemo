using Common;
using System;
using System.Runtime.CompilerServices;
using Evergine.Common.Graphics;
using Evergine.Mathematics;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace ComputeTexture
{
    public class ComputeTextureTest : VisualTestDefinition
    {
        private const uint GroupSizeX = 8;
        private const uint GroupSizeY = 8;

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue graphicsCommandQueue;
        private CommandQueue computeCommandQueue;
        private GraphicsPipelineState graphicsPipelineState;
        private ComputePipelineState computePipelineState;
        private ResourceSet resourceSet;
        private ResourceSet computeResourceSet;
        private Buffer constantBuffer;
        private ComputeData computeData;
        private uint width;
        private uint height;

        public struct ComputeData
        {
            public float time;
            public float width;
            public float height;
            public float padding;
        }

        public ComputeTextureTest()
        {
        }

        protected override bool CheckBackendCompatibility()
        {
            return this.GraphicsBackend != GraphicsBackend.OpenGL
                && this.GraphicsBackend != GraphicsBackend.OpenGLES;
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
        }

        protected override async void InternalLoad()
        {
            var swapChainDescription = this.swapChain?.SwapChainDescription;
            this.width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : this.surface.Width;
            this.height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : this.surface.Height;


            // Compute Resources
            CompilerParameters parameters = new CompilerParameters()
            {
                CompilationMode = CompilationMode.None,
                Profile = GraphicsProfile.Level_11_0,
            };

            var computeShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "CS", "ComputeShader", ShaderStages.Compute, "CS", parameters);
            var computeShader = this.graphicsContext.Factory.CreateShader(ref computeShaderDescription);

            var textureDescription = new TextureDescription()
            {
                Type = TextureType.Texture2D,
                Usage = ResourceUsage.Default,
                Flags = TextureFlags.UnorderedAccess | TextureFlags.ShaderResource,
                Format = PixelFormat.R8G8B8A8_UNorm,
                Width = width,
                Height = height,
                Depth = 1,
                MipLevels = 1,
                ArraySize = 1,
                Faces = 1,
                CpuAccess = ResourceCpuAccess.None,
                SampleCount = TextureSampleCount.None,
            };

            Texture texture2D = this.graphicsContext.Factory.CreateTexture(ref textureDescription);

            this.computeData = new ComputeData()
            {
                time = 0,
                width = width,
                height = height,
            };

            var constantBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<ComputeData>(), BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.constantBuffer = this.graphicsContext.Factory.CreateBuffer(ref this.computeData, ref constantBufferDescription);

            ResourceLayoutDescription computeLayoutDescription = new ResourceLayoutDescription(
                new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Compute),
                new LayoutElementDescription(0, ResourceType.TextureReadWrite, ShaderStages.Compute));

            ResourceLayout computeResourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref computeLayoutDescription);

            ResourceSetDescription computeResourceSetDescription = new ResourceSetDescription(computeResourceLayout, this.constantBuffer, texture2D);
            this.computeResourceSet = this.graphicsContext.Factory.CreateResourceSet(ref computeResourceSetDescription);

            var computePipelineDescription = new ComputePipelineDescription()
            {
                shaderDescription = new ComputeShaderStateDescription()
                {
                    ComputeShader = computeShader
                },
                ResourceLayouts = new[] { computeResourceLayout },
                ThreadGroupSizeX = MathHelper.DivideByMultiple(this.width, GroupSizeX),
                ThreadGroupSizeY = MathHelper.DivideByMultiple(this.height, GroupSizeY),
                ThreadGroupSizeZ = 1,
            };

            this.computePipelineState = this.graphicsContext.Factory.CreateComputePipeline(ref computePipelineDescription);

            // Graphics Resources
            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "VertexShader", ShaderStages.Vertex, "VS");
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "FragmentShader", ShaderStages.Pixel, "PS");
            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            var samplerDescription = SamplerStates.LinearClamp;
            var samplerState = this.graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
                   new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                   new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));
            ResourceLayout resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDescription);

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(resourceLayout, texture2D, samplerState);
            this.resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = null,
                ResourceLayouts = new[] { resourceLayout },
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = vertexShader,
                    PixelShader = pixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.CullBack,
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.None,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.graphicsPipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
            this.graphicsCommandQueue = this.graphicsContext.Factory.CreateCommandQueue(CommandQueueType.Graphics);
            this.computeCommandQueue = this.graphicsContext.Factory.CreateCommandQueue(CommandQueueType.Graphics);

            this.viewports = new Viewport[1];
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors = new Rectangle[1];
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            var computeCommandBuffer = this.graphicsCommandQueue.CommandBuffer();

            computeCommandBuffer.Begin();
            this.computeData.time += (float)gameTime.TotalSeconds;
            computeCommandBuffer.UpdateBufferData(this.constantBuffer, ref this.computeData);
            computeCommandBuffer.SetComputePipelineState(this.computePipelineState);
            computeCommandBuffer.SetResourceSet(this.computeResourceSet);
            computeCommandBuffer.Dispatch2D(this.width, this.height, GroupSizeX, GroupSizeY);

            computeCommandBuffer.End();

            computeCommandBuffer.Commit();
            this.computeCommandQueue.Submit();
            this.computeCommandQueue.WaitIdle();

            var graphicsCommandBuffer = this.graphicsCommandQueue.CommandBuffer();
            graphicsCommandBuffer.Begin();

            graphicsCommandBuffer.SetGraphicsPipelineState(this.graphicsPipelineState);
            graphicsCommandBuffer.SetResourceSet(this.resourceSet);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, ClearValue.Default);
            graphicsCommandBuffer.BeginRenderPass(ref renderPassDescription);

            graphicsCommandBuffer.SetViewports(this.viewports);
            graphicsCommandBuffer.SetScissorRectangles(this.scissors);

            graphicsCommandBuffer.Draw(3);

            graphicsCommandBuffer.EndRenderPass();
            graphicsCommandBuffer.End();

            graphicsCommandBuffer.Commit();

            this.graphicsCommandQueue.Submit();
            this.graphicsCommandQueue.WaitIdle();
        }
    }
}
