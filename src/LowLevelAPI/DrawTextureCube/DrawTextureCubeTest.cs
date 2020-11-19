using Common;
using System;
using System.Runtime.CompilerServices;
using WaveEngine.Common.Graphics;
using WaveEngine.Common.Graphics.VertexFormats;
using WaveEngine.Mathematics;
using Buffer = WaveEngine.Common.Graphics.Buffer;

namespace DrawTextureCube
{
    public class DrawTextureCubeTest : VisualTestDefinition
    {
        private VertexPositionColorTexture[] vertexData = new VertexPositionColorTexture[]
        {
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f), new Vector2(1, 0)), // Front
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f, -1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f, -1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f), new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f), new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f, -1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f), new Vector2(1, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f, -1.0f), new Color(1.0f, 0.0f, 0.0f, 1.0f), new Vector2(0, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Vector2(1, 0)), // BACK
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f,  1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f,  1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,   1.0f,  1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f,  1.0f), new Color(0.0f, 1.0f, 0.0f, 1.0f), new Vector2(1, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(1, 0)), // Top
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f,  1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f, -1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(1, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), new Color(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(0, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f), new Vector2(1, 0)), // Bottom
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f), new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f,  1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f), new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f,  1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  -1.0f, -1.0f), new Color(1.0f, 1.0f, 0.0f, 1.0f), new Vector2(1, 1)),

            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f), new Vector2(1, 0)), // Left
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f,  1.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f,  1.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f), new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f, -1.0f, -1.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f), new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f, -1.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f), new Vector2(1, 1)),
            new VertexPositionColorTexture(new Vector3(-1.0f,  1.0f,  1.0f), new Color(1.0f, 0.0f, 1.0f, 1.0f), new Vector2(0, 1)),

            new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, -1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0)), // Right
            new VertexPositionColorTexture(new Vector3(1.0f, -1.0f,  1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f, -1.0f, -1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 0)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f,  1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f), new Vector2(0, 1)),
            new VertexPositionColorTexture(new Vector3(1.0f,  1.0f, -1.0f), new Color(0.0f, 1.0f, 1.0f, 1.0f), new Vector2(1, 1)),
        };

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private GraphicsPipelineState pipelineState;
        private Buffer[] vertexBuffers;
        private ResourceLayout resourceLayout;
        private ResourceSet resourceSet;
        private Buffer constantBuffer;

        private Matrix4x4 view;
        private Matrix4x4 proj;
        private float time;

        public DrawTextureCubeTest()
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

            SamplerStateDescription samplerDescription = SamplerStates.LinearClamp;
            var sampler = this.graphicsContext.Factory.CreateSamplerState(ref samplerDescription);

            this.view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 5), new Vector3(0, 0, 0), Vector3.UnitY);
            this.proj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.frameBuffer.Width / (float)this.frameBuffer.Height, 0.1f, 100f);

            // Constant Buffer
            var constantBufferDescription = new BufferDescription(64, BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.constantBuffer = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            // Prepare Pipeline
            var vertexLayouts = new InputLayouts()
                  .Add(VertexPositionColorTexture.VertexFormat);

            var resourceLayoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Vertex),
                    new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                    new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));

            this.resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref resourceLayoutDescription);

            var pipelineDescription = new GraphicsPipelineDescription()
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                InputLayouts = vertexLayouts,
                ResourceLayouts = new[] { this.resourceLayout },
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

            var resourceSetDescription = new ResourceSetDescription(this.resourceLayout, this.constantBuffer, texture2D, sampler);
            this.resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            // Update
            this.time += (float)gameTime.TotalSeconds;
            var viewProj = Matrix4x4.Multiply(this.view, this.proj);
            var worldViewProj = Matrix4x4.CreateRotationX(this.time) * Matrix4x4.CreateRotationY(this.time * 2) * Matrix4x4.CreateRotationZ(this.time * .7f) * viewProj;

            // Draw
            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();

            commandBuffer.UpdateBufferData(this.constantBuffer, ref worldViewProj);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.Target, Color.CornflowerBlue));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetGraphicsPipelineState(this.pipelineState);
            commandBuffer.SetResourceSet(this.resourceSet);
            commandBuffer.SetVertexBuffers(this.vertexBuffers);

            commandBuffer.Draw((uint)this.vertexData.Length);

            commandBuffer.EndRenderPass();
            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }
    }
}
