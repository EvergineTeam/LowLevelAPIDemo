using Common;
using System;
using WaveEngine.Common.Graphics;
using WaveEngine.Mathematics;
using Buffer = WaveEngine.Common.Graphics.Buffer;
using WaveEngine.Common.Graphics.VertexFormats;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DrawInstancingIndirect
{
    public class DrawInstancingIndirectTest : VisualTestDefinition
    {
        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private GraphicsPipelineState pipelineState;
        private Buffer[] vertexBuffers;
        private Buffer indexBuffer;
        private Buffer constantBuffer;
        private ResourceSet resourceSet;
        private float time;

        private int[] PlantsTextures = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 0, 11 };

        const uint N_INSTANCES = 2048;

        [StructLayout(LayoutKind.Explicit, Size = 68)]
        struct PerInstanceData
        {
            [FieldOffset(0)]
            public Matrix4x4 world;

            [FieldOffset(64)]
            public int texIndex;
        }
        PerInstanceData[] perInstanceData;

        // Per-instance data block
        [StructLayout(LayoutKind.Explicit, Size = 64)]
        struct VertexData
        {
            [FieldOffset(0)]
            public Matrix4x4 projView;
        }
        VertexData vertexData;

        struct IndirectCall
        {
            public uint indexCountPerInstance;
            public uint instanceCount;
            public uint startIndexLocation;
            public int baseVertexLocation;
            public uint startInstanceLocation;
        }
        IndirectCall[] indirectCalls;
        Buffer indirectCallsBuffer;

        public DrawInstancingIndirectTest()
        {
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
        }

        Random r = new Random();
        float RandomFloat(double min, double max)
        {
            double d = r.NextDouble();
            return (float)(min + d * (max - min));
        }

        protected override async void InternalLoad()
        {
            // Compile Vertex and Pixel shaders
            var vertexShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "VertexShader", ShaderStages.Vertex, "VS");
            var pixelShaderDescription = await this.assetsDirectory.ReadAndCompileShader(this.graphicsContext, "HLSL", "FragmentShader", ShaderStages.Pixel, "PS");

            var vertexShader = this.graphicsContext.Factory.CreateShader(ref vertexShaderDescription);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            // Load gltf mesh 
            Buffer vertexBuffer;
            var gltf = new GLTFLoader(this.assetsDirectory, "Resources/plants.gltf");
            {
                var model = gltf.model;

                // Index Buffer
                var indexPointer = gltf.Buffers[0].bufferPointer;
                var indexBufferDescription = new BufferDescription((uint)model.BufferViews[0].ByteLength, BufferFlags.IndexBuffer, ResourceUsage.Default);
                this.indexBuffer = this.graphicsContext.Factory.CreateBuffer(indexPointer, ref indexBufferDescription);

                // Vertex Buffer (we can't use gltf.Buffers[0].bufferBytes directly because it contains gaps between meshes)
                int vBufferSize = 0;
                for (int i = 0; i < model.BufferViews.Length - 1; ++i)
                {
                    vBufferSize += model.BufferViews[i + 1].ByteLength;
                }
                byte[] vBuffer = new byte[vBufferSize];

                int idx = 0;
                indirectCalls = new IndirectCall[model.BufferViews.Length - 1];
                for (uint i = 0; i < model.BufferViews.Length - 1; ++i)
                {
                    var bufferView = model.BufferViews[i + 1];
                    var accessor = model.Accessors[i * 4];

                    indirectCalls[i] = new IndirectCall
                    {
                        indexCountPerInstance = (uint)accessor.Count,
                        instanceCount = N_INSTANCES,
                        startIndexLocation = (uint)accessor.ByteOffset / sizeof(ushort),
                        baseVertexLocation = idx / 32,
                        startInstanceLocation = i * N_INSTANCES
                    };

                    Array.Copy(gltf.Buffers[0].bufferBytes, bufferView.ByteOffset, vBuffer, idx, bufferView.ByteLength);
                    idx += bufferView.ByteLength;
                }

                var vertexBufferDescription = new BufferDescription((uint)vBuffer.Length, BufferFlags.VertexBuffer, ResourceUsage.Default);
                vertexBuffer = this.graphicsContext.Factory.CreateBuffer(vBuffer, ref vertexBufferDescription);

                var indirectCallsBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<IndirectCall>() * (uint)indirectCalls.Length, BufferFlags.IndirectBuffer, ResourceUsage.Default);
                indirectCallsBuffer = this.graphicsContext.Factory.CreateBuffer(indirectCalls, ref indirectCallsBufferDescription);
            }

            // Create per instance data (world mtx + texIndex)
            perInstanceData = new PerInstanceData[N_INSTANCES * indirectCalls.Length];
            int k = 0;
            for (int meshIdx = 0; meshIdx < indirectCalls.Length; ++meshIdx)
            {
                for (int instance = 0; instance < N_INSTANCES; ++instance)
                {
                    float radius = RandomFloat(0, 5000);
                    float ang = RandomFloat(0, Math.PI * 2.0f);

                    perInstanceData[k].world = Matrix4x4.CreateFromTRS(
                        new Vector3(radius * (float)Math.Cos(ang), 0.0f, radius * (float)Math.Sin(ang)),
                        Quaternion.CreateFromAxisAngle(Vector3.Up, RandomFloat(-Math.PI, Math.PI)),
                        Vector3.One * RandomFloat(0.5f, 2.0f)
                    ); ;
                    perInstanceData[k].texIndex = meshIdx;

                    k++;
                }
            }
            var instanceBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<PerInstanceData>() * N_INSTANCES * (uint)indirectCalls.Length, BufferFlags.VertexBuffer, ResourceUsage.Default);
            var instanceBuffer = this.graphicsContext.Factory.CreateBuffer(perInstanceData, ref instanceBufferDescription);

            this.vertexBuffers = new Buffer[] { vertexBuffer, instanceBuffer };

            var vertexLayouts = new InputLayouts()
            .Add(new LayoutDescription()
                .Add(new ElementDescription(ElementFormat.Float3, ElementSemanticType.Position))
                .Add(new ElementDescription(ElementFormat.Float3, ElementSemanticType.Normal))
                .Add(new ElementDescription(ElementFormat.Float2, ElementSemanticType.TexCoord, 0))
            )
            .Add(new LayoutDescription(VertexStepFunction.PerInstanceData, 1)
                .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.TexCoord, 1))
                .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.TexCoord, 2))
                .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.TexCoord, 3))
                .Add(new ElementDescription(ElementFormat.Float4, ElementSemanticType.TexCoord, 4))
                .Add(new ElementDescription(ElementFormat.Int, ElementSemanticType.TexCoord, 5))
            );

            // Constant Buffers
            var constantBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<VertexData>(), BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.constantBuffer = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            // Create Texture from file
            Texture texture2DArray = null;
            using (var stream = this.assetsDirectory.Open("Resources/texturearray_plants_8888_unorm.ktx"))
            {
                if (stream != null)
                {
                    VisualTests.LowLevel.Images.Image image = VisualTests.LowLevel.Images.Image.Load(stream);
                    var textureDescription = image.TextureDescription;
                    texture2DArray = graphicsContext.Factory.CreateTexture(image.DataBoxes, ref textureDescription);
                }
            }

            //Sampler
            var samplerDescription = SamplerStates.LinearWrap;
            var samplerState = this.graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex),
                    new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                    new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));
            ResourceLayout resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDescription);

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(resourceLayout, this.constantBuffer, texture2DArray, samplerState);
            this.resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            // Prepare Pipeline
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
                    RasterizerState = RasterizerStates.None,
                    BlendState = BlendStates.Opaque,
                    DepthStencilState = DepthStencilStates.ReadWrite,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.pipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
            this.commandQueue = this.graphicsContext.Factory.CreateCommandQueue();

            this.viewports = new Viewport[1];
            this.scissors = new Rectangle[1];

            var width = this.frameBuffer.Width;
            var height = this.frameBuffer.Height;
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            // Update
            this.time += (float)gameTime.TotalSeconds;
            Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.frameBuffer.Width / this.frameBuffer.Height, 1.0f, 20000f);
            Matrix4x4 view = Matrix4x4.CreateRotationY(this.time * 0.03f) * Matrix4x4.CreateLookAt(new Vector3(0, 2000, 7000), new Vector3(0, 0, 2500), Vector3.UnitY);
            this.vertexData.projView = Matrix4x4.Multiply(view, proj);

            // Draw
            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();
            commandBuffer.UpdateBufferData(this.constantBuffer, ref this.vertexData);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, ClearValue.Default);
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);
            commandBuffer.SetGraphicsPipelineState(this.pipelineState);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);
            commandBuffer.SetIndexBuffer(this.indexBuffer);
            commandBuffer.SetResourceSet(this.resourceSet);

            // Using Instance
            /*for (uint i = 0; i < indirectCalls.Length; ++i)
            {
                IndirectCall indirectCall = indirectCalls[i];
                commandBuffer.DrawIndexedInstanced(indirectCall.indexCountPerInstance, indirectCall.instanceCount, indirectCall.startIndexLocation, (uint)indirectCall.baseVertexLocation, indirectCall.startInstanceLocation);
            }*/

            // Using Instance Indirect
            commandBuffer.DrawIndexedInstancedIndirect(indirectCallsBuffer, 0, (uint)indirectCalls.Length, (uint)Unsafe.SizeOf<IndirectCall>());

            commandBuffer.EndRenderPass();
            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }
    }
}
