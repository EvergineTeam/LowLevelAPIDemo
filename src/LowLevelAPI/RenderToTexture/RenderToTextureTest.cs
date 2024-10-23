using Common;
using System;
using System.Runtime.CompilerServices;
using Evergine.Common.Graphics;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Mathematics;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace RenderToTexture
{
    public class RenderToTextureTest : VisualTestDefinition
    {
        private VertexPositionColor[] triangleVertexData = new VertexPositionColor[]
        {
            // TriangleList
            new VertexPositionColor(new Vector3(0f,     0.5f, 0.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f)),
            new VertexPositionColor(new Vector3(0.5f,  -0.5f, 0.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f)),
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f)),
        };

        private VertexPositionTexture[] cubeVertexData = new VertexPositionTexture[]
        {
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1, 0)), // Front
            new VertexPositionTexture(new Vector3(1.0f,  1.0f, -1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(-1.0f,  1.0f, -1.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(1.0f,  1.0f, -1.0f), new Vector2(0, 1)),

            new VertexPositionTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(1, 0)), // BACK
            new VertexPositionTexture(new Vector3(-1.0f,  1.0f,  1.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1.0f,  1.0f,  1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f,   1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f,  1.0f), new Vector2(1, 1)),

            new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(1, 0)), // Top
            new VertexPositionTexture(new Vector3(1.0f, 1.0f,  1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(-1.0f, 1.0f,  1.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f, -1.0f), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f,  1.0f), new Vector2(0, 1)),

            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1, 0)), // Bottom
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f,  1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f,  1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1, 1)),

            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1, 0)), // Left
            new VertexPositionTexture(new Vector3(-1.0f,  1.0f,  1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-1.0f,  1.0f, -1.0f), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(-1.0f,  1.0f,  1.0f), new Vector2(0, 1)),

            new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1, 0)), // Right
            new VertexPositionTexture(new Vector3(1.0f, -1.0f,  1.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1.0f,  1.0f,  1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1.0f,  1.0f,  1.0f), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(1.0f,  1.0f, -1.0f), new Vector2(1, 1)),
        };

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private Rectangle[] rTScissors;
        private CommandQueue commandQueue;

        private FrameBuffer rTFrameBuffer;
        private Viewport[] rTViewports;
        private GraphicsPipelineState trianglePipelineState;
        private ResourceSet triangleResourceSet;
        private Buffer[] triangleVertexBuffers;
        private Buffer triangleConstantBuffer;

        private GraphicsPipelineState cubePipelineState;
        private ResourceSet cubeResourceSet;
        private Buffer[] cubeVertexBuffers;
        private Buffer cubeConstantBuffer;

        private float time;
        private Matrix4x4 triangleViewProj;
        private Matrix4x4 cubeViewProj;

        public RenderToTextureTest()
        {
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
        }

        protected override async void InternalLoad()
        {
            // Compile Vertex and Pixel shaders
            var triangleVertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "TriangleHLSL", "TriangleVertexShader", ShaderStages.Vertex, "VS");
            var trianglePixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "TriangleHLSL", "TriangleFragmentShader", ShaderStages.Pixel, "PS");

            var triangleVertexShader = this.graphicsContext.Factory.CreateShader(ref triangleVertexShaderDescription);
            var trianglePixelShader = this.graphicsContext.Factory.CreateShader(ref trianglePixelShaderDescription);

            // Instantiate Vertex buffer from vertex data
            var triangleVertexBufferDescription = new BufferDescription((uint)(Unsafe.SizeOf<VertexPositionColor>() * this.triangleVertexData.Length), BufferFlags.VertexBuffer, ResourceUsage.Default);
            var triangleVertexBuffer = this.graphicsContext.Factory.CreateBuffer(this.triangleVertexData, ref triangleVertexBufferDescription);

            var triangleView = Matrix4x4.CreateLookAt(new Vector3(0, 0, 2), new Vector3(0, 0, 0), Vector3.UnitY);
            var triangleProj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 512 / 512f, 0.1f, 100f);
            this.triangleViewProj = Matrix4x4.Multiply(triangleView, triangleProj);

            // Constant Buffer
            var triangleconstantBufferDescription = new BufferDescription(64, BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.triangleConstantBuffer = this.graphicsContext.Factory.CreateBuffer(ref triangleconstantBufferDescription);

            // Inputs
            var triangleVertexLayouts = new InputLayouts()
                .Add(VertexPositionColor.VertexFormat);

            ResourceLayoutDescription triangleLayoutDescription = new ResourceLayoutDescription(
                new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex));

            ResourceLayout triangleResourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref triangleLayoutDescription);

            ResourceSetDescription triangleSetDescription = new ResourceSetDescription(triangleResourceLayout, this.triangleConstantBuffer);
            this.triangleResourceSet = this.graphicsContext.Factory.CreateResourceSet(ref triangleSetDescription);

            uint rtSize = 512;

            // Create Render Target FrameBuffer
            var rTColorTargetDescription = new TextureDescription()
            {
                Format = PixelFormat.R8G8B8A8_UNorm,
                Width = rtSize,
                Height = rtSize,
                Depth = 1,
                ArraySize = 1,
                Faces = 1,
                Flags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                CpuAccess = ResourceCpuAccess.None,
                MipLevels = 1,
                Type = TextureType.Texture2D,
                Usage = ResourceUsage.Default,
                SampleCount = TextureSampleCount.None,
            };
            var rTColorTarget = this.graphicsContext.Factory.CreateTexture(ref rTColorTargetDescription);

            var rTDepthTargetDescription = new TextureDescription()
            {
                Format = PixelFormat.D24_UNorm_S8_UInt,
                Width = rtSize,
                Height = rtSize,
                Depth = 1,
                ArraySize = 1,
                Faces = 1,
                Flags = TextureFlags.DepthStencil,
                CpuAccess = ResourceCpuAccess.None,
                MipLevels = 1,
                Type = TextureType.Texture2D,
                Usage = ResourceUsage.Default,
                SampleCount = TextureSampleCount.None,
            };

            var rTDepthTarget = this.graphicsContext.Factory.CreateTexture(ref rTDepthTargetDescription);
            var depthAttachment = new FrameBufferAttachment(rTDepthTarget, 0, 1);
            var colorsAttachment = new[] { new FrameBufferAttachment(rTColorTarget, 0, 1) };
            this.rTFrameBuffer = this.graphicsContext.Factory.CreateFrameBuffer(depthAttachment, colorsAttachment);

            var trianglePipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = triangleVertexLayouts,
                ResourceLayouts = new[] { triangleResourceLayout },
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = triangleVertexShader,
                    PixelShader = trianglePixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.None,
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.None,
                },
                Outputs = this.rTFrameBuffer.OutputDescription,
            };

            this.trianglePipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref trianglePipelineDescription);

            this.triangleVertexBuffers = new Buffer[1];
            this.triangleVertexBuffers[0] = triangleVertexBuffer;

            this.rTViewports = new Viewport[1];
            this.rTViewports[0] = new Viewport(0, 0, rtSize, rtSize);

            this.rTScissors = new Rectangle[1];
            this.rTScissors[0] = new Rectangle(0, 0, (int)rtSize, (int)rtSize);

            // Compile Vertex and Pixel shaders
            var cubeVertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "CubeHLSL", "CubeVertexShader", ShaderStages.Vertex, "VS");
            var cubePixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "CubeHLSL", "CubeFragmentShader", ShaderStages.Pixel, "PS");

            var cubeVertexShader = this.graphicsContext.Factory.CreateShader(ref cubeVertexShaderDescription);
            var cubePixelShader = this.graphicsContext.Factory.CreateShader(ref cubePixelShaderDescription);

            // Instantiate Vertex buffer from vertex data
            var cubeVertexBufferDescription = new BufferDescription((uint)(Unsafe.SizeOf<VertexPositionTexture>() * this.cubeVertexData.Length), BufferFlags.VertexBuffer, ResourceUsage.Default);
            var cubeVertexBuffer = this.graphicsContext.Factory.CreateBuffer(this.cubeVertexData, ref cubeVertexBufferDescription);

            var cubeView = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), Vector3.UnitY);
            var cubeProj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1280 / 720f, 0.1f, 100f);
            this.cubeViewProj = Matrix4x4.Multiply(cubeView, cubeProj);

            // Constant Buffer
            var cubeConstantBufferDescription = new BufferDescription(64, BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.cubeConstantBuffer = this.graphicsContext.Factory.CreateBuffer(ref cubeConstantBufferDescription);

            // Inputs
            InputLayouts cubeVertexLayouts = new InputLayouts()
                .Add(VertexPositionTexture.VertexFormat);

            var samplerDescription = SamplerStates.LinearClamp;
            var samplerState = this.graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            ResourceLayoutDescription cubeLayoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex),
                    new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                    new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));
            ResourceLayout cubeResourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref cubeLayoutDescription);

            ResourceSetDescription cubeSetDescription = new ResourceSetDescription(cubeResourceLayout, this.cubeConstantBuffer, this.rTFrameBuffer.ColorTargets[0].Texture, samplerState);
            this.cubeResourceSet = this.graphicsContext.Factory.CreateResourceSet(ref cubeSetDescription);

            var cubePipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = cubeVertexLayouts,
                ResourceLayouts = new[] { cubeResourceLayout },
                Shaders = new GraphicsShaderStateDescription()
                {
                    VertexShader = cubeVertexShader,
                    PixelShader = cubePixelShader,
                },
                RenderStates = new RenderStateDescription()
                {
                    RasterizerState = RasterizerStates.CullBack,
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.None,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.cubePipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref cubePipelineDescription);

            this.cubeVertexBuffers = new Buffer[1];
            this.cubeVertexBuffers[0] = cubeVertexBuffer;

            this.commandQueue = this.graphicsContext.Factory.CreateCommandQueue();

            var swapChainDescription = this.swapChain?.SwapChainDescription;
            var width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : this.surface.Width;
            var height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : this.surface.Height;

            this.viewports = new Viewport[1];
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors = new Rectangle[1];
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            // Update
            this.time += (float)gameTime.TotalSeconds;
            var triangleWVP = Matrix4x4.CreateRotationY(this.time * 2) * this.triangleViewProj;
            this.graphicsContext.UpdateBufferData(this.triangleConstantBuffer, ref triangleWVP);
            var cubeWVP = Matrix4x4.CreateRotationX(this.time) * Matrix4x4.CreateRotationY(this.time * 2) * Matrix4x4.CreateRotationZ(this.time * .7f) * this.cubeViewProj;
            this.graphicsContext.UpdateBufferData(this.cubeConstantBuffer, ref cubeWVP);

            // Render to texture
            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.rTFrameBuffer, new ClearValue(ClearFlags.Target, Color.CornflowerBlue));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.rTViewports);
            commandBuffer.SetScissorRectangles(this.rTScissors);
            commandBuffer.SetGraphicsPipelineState(this.trianglePipelineState);
            commandBuffer.SetResourceSet(this.triangleResourceSet);
            commandBuffer.SetVertexBuffers(this.triangleVertexBuffers);

            commandBuffer.Draw((uint)this.triangleVertexData.Length);

            commandBuffer.EndRenderPass();

            renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.Target, Color.Black));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetGraphicsPipelineState(this.cubePipelineState);
            commandBuffer.SetResourceSet(this.cubeResourceSet);
            commandBuffer.SetVertexBuffers(this.cubeVertexBuffers);
            commandBuffer.Draw((uint)this.cubeVertexData.Length);

            commandBuffer.EndRenderPass();
            commandBuffer.End();
            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }
    }
}
