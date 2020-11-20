using Common;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Mathematics;
using Buffer = WaveEngine.Common.Graphics.Buffer;

namespace TextureCubeArray
{
    public class CubemapArrayTest : VisualTestDefinition
    {
        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private GraphicsPipelineState pipelineState;
        private Buffer[] vertexBuffers;
        private Buffer indexBuffer;
        private Buffer constantBuffer;
        private ResourceSet resourceSet;

        private Parameters param;
        private uint indexCount;

        private Matrix4x4 view;
        private Matrix4x4 proj;
        private float time;

        public CubemapArrayTest()
        {
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
            this.proj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)width / height, 0.1f, 100f);
        }

        protected override async void InternalLoad()
        {
            // Compile Vertex and Pixel shaders
            CompilerParameters cp = new CompilerParameters()
            {
                CompilationMode = CompilationMode.Debug,
                Profile = GraphicsProfile.Level_11_0,
            };

            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "VertexShader", ShaderStages.Vertex, "VS", cp);
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "FragmentShader", ShaderStages.Pixel, "PS", cp);

            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            // Create Texture from file
            Texture textureCube = null;
            using (var stream = this.assetsDirectory.Open("TextureCubeArrayMips.ktx"))
            {
                if (stream != null)
                {
                    VisualTests.LowLevel.Images.Image image = VisualTests.LowLevel.Images.Image.Load(stream);
                    var textureDescription = image.TextureDescription;
                    textureCube = graphicsContext.Factory.CreateTexture(image.DataBoxes, ref textureDescription);
                }
            }

            SamplerStateDescription samplerDescription = SamplerStates.LinearWrap;
            var samplerState = this.graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            Vector3 cameraPosition = new Vector3(0, 0, 1f);
            this.view = Matrix4x4.CreateLookAt(cameraPosition, new Vector3(0, 0, 0), Vector3.UnitY);
            this.proj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.frameBuffer.Width / (float)this.frameBuffer.Height, 0.1f, 100f);

            // Constant Buffer
            this.param = new Parameters()
            {
                CameraPosition = cameraPosition,
                WorldViewProjection = Matrix4x4.Identity,
                World = Matrix4x4.Identity,
                WorldInverseTranspose = Matrix4x4.Identity,
            };

            var constantBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Parameters>(), BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.constantBuffer = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            // Prepare Pipeline
            var vertexLayouts = new InputLayouts()
                  .Add(VertexPositionNormalTangentTexture.VertexFormat);

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex),
                    new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                    new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));

            ResourceLayout resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDescription);

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(resourceLayout, this.constantBuffer, textureCube, samplerState);

            this.resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            Primitives.Torus(1.0f, 0.3f, 28, out List<VertexPositionNormalTangentTexture> vertexData, out List<ushort> indexData);
            this.indexCount = (uint)indexData.Count;

            var vertexBufferDescription = new BufferDescription((uint)(Unsafe.SizeOf<VertexPositionNormalTangentTexture>() * vertexData.Count), BufferFlags.VertexBuffer, ResourceUsage.Default);
            Buffer vertexBuffer = this.graphicsContext.Factory.CreateBuffer(vertexData.ToArray(), ref vertexBufferDescription);

            var indexBufferDescription = new BufferDescription(sizeof(ushort) * this.indexCount, BufferFlags.IndexBuffer, ResourceUsage.Default);
            this.indexBuffer = this.graphicsContext.Factory.CreateBuffer(indexData.ToArray(), ref indexBufferDescription);

            var pipelineDescription = new GraphicsPipelineDescription()
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

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            // Update
            this.time += (float)gameTime.TotalSeconds;
            var viewProj = Matrix4x4.Multiply(this.view, this.proj);
            var world = Matrix4x4.CreateRotationX(this.time) * Matrix4x4.CreateRotationY(this.time * 2) * Matrix4x4.CreateRotationZ(this.time * .7f);

            var worldInverse = Matrix4x4.Invert(world);
            var worldViewProj = world * viewProj;
            worldInverse = Matrix4x4.Transpose(worldInverse);
            var worldInverseTranspose = worldInverse;
            this.param.World = world;
            this.param.WorldInverseTranspose = worldInverseTranspose;
            this.param.WorldViewProjection = worldViewProj;

            // Draw
            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();
            commandBuffer.UpdateBufferData(this.constantBuffer, ref this.param);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, ClearValue.Default);
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);
            commandBuffer.SetGraphicsPipelineState(this.pipelineState);
            commandBuffer.SetResourceSet(this.resourceSet);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);
            commandBuffer.SetIndexBuffer(this.indexBuffer);

            commandBuffer.DrawIndexed(this.indexCount);

            commandBuffer.EndRenderPass();
            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }

        [StructLayout(LayoutKind.Explicit, Size = 208)]
        private struct Parameters
        {
            [FieldOffset(0)]
            public Matrix4x4 WorldViewProjection;

            [FieldOffset(64)]
            public Matrix4x4 World;

            [FieldOffset(128)]
            public Matrix4x4 WorldInverseTranspose;

            [FieldOffset(192)]
            public Vector3 CameraPosition;
        }
    }
}
