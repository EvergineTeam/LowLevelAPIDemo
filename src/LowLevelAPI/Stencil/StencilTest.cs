using Common;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Evergine.Common.Graphics;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Mathematics;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace Stencil
{
    public class StencilTest : VisualTestDefinition
    {
        private VertexPositionColorTexture[] vertexData = new VertexPositionColorTexture[]
        {
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)), // Front
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f, -1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f, -1.0f), Color.Orange, new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f, -1.0f), Color.Orange, new Vector2(1, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f, -1.0f), Color.Orange, new Vector2(0, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), Color.Orange, new Vector2(1, 0)), // BACK
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f,  1.0f), Color.Orange, new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), Color.Orange, new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f,  1.0f), Color.Orange, new Vector2(1, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, -1.0f), Color.Orange, new Vector2(1, 0)), // Top
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f,  1.0f), Color.Orange, new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, -1.0f), Color.Orange, new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f, -1.0f), Color.Orange, new Vector2(1, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)), // Bottom
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), Color.Orange, new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f, -1.0f), Color.Orange, new Vector2(1, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)), // Left
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), Color.Orange, new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f, -1.0f), Color.Orange, new Vector2(1, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),

            new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)), // Right
            new VertexPositionColorTexture(new Vector3(1.0f, -1.0f,  1.0f), Color.Orange, new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, -1.0f), Color.Orange, new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), Color.Orange, new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f, -1.0f), Color.Orange, new Vector2(1, 1)),
        };

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private GraphicsPipelineState pipelineState1;
        private GraphicsPipelineState pipelineState2;
        private Buffer[] vertexBuffers;
        private Buffer constantBuffer0;
        private Buffer constantBuffer1;
        private ResourceSet resourceSet0;
        private ResourceSet resourceSet1;
        private Parameters param;

        private float time;
        private Matrix4x4 viewProj;

        public StencilTest()
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
            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "VertexShader", ShaderStages.Vertex, "VS");
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "FragmentShader", ShaderStages.Pixel, "PS");

            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            var vertexBufferDescription = new BufferDescription((uint)(Unsafe.SizeOf<VertexPositionColorTexture>() * this.vertexData.Length), BufferFlags.VertexBuffer, ResourceUsage.Default);
            var vertexBuffer = this.graphicsContext.Factory.CreateBuffer(this.vertexData, ref vertexBufferDescription);

            // Create Texture from file
            Texture texture2D = null;
            using (var stream = this.assetsDirectory.Open("crate.ktx"))
            {
                if (stream != null)
                {
                    VisualTests.LowLevel.Images.Image image = VisualTests.LowLevel.Images.Image.Load(stream);
                    var textureDescription = image.TextureDescription;
                    texture2D = graphicsContext.Factory.CreateTexture(image.DataBoxes, ref textureDescription);
                }
            }

            var samplerDescription = SamplerStates.LinearClamp;
            var samplerState = this.graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            var view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), Vector3.UnitY);
            var proj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.frameBuffer.Width / this.frameBuffer.Height, 0.1f, 100f);
            this.viewProj = Matrix4x4.Multiply(view, proj);

            // Constant Buffer
            this.param.IsTextured = false;
            this.param.WorldViewProjection = Matrix4x4.Identity;
            var constantBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Parameters>(), BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.constantBuffer0 = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);
            this.constantBuffer1 = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            // Prepare Pipeline
            var vertexLayouts = new InputLayouts()
                  .Add(VertexPositionColorTexture.VertexFormat);

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex | ShaderStages.Pixel),
                    new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                    new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));

            ResourceLayout resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDescription);

            ResourceSetDescription resourceSetDescription0 = new ResourceSetDescription(resourceLayout, this.constantBuffer0, texture2D, samplerState);
            this.resourceSet0 = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription0);

            ResourceSetDescription resourceSetDescription1 = new ResourceSetDescription(resourceLayout, this.constantBuffer1, texture2D, samplerState);
            this.resourceSet1 = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription1);

            var depthStencilState1 = DepthStencilStates.None;
            depthStencilState1.StencilEnable = true;
            depthStencilState1.DepthEnable = true;
            depthStencilState1.StencilWriteMask = 0xff;
            depthStencilState1.FrontFace.StencilFunction = ComparisonFunction.Always;
            depthStencilState1.FrontFace.StencilPassOperation = StencilOperation.IncrementSaturation;

            GraphicsPipelineDescription pipelineDescription1 = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
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
                    DepthStencilState = depthStencilState1,
                    StencilReference = 0,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.pipelineState1 = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription1);

            var depthStencilState2 = DepthStencilStates.None;
            depthStencilState2.StencilEnable = true;
            depthStencilState2.DepthEnable = true;
            ////depthStencilState2.StencilWriteMask = 0x00;
            depthStencilState2.FrontFace.StencilFunction = ComparisonFunction.NotEqual;

            var pipelineDescription2 = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
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
                    DepthStencilState = depthStencilState2,
                    StencilReference = 1,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.pipelineState2 = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription2);

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

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            // Update
            this.time += (float)gameTime.TotalSeconds;
            var worldViewProj = Matrix4x4.CreateRotationX(this.time) * Matrix4x4.CreateRotationY(this.time * 2) * Matrix4x4.CreateRotationZ(this.time * .7f) * this.viewProj;
            this.param.WorldViewProjection = worldViewProj;
            this.param.IsTextured = true;
            this.graphicsContext.UpdateBufferData(this.constantBuffer0, ref this.param);

            worldViewProj = Matrix4x4.CreateRotationX(this.time) * Matrix4x4.CreateRotationY(this.time * 2) * Matrix4x4.CreateRotationZ(this.time * .7f) * Matrix4x4.CreateScale(1.04f) * this.viewProj;
            this.param.WorldViewProjection = worldViewProj;
            this.param.IsTextured = false;
            this.graphicsContext.UpdateBufferData(this.constantBuffer1, ref this.param);

            // Draw
            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.All, Color.CornflowerBlue));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);
            commandBuffer.SetGraphicsPipelineState(this.pipelineState1);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);
            commandBuffer.SetResourceSet(this.resourceSet0);

            commandBuffer.Draw((uint)this.vertexData.Length);

            commandBuffer.SetGraphicsPipelineState(this.pipelineState2);
            commandBuffer.SetResourceSet(this.resourceSet1);
            commandBuffer.Draw((uint)this.vertexData.Length);

            commandBuffer.EndRenderPass();
            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }

        [StructLayout(LayoutKind.Explicit, Size = 80)]
        private struct Parameters
        {
            [FieldOffset(0)]
            public bool IsTextured;

            [FieldOffset(16)]
            public Matrix4x4 WorldViewProjection;
        }
    }
}
