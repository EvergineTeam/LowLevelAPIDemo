using Common;
using System;
using System.Runtime.CompilerServices;
using Evergine.Common.Graphics;
using Evergine.Mathematics;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace ComputeRayTracing
{
    public class ComputeRayTracingTest : VisualTestDefinition
    {
        private const uint GroupSizeX = 8;
        private const uint GroupSizeY = 8;

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue graphicsCommandQueue;
        private GraphicsPipelineState graphicsPipelineState;
        private ComputePipelineState computePipelineState;
        private ResourceSet resourceSet;
        private ResourceSet computeResourceSet;
        private Buffer constantBuffer;
        private Buffer spheresBuffer;
        private Buffer materialsBuffer;
        private ComputeData computeData;
        private Camera cam;
        private Sphere[] spheres;
        private Material[] materials;
        private Buffer paramsBuffer;
        private Params paramsData;
        private Vector3 lookFrom;
        private Vector3 lookat;
        private uint width;
        private uint height;
        private float dist_to_focus;
        private float aperture;

        public struct Sphere
        {
            public Vector3 Center;
            public float Radius;

            public static Sphere Create(Vector3 center, float radius)
            {
                Sphere s;
                s.Center = center;
                s.Radius = radius;
                return s;
            }
        }

        public enum MaterialType
        {
            Lambertian,
            Metal,
            Dielectric,
        }

        public struct Material
        {
            public Vector3 Albedo;
            public float RefIndex;
            public MaterialType Type;
            public uint padding0;
            public uint padding1;
            public uint padding2;

            public static Material Create(Vector3 albedo, float refIndex, MaterialType type)
            {
                Material m;
                m.Albedo = albedo;
                m.RefIndex = refIndex;
                m.Type = type;
                m.padding0 = 0;
                m.padding1 = 0;
                m.padding2 = 0;
                return m;
            }
        }

        public struct Camera
        {
            public Vector3 Origin;
            public float LensRadius;
            public Vector3 LowerLeftCorner;
            public uint Padding0;
            public Vector3 Horizontal;
            public uint Padding1;
            public Vector3 Vertical;
            public uint Padding2;
            public Vector3 U;
            public uint Padding3;
            public Vector3 V;
            public uint Padding4;
            public Vector3 W;
            public uint Padding5;

            public static Camera Create(Vector3 lookFrom, Vector3 lookat, Vector3 vup, float vfov, float aspect, float aperture, float focus_dist)
            {
                Camera cam = new Camera();
                cam.LensRadius = aperture / 2f;
                float theta = vfov * (float)(Math.PI / 180f);
                float half_height = (float)Math.Tan(theta / 2f);
                float half_width = aspect * half_height;

                cam.Origin = lookFrom;
                cam.W = Vector3.Normalize(lookFrom - lookat);
                cam.U = Vector3.Normalize(Vector3.Cross(vup, cam.W));
                cam.V = Vector3.Cross(cam.W, cam.U);

                cam.LowerLeftCorner = cam.Origin - half_width * focus_dist * cam.U - half_height * focus_dist * cam.V - focus_dist * cam.W;
                cam.Horizontal = 2f * half_width * focus_dist * cam.U;
                cam.Vertical = 2f * half_height * focus_dist * cam.V;

                return cam;
            }
        }

        public struct ComputeData
        {
            public float time;
            public float width;
            public float height;
            public uint framecount;
            public uint samples;
            public uint recursion;
            public uint spherecount;
            public uint padding0;
            public Camera cam;
        }

        public struct Params
        {
            public uint Samples;
            public bool IsPathTracing;
            public uint Padding1;
            public uint Padding2;
        }

        public ComputeRayTracingTest()
        {
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
        }

        private void BasicScene()
        {
            this.lookFrom = new Vector3(12, 5, 3);
            this.lookat = new Vector3(0, 0, 0);
            float dist_to_focus = 12;
            float aperture = 0.2f;
            this.cam = Camera.Create(lookFrom, lookat, Vector3.UnitY, 25f, (float)this.width / this.height, aperture, dist_to_focus);

            spheres = new[]
            {
                Sphere.Create(new Vector3(0, 0.3f, 0), 0.8f),
                Sphere.Create(new Vector3(0, -0.2f,-1), 0.3f),
                Sphere.Create(new Vector3(1.4f, -0.1f, 1.3f), 0.4f),
                Sphere.Create(new Vector3(0.5f, -0.2f, 1), 0.3f),
                Sphere.Create(new Vector3(-1.5f, 0.1f, -1.6f), 0.6f),
                Sphere.Create(new Vector3(-0.9f, -0.2f, -0.7f), 0.3f),
                Sphere.Create(new Vector3(-1.4f, 0, 1f), 0.5f),
                Sphere.Create(new Vector3(1.3f, 0, -0.9f), 0.5f),
                Sphere.Create(new Vector3(1.6f, 0, 2.3f), 0.5f),
                Sphere.Create(new Vector3(2.1f, 0.2f, 0.1f), 0.7f),

                Sphere.Create(new Vector3(0,-100.5f,-1), 100f),
            };

            materials = new[]
            {
                Material.Create(new Vector3(0.6f, 0.2f, 0.8f), 0, MaterialType.Metal),
                Material.Create(new Vector3(0.8f, 0.1f, 0.1f), 0f, MaterialType.Lambertian),
                Material.Create(new Vector3(0.1f, 0.8f, 0.2f), 0f, MaterialType.Lambertian),
                Material.Create(new Vector3(0.4f, 0.1f, 0.5f), 0f, MaterialType.Lambertian),
                Material.Create(new Vector3(0.8f, 0.8f, 0.1f), 0.6f, MaterialType.Metal),
                Material.Create(new Vector3(0.8f, 0.8f, 0.8f), 0f, MaterialType.Lambertian),
                Material.Create(new Vector3(0.1f, 0.5f, 0.5f), 0f, MaterialType.Lambertian),
                Material.Create(new Vector3(0.8f, 0.1f, 0.5f), 0.2f, MaterialType.Metal),
                Material.Create(new Vector3(0.9f, 0.8f, 0.9f), 0.3f, MaterialType.Metal),
                Material.Create(new Vector3(0.9f, 0.8f, 0.9f), 1.1f, MaterialType.Dielectric),

                Material.Create(new Vector3(0.8f, 0.8f, 0.8f), 0, MaterialType.Lambertian),
            };
        }

        protected override async void InternalLoad()
        {
            var swapChainDescription = this.swapChain?.SwapChainDescription;
            this.width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : this.surface.Width;
            this.height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : this.surface.Height;

            // Create Scene
            BasicScene();

            // Compute Resources
            var spheresBufferDescription = new BufferDescription(
                (uint)(Unsafe.SizeOf<Sphere>() * spheres.Length),
                BufferFlags.UnorderedAccess | BufferFlags.ShaderResource | BufferFlags.BufferStructured,
                ResourceUsage.Default,
                ResourceCpuAccess.None,
                Unsafe.SizeOf<Sphere>());
            spheresBuffer = this.graphicsContext.Factory.CreateBuffer(spheres, ref spheresBufferDescription);

            var materialsBufferDescription = new BufferDescription(
                (uint)(Unsafe.SizeOf<Material>() * spheres.Length),
                BufferFlags.UnorderedAccess | BufferFlags.ShaderResource | BufferFlags.BufferStructured,
                ResourceUsage.Default,
                ResourceCpuAccess.None,
                Unsafe.SizeOf<Material>());
            materialsBuffer = this.graphicsContext.Factory.CreateBuffer(materials, ref materialsBufferDescription);

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
                Width = this.width,
                Height = this.height,
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
                width = this.width,
                height = this.height,
                framecount = 0,
                samples = 25,
                recursion = 5,
                spherecount = (uint)spheres.Length,
                cam = this.cam
            };

            var constantBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<ComputeData>(), BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.constantBuffer = this.graphicsContext.Factory.CreateBuffer(ref this.computeData, ref constantBufferDescription);

            ResourceLayoutDescription computeLayoutDescription = new ResourceLayoutDescription(
                new LayoutElementDescription(0, ResourceType.StructuredBuffer, ShaderStages.Compute),
                new LayoutElementDescription(1, ResourceType.StructuredBuffer, ShaderStages.Compute),
                new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Compute),
                new LayoutElementDescription(0, ResourceType.TextureReadWrite, ShaderStages.Compute));

            ResourceLayout computeResourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref computeLayoutDescription);

            ResourceSetDescription computeResourceSetDescription = new ResourceSetDescription(computeResourceLayout, this.spheresBuffer, this.materialsBuffer, this.constantBuffer, texture2D);
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

            // Set Params
            this.paramsData.Samples = 1;
            this.paramsData.IsPathTracing = false;

            var paramsBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<Params>(), BufferFlags.ConstantBuffer, ResourceUsage.Default);
            this.paramsBuffer = this.graphicsContext.Factory.CreateBuffer(ref paramsBufferDescription);

            ResourceLayoutDescription layoutDescription = new ResourceLayoutDescription(
                   new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Pixel),
                   new LayoutElementDescription(0, ResourceType.Texture, ShaderStages.Pixel),
                   new LayoutElementDescription(0, ResourceType.Sampler, ShaderStages.Pixel));
            ResourceLayout resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref layoutDescription);

            ResourceSetDescription resourceSetDescription = new ResourceSetDescription(resourceLayout, this.paramsBuffer, texture2D, samplerState);
            this.resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            BlendStateDescription blend = BlendStateDescription.Default;

            if (this.paramsData.IsPathTracing)
            {
                blend.RenderTarget0.BlendEnable = true;
                blend.RenderTarget0.SourceBlendColor = Blend.SourceAlpha;
                blend.RenderTarget0.DestinationBlendColor = Blend.InverseSourceAlpha;
            }
            else
            {
                blend = BlendStates.Opaque;
            }

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
                    BlendState = blend,
                    DepthStencilState = DepthStencilStates.Read,
                },
                Outputs = this.frameBuffer.OutputDescription,
            };

            this.graphicsPipelineState = this.graphicsContext.Factory.CreateGraphicsPipeline(ref pipelineDescription);
            this.graphicsCommandQueue = this.graphicsContext.Factory.CreateCommandQueue(CommandQueueType.Graphics);

            this.viewports = new Viewport[1];
            this.viewports[0] = new Viewport(0, 0, this.width, this.height);
            this.scissors = new Rectangle[1];
            this.scissors[0] = new Rectangle(0, 0, (int)this.width, (int)this.height);

            this.MarkAsLoaded();
        }

        private void RotateAround(Vector3 point, Vector3 pivot, float angle, out Vector3 rotatedPoint)
        {
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);

            rotatedPoint = point;
            rotatedPoint.X -= pivot.X;
            rotatedPoint.Z -= pivot.Z;

            rotatedPoint.X = rotatedPoint.X * c - rotatedPoint.Z * s;
            rotatedPoint.Z = rotatedPoint.X * s + rotatedPoint.Z * c;

            rotatedPoint.X += pivot.X;
            rotatedPoint.Z += pivot.Z;
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            var computeCommandBuffer = this.graphicsCommandQueue.CommandBuffer();

            this.computeData.time += (float)gameTime.TotalSeconds;
            this.computeData.framecount++;

            if (!this.paramsData.IsPathTracing)
            {
                RotateAround(this.lookFrom, this.lookat, (float)gameTime.TotalSeconds / 1, out this.lookFrom);
                this.dist_to_focus = (lookFrom - lookat).Length();
                this.aperture = 0.01f;
                this.cam = Camera.Create(lookFrom, lookat, Vector3.UnitY, 25f, (float)this.width / this.height, this.aperture, this.dist_to_focus);
                this.computeData.cam = this.cam;
            }

            computeCommandBuffer.Begin();
            computeCommandBuffer.UpdateBufferData(this.constantBuffer, ref this.computeData);
            computeCommandBuffer.SetComputePipelineState(this.computePipelineState);
            computeCommandBuffer.SetResourceSet(this.computeResourceSet);
            computeCommandBuffer.Dispatch2D(this.width, this.height, GroupSizeX, GroupSizeY);

            computeCommandBuffer.End();

            computeCommandBuffer.Commit();
            this.graphicsCommandQueue.Submit();
            this.graphicsCommandQueue.WaitIdle();

            var graphicsCommandBuffer = this.graphicsCommandQueue.CommandBuffer();
            graphicsCommandBuffer.Begin();

            graphicsCommandBuffer.UpdateBufferData(this.paramsBuffer, ref this.paramsData);

            graphicsCommandBuffer.SetViewports(this.viewports);
            graphicsCommandBuffer.SetScissorRectangles(this.scissors);
            graphicsCommandBuffer.SetGraphicsPipelineState(this.graphicsPipelineState);
            graphicsCommandBuffer.SetResourceSet(this.resourceSet);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, ClearValue.None);
            graphicsCommandBuffer.BeginRenderPass(ref renderPassDescription);

            graphicsCommandBuffer.Draw(3);

            graphicsCommandBuffer.EndRenderPass();
            graphicsCommandBuffer.End();

            graphicsCommandBuffer.Commit();

            this.graphicsCommandQueue.Submit();
            this.graphicsCommandQueue.WaitIdle();

            this.paramsData.Samples += this.computeData.samples;
        }
    }
}
