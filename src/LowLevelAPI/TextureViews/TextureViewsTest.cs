// Copyright © Plain Concepts S.L.U. All rights reserved. Use is subject to license terms.

using System;
using Common;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Evergine.Common.Graphics;
using Evergine.Framework;
using Evergine.Common.Graphics.VertexFormats;
using Evergine.Mathematics;
using Buffer = Evergine.Common.Graphics.Buffer;
using System.Threading.Tasks;

namespace TextureViews
{
    public class TextureViewsTest : VisualTestDefinition
    {
        const uint TEXTURE_RESOLUTION = 1024;

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue graphicsCommandQueue;
        private GraphicsPipelineState graphicsPipeline;
        private ResourceLayout graphicsResourceLayout;

        private CommandQueue computeCommandQueue;
        private ComputePipelineState computePipelineState;
        private ResourceLayout computeResourceLayout;

        private ResourceSet computeResourceSet;
        private Buffer computeConstantBuffer;

        private Texture facesTexture;
        private TextureView facesStoreTextureView;

        private Buffer cubeVertexBuffer;
        private Buffer cubeIndexBuffer;
        private Buffer cubeConstantBuffer;
        private TextureView cubeView;
        private SamplerState cubeSampler;
        private ResourceSet cubeResourceSet;

        private Matrix4x4 modelMtx;
        private Matrix4x4 viewMtx;
        private Matrix4x4 projMtx;
        private float time;

        public TextureViewsTest()
        {
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
        }

        private void createFacesTexture()
        {
            var facesTextureDesc = new TextureDescription
            {
                Type = TextureType.Texture2DArray,
                Format = PixelFormat.R8G8B8A8_UNorm_SRgb,
                Width = TEXTURE_RESOLUTION,
                Height = TEXTURE_RESOLUTION,
                Depth = 1,
                ArraySize = 6,
                MipLevels = 1,
                Flags = TextureFlags.ShaderResource | TextureFlags.UnorderedAccess,
            };
            this.facesTexture = this.graphicsContext.Factory.CreateTexture(ref facesTextureDesc, "Faces");
            // create a view for storage because UAV doesn't support sRGB
            this.facesStoreTextureView = this.graphicsContext.Factory.CreateTextureView(this.facesTexture, pixelFormat: facesTextureDesc.Format.WithoutGamma());
        }

        private async Task createComputePipeline()
        {
            var shaderDesc = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "ComputeFaces", "ComputeFaces", ShaderStages.Compute, "CS");
            var shader = this.graphicsContext.Factory.CreateShader(ref shaderDesc);

            ResourceLayoutDescription layoutDesc = new ResourceLayoutDescription(
                new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Compute),
                new LayoutElementDescription(0, ResourceType.TextureViewReadWrite, ShaderStages.Compute));

            this.computeResourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDesc);

            ComputePipelineDescription pipelineDescription = new ComputePipelineDescription()
            {
                shaderDescription = new ComputeShaderStateDescription()
                {
                    ComputeShader = shader,
                },
                ResourceLayouts = new[] { this.computeResourceLayout },
                ThreadGroupSizeX = 8,
                ThreadGroupSizeY = 8,
                ThreadGroupSizeZ = 1
            };

            this.computePipelineState = this.graphicsContext.Factory.CreateComputePipeline(ref pipelineDescription);
            this.computeCommandQueue = this.graphicsContext.Factory.CreateCommandQueue(CommandQueueType.Graphics);
        }

        private void updateMatrices(float dt)
        {
            this.modelMtx = Matrix4x4.CreateFromYawPitchRoll(0.1f * this.time, 0.2f * this.time, 0.3f * this.time);
            this.viewMtx = Matrix4x4.CreateLookAt(new Vector3(0, 0, 3.3f), new Vector3(0, 0, 0), Vector3.UnitY);
            this.projMtx = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.frameBuffer.Width / (float)this.frameBuffer.Height, 0.1f, 100f, reverseDepthBuffer: true);
        }

        private async Task createGraphicsPipeline()
        {
            var vertexShaderDesc = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "VertexShader", ShaderStages.Vertex, "VS");
            var pixelShaderDesc = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "FragmentShader", ShaderStages.Pixel, "PS");

            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDesc);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDesc);

            var vertexLayouts = new InputLayouts()
                .Add(new LayoutDescription()
                    .Add(new ElementDescription(ElementFormat.Float3, ElementSemanticType.Position)));

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex),
                    new LayoutElementDescription(0, ResourceType.TextureView, ShaderStages.Pixel),
                    new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));
            this.graphicsResourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDescription);

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                ResourceLayouts = new[] { this.graphicsResourceLayout },
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
            this.graphicsPipeline = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
            this.graphicsCommandQueue = this.graphicsContext.Factory.CreateCommandQueue();
        }

        private void createComputeResourceSet()
        {
            var gfxFactory = this.graphicsContext.Factory;

            var bufferDesc = new BufferDescription((uint)Unsafe.SizeOf<Vector4>(), BufferFlags.ConstantBuffer, ResourceUsage.Dynamic);
            this.computeConstantBuffer = gfxFactory.CreateBuffer(ref bufferDesc);

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(this.computeResourceLayout, this.computeConstantBuffer, this.facesStoreTextureView);
            this.computeResourceSet = gfxFactory.CreateResourceSet(ref resourceSetDescription);
        }

        private void createCube()
        {
            Span<Vector3> vertices = stackalloc Vector3[8];
            for (int i = 0; i < 8; i++)
            {
                float x = (i & 0b001) != 0 ? -1 : +1;
                float y = (i & 0b010) != 0 ? -1 : +1;
                float z = (i & 0b100) != 0 ? -1 : +1;
                vertices[i] = new Vector3(x, y, z);
            }

            Span<uint> indices = stackalloc uint[6 * 6];
            {
                int i = 0;
                uint[] quadInds = new uint[]
                {
                    0b000, 0b100, 0b110, 0b010, // -X
                    0b001, 0b011, 0b111, 0b101, // +X
                    0b000, 0b001, 0b101, 0b100, // -Y
                    0b010, 0b110, 0b111, 0b011, // +Y
                    0b000, 0b010, 0b011, 0b001, // -Z
                    0b100, 0b101, 0b111, 0b110, // +Z
                };
                for (int faceInd = 0; faceInd < 6; faceInd++)
                {
                    indices[i++] = quadInds[4 * faceInd + 0];
                    indices[i++] = quadInds[4 * faceInd + 1];
                    indices[i++] = quadInds[4 * faceInd + 2];
                    indices[i++] = quadInds[4 * faceInd + 0];
                    indices[i++] = quadInds[4 * faceInd + 2];
                    indices[i++] = quadInds[4 * faceInd + 3];
                }
            }

            var gfxFactory = this.graphicsContext.Factory;

            var vertexBufferDescription = new BufferDescription((uint)MemoryMarshal.AsBytes(vertices).Length, BufferFlags.VertexBuffer, ResourceUsage.Immutable);
            this.cubeVertexBuffer = gfxFactory.CreateBuffer((ReadOnlySpan<Vector3>)vertices, ref vertexBufferDescription, "cube_vertex_buffer");

            var indexBufferDescription = new BufferDescription((uint)MemoryMarshal.AsBytes(indices).Length, BufferFlags.IndexBuffer, ResourceUsage.Immutable);
            this.cubeIndexBuffer = gfxFactory.CreateBuffer((ReadOnlySpan<uint>)indices, ref indexBufferDescription, "cube_index_buffer");

            // here we create a cube TextureView of the facesTextures, which is a texture array
            this.cubeView = gfxFactory.CreateTextureView(this.facesTexture, viewType: TextureType.TextureCube);

            var samplerDesc = SamplerStates.LinearWrap;
            this.cubeSampler = this.graphicsContext.Factory.CreateSamplerState(ref samplerDesc);

            var constantBufferDesc = new BufferDescription((uint)Unsafe.SizeOf<Matrix4x4>(), BufferFlags.ConstantBuffer, ResourceUsage.Dynamic);
            this.cubeConstantBuffer = gfxFactory.CreateBuffer(ref constantBufferDesc);

            var resourceSetDec = new ResourceSetDescription(this.graphicsResourceLayout, this.cubeConstantBuffer, this.cubeView, this.cubeSampler);
            this.cubeResourceSet = gfxFactory.CreateResourceSet(ref resourceSetDec);
        }

        private void initViewportsAndScissors()
        {
            var swapChainDescription = this.swapChain?.SwapChainDescription;
            var width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : this.surface.Width;
            var height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : this.surface.Height;
            this.viewports = new Viewport[] { new Viewport(0, 0, width, height) };
            this.scissors = new Rectangle[] { new Rectangle(0, 0, (int)width, (int)height) };
        }

        protected override async void InternalLoad()
        {
            this.initViewportsAndScissors();
            await this.createGraphicsPipeline();
            await this.createComputePipeline();
            this.createFacesTexture();
            this.createComputeResourceSet();
            this.createCube();

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            this.time += (float)gameTime.TotalSeconds;

            { // compute
                var commandBuffer = this.computeCommandQueue.CommandBuffer();
                commandBuffer.Begin();

                Vector4 constantBufferData = new Vector4(this.time, (float)TEXTURE_RESOLUTION, 0, 0);
                commandBuffer.UpdateBufferData(this.computeConstantBuffer, ref constantBufferData);
                commandBuffer.SetComputePipelineState(this.computePipelineState);
                commandBuffer.SetResourceSet(this.computeResourceSet);
                commandBuffer.Dispatch3D(TEXTURE_RESOLUTION, TEXTURE_RESOLUTION, 6, 8, 8, 1);

                commandBuffer.End();
                commandBuffer.Commit();

                this.computeCommandQueue.Submit();
                this.computeCommandQueue.WaitIdle();
            }

            this.updateMatrices((float)gameTime.TotalSeconds);

            { // graphics
                var commandBuffer = this.graphicsCommandQueue.CommandBuffer();
                commandBuffer.Begin();

                var modelViewProj = Matrix4x4.Multiply(modelMtx, Matrix4x4.Multiply(this.viewMtx, this.projMtx));
                commandBuffer.UpdateBufferData(this.cubeConstantBuffer, ref modelViewProj);

                RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.All, new Color(10, 10, 20)));
                commandBuffer.BeginRenderPass(ref renderPassDescription);

                commandBuffer.SetViewports(this.viewports);
                commandBuffer.SetScissorRectangles(this.scissors);
                commandBuffer.SetGraphicsPipelineState(this.graphicsPipeline);
                commandBuffer.SetResourceSet(this.cubeResourceSet);
                commandBuffer.SetVertexBuffer(0, this.cubeVertexBuffer, 0);
                commandBuffer.SetIndexBuffer(this.cubeIndexBuffer, IndexFormat.UInt32);
                commandBuffer.DrawIndexed(6*6);

                commandBuffer.EndRenderPass();
                commandBuffer.End();
                commandBuffer.Commit();

                this.graphicsCommandQueue.Submit();
                this.graphicsCommandQueue.WaitIdle();
            }

        }
    }
}
