using Common;
using Evergine.Bindings.MeshOptimizer;
using Evergine.Common.Graphics;
using Evergine.Common.Graphics.MeshShader;
using Evergine.Common.Input;
using Evergine.Common.Input.Keyboard;
using Evergine.Mathematics;
using OBJRuntime.DataTypes;
using OBJRuntime.Readers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Buffer = Evergine.Common.Graphics.Buffer;

namespace DrawMeshlets
{
    public class DrawMeshletsTest : VisualTestDefinition
    {
        public struct MeshletBounds
        {
            public Vector3 Center;
            public float Radius;
        }

        [StructLayout(LayoutKind.Explicit, Size = 128)]
        public struct WorldInfo
        {
            [FieldOffset(0)]
            public Matrix4x4 WorldViewProj;

            [FieldOffset(64)]
            public Matrix4x4 WorldViewProj2;
        }

        private Viewport[] viewports;
        private Rectangle[] scissors;
        private CommandQueue commandQueue;
        private MeshShaderPipelineState pipelineState;
        private ResourceLayout resourceLayout;
        private ResourceSet resourceSet;

        private Buffer constantBuffer;
        private Matrix4x4 view;
        private Matrix4x4 view2;
        private Matrix4x4 proj;
        private WorldInfo worldInfo;
        private float time;

        private Buffer positionBuffer;
        private Buffer meshletBuffer;
        private Buffer meshletVerticesBuffer;
        private Buffer meshletTrianglesBuffer;
        private Buffer meshletBoundsBuffer;
        private nuint meshletCount;
        private bool IsCullingEnabled;

        public DrawMeshletsTest()
        {
        }

        protected override bool CheckBackendCompatibility()
        {
            return this.GraphicsBackend != GraphicsBackend.OpenGL
                && this.GraphicsBackend != GraphicsBackend.OpenGLES
                && this.GraphicsBackend != GraphicsBackend.DirectX11;
        }

        protected override void OnResized(uint width, uint height)
        {
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);
        }

        protected override async void InternalLoad()
        {
            // ----------------------------
            // Compile Mesh and Pixel shaders
            // ----------------------------
            CompilerParameters parameters = new CompilerParameters()
            {
                Profile = GraphicsProfile.Level_12_5,
                CompilationMode = CompilationMode.Debug,
            };

            GraphicsBackend backend = graphicsContext.BackendType;
            byte[] amplificationShaderCode = null;
            byte[] meshShaderByteCode = null;
            byte[] pixelShaderByteCode = null;
            switch (backend)
            {
                case GraphicsBackend.DirectX12:
                    var source = await this.assetsDirectory.ReadAsStringAsync($"Shaders/HLSL/HLSL.fx");
                    amplificationShaderCode = graphicsContext.ShaderCompile(source, "AS", ShaderStages.Amplification, parameters).ByteCode;
                    meshShaderByteCode = graphicsContext.ShaderCompile(source, "MS", ShaderStages.Mesh, parameters).ByteCode;
                    pixelShaderByteCode = graphicsContext.ShaderCompile(source, "PS", ShaderStages.Pixel, parameters).ByteCode;
                    break;
                case GraphicsBackend.Vulkan:

                    using (var stream = assetsDirectory.Open($"Shaders/VK/AmplificationShader.spirv"))
                    using (var memstream = new MemoryStream())
                    {
                        stream.CopyTo(memstream);
                        amplificationShaderCode = memstream.ToArray();
                    }

                    using (var stream = assetsDirectory.Open($"Shaders/VK/MeshShader.spirv"))
                    using (var memstream = new MemoryStream())
                    {
                        stream.CopyTo(memstream);
                        meshShaderByteCode = memstream.ToArray();
                    }

                    using (var stream = assetsDirectory.Open($"Shaders/VK/FragmentShader.spirv"))
                    using (var memstream = new MemoryStream())
                    {
                        stream.CopyTo(memstream);
                        pixelShaderByteCode = memstream.ToArray();
                    }

                    break;
                default:
                    throw new Exception("Backend no supported");
            }

            var amplificationShaderDescription = new ShaderDescription(ShaderStages.Amplification, "AS", amplificationShaderCode);
            var amplificationShader = this.graphicsContext.Factory.CreateShader(ref amplificationShaderDescription);

            var meshShaderDescription = new ShaderDescription(ShaderStages.Mesh, "MS", meshShaderByteCode);
            var meshShader = this.graphicsContext.Factory.CreateShader(ref meshShaderDescription);

            var pixelShaderDescription = new ShaderDescription(ShaderStages.Pixel, "PS", pixelShaderByteCode);
            var pixelShader = this.graphicsContext.Factory.CreateShader(ref pixelShaderDescription);

            // ----------------------------
            // Read obj model
            // ----------------------------
            var attrib = new OBJAttrib();
            var shapes = new List<OBJShape>();
            var materials = new List<OBJMaterial>();
            var warning = string.Empty;
            var error = string.Empty;

            using (var stream = assetsDirectory.Open($"Resources/horse.obj"))
            using (var srObj = new StreamReader(stream))
            {
                bool success = OBJLoader.Load(srObj, ref attrib, shapes, materials, ref warning, ref error, null, string.Empty, true, true);
                if (!success)
                {
                    throw new Exception($"OBJ Load failed. Error:{error}");
                }
            }

            var mesh = shapes[0].Mesh;

            nuint kMaxVertices = 64;
            nuint kMaxTriangles = 124;
            float kConeWeight = 0.0f;
            nuint meshNumIndices = (nuint)mesh.Indices.Count;
            nuint meshNumVertices = (nuint)attrib.Vertices.Count;

            uint[] indices = new uint[meshNumIndices];
            for (int i = 0; i < (int)meshNumIndices; i += 3)
            {
                indices[i] = (uint)mesh.Indices[i].VertexIndex;
                indices[i + 1] = (uint)mesh.Indices[i + 1].VertexIndex;
                indices[i + 2] = (uint)mesh.Indices[i + 2].VertexIndex;
            }

            // ----------------------------
            // Make them meshlets!
            // ----------------------------
            nuint maxMeshlets = MeshOptNative.meshopt_buildMeshletsBound(meshNumIndices, kMaxVertices, kMaxTriangles);

            meshopt_Meshlet[] meshlets = new meshopt_Meshlet[maxMeshlets];
            uint[] meshletVertices = new uint[maxMeshlets * kMaxVertices];
            byte[] meshletTriangles = new byte[maxMeshlets * kMaxTriangles * 3];
            Vector3[] positions = attrib.Vertices.ToArray();

            uint[] meshletTrianglesU32;
            MeshletBounds[] meshletBounds;

            unsafe
            {
                meshopt_Meshlet* pMeshlets = (meshopt_Meshlet*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(meshlets));
                uint* pMeshletVertices = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(meshletVertices));
                byte* pMeshletTriangles = (byte*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(meshletTriangles));
                uint* pIndices = (uint*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(indices));
                float* pVertsAsFloats = (float*)Unsafe.AsPointer(ref MemoryMarshal.GetArrayDataReference(attrib.Vertices.ToArray()));

                this.meshletCount = MeshOptNative.meshopt_buildMeshlets(
                    pMeshlets,
                    pMeshletVertices,
                    pMeshletTriangles,
                    pIndices,
                    meshNumIndices,
                    pVertsAsFloats,
                    meshNumVertices,
                    (nuint)sizeof(Vector3),
                    kMaxVertices,
                    kMaxTriangles,
                    kConeWeight);

                meshletBounds = new MeshletBounds[this.meshletCount];
                for (int i = 0; i < (int)this.meshletCount; i++)
                {
                    var m = meshlets[i];
                    meshopt_Bounds b = MeshOptNative.meshopt_computeMeshletBounds(
                                                                                 pMeshletVertices + m.vertex_offset,
                                                                                 pMeshletTriangles + m.triangle_offset,
                                                                                 m.triangle_count,
                                                                                 pVertsAsFloats,
                                                                                 meshNumVertices,
                                                                                 (uint)sizeof(Vector3)
                                                                                );
                    meshletBounds[i] = new MeshletBounds()
                    {
                        Center = new Vector3(b.center[0], b.center[1], b.center[2]),
                        Radius = b.radius,
                    };
                }

                var last = meshlets[this.meshletCount - 1];
                Array.Resize(ref meshletVertices, (int)(last.vertex_offset + last.vertex_count));
                Array.Resize(ref meshletTriangles, (int)(last.triangle_offset + ((last.triangle_count * 3 + 3) & ~3)));
                Array.Resize(ref meshlets, (int)this.meshletCount);

                // Repack triangles from 3 consecutive bytes to 4-byte uint to
                // make it easier to unpack on the GPU.
                List<uint> meshletTrianglesAuxiliar = new List<uint>();
                for (int m = 0; m < meshlets.Length; m++)
                {
                    // Save triangle offset for current meshlet
                    uint triangleOffset = (uint)meshletTrianglesAuxiliar.Count;

                    // Repack to uint
                    for (uint i = 0; i < meshlets[m].triangle_count; i++)
                    {
                        uint baseIdx = meshlets[m].triangle_offset;
                        uint i0 = 3 * i + baseIdx + 0;
                        uint i1 = 3 * i + baseIdx + 1;
                        uint i2 = 3 * i + baseIdx + 2;

                        byte vIdx0 = meshletTriangles[(int)i0];
                        byte vIdx1 = meshletTriangles[(int)i1];
                        byte vIdx2 = meshletTriangles[(int)i2];

                        uint packed = (((uint)vIdx0 & 0xFF) << 0) |
                                      (((uint)vIdx1 & 0xFF) << 8) |
                                      (((uint)vIdx2 & 0xFF) << 16);

                        meshletTrianglesAuxiliar.Add(packed);
                    }

                    // Update triangle offset for current meshlet (en el array original)
                    meshlets[m].triangle_offset = triangleOffset;
                }

                meshletTrianglesU32 = meshletTrianglesAuxiliar.ToArray();
            }

            // ----------------------------
            // Create Resources
            // ----------------------------
            this.view = Matrix4x4.CreateLookAt(new Vector3(0, 0.105f, 0.4f), new Vector3(0, 0.105f, 0), Vector3.UnitY); // horse
            this.view2 = Matrix4x4.CreateLookAt(new Vector3(0, 0.105f, 0.4f), new Vector3(0.3f, 0.105f, 0), Vector3.UnitY); // culling
            //this.view = Matrix4x4.CreateLookAt(new Vector3(0, 1, 5f), new Vector3(0, 1, 0), Vector3.UnitY);
            this.proj = Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)this.frameBuffer.Width / (float)this.frameBuffer.Height, 0.1f, 10000f);
            this.worldInfo = new WorldInfo();

            // Constant Buffer
            var constantBufferDescription = new BufferDescription((uint)Unsafe.SizeOf<WorldInfo>(),
                                                                  BufferFlags.ConstantBuffer,
                                                                  ResourceUsage.Default);
            this.constantBuffer = this.graphicsContext.Factory.CreateBuffer(ref constantBufferDescription);

            // Vertex positions Buffers
            // Note. Required use -fvk-use-scalar-layout flag on DXC compilation process
            var positionBufferDescription = new BufferDescription((uint)(Unsafe.SizeOf<Vector3>() * positions.Length),
                                                                  BufferFlags.BufferStructured | BufferFlags.ShaderResource,
                                                                  ResourceUsage.Default,
                                                                  ResourceCpuAccess.None,
                                                                  Unsafe.SizeOf<Vector3>());
            this.positionBuffer = this.graphicsContext.Factory.CreateBuffer(positions, ref positionBufferDescription);

            // Meshlet Buffer
            var meshletBufferDescription = new BufferDescription((uint)(Unsafe.SizeOf<meshopt_Meshlet>() * meshlets.Length),
                                                                  BufferFlags.BufferStructured | BufferFlags.ShaderResource,
                                                                  ResourceUsage.Default,
                                                                  ResourceCpuAccess.None,
                                                                  Unsafe.SizeOf<meshopt_Meshlet>());
            this.meshletBuffer = this.graphicsContext.Factory.CreateBuffer(meshlets, ref meshletBufferDescription);

            // Meshlet vertices Buffer
            var meshletVerticesBufferDescription = new BufferDescription((uint)(sizeof(uint) * meshletVertices.Length),
                                                                          BufferFlags.BufferStructured | BufferFlags.ShaderResource,
                                                                          ResourceUsage.Default,
                                                                          ResourceCpuAccess.None,
                                                                          sizeof(uint));
            this.meshletVerticesBuffer = this.graphicsContext.Factory.CreateBuffer(meshletVertices, ref meshletVerticesBufferDescription);

            // Meshlet triangles Buffer
            var meshletTrianglesBufferDescription = new BufferDescription((uint)(sizeof(uint) * meshletTrianglesU32.Length),
                                                                          BufferFlags.BufferStructured | BufferFlags.ShaderResource,
                                                                          ResourceUsage.Default,
                                                                          ResourceCpuAccess.None,
                                                                          sizeof(uint));
            this.meshletTrianglesBuffer = this.graphicsContext.Factory.CreateBuffer(meshletTrianglesU32, ref meshletTrianglesBufferDescription);

            // Meshlet bounds Buffer
            var meshletBoundsBufferDescription = new BufferDescription((uint)(Unsafe.SizeOf<MeshletBounds>() * meshletBounds.Length),
                                                                       BufferFlags.BufferStructured | BufferFlags.ShaderResource,
                                                                       ResourceUsage.Default,
                                                                       ResourceCpuAccess.None,
                                                                       Unsafe.SizeOf<MeshletBounds>());
            this.meshletBoundsBuffer = this.graphicsContext.Factory.CreateBuffer(meshletBounds, ref meshletBoundsBufferDescription);

            var resourceLayoutDescription = new ResourceLayoutDescription(
                    new LayoutElementDescription(0, ResourceType.ConstantBuffer, ShaderStages.Mesh | ShaderStages.Amplification),
                    new LayoutElementDescription(1, ResourceType.StructuredBuffer, ShaderStages.Mesh),
                    new LayoutElementDescription(2, ResourceType.StructuredBuffer, ShaderStages.Mesh),
                    new LayoutElementDescription(3, ResourceType.StructuredBuffer, ShaderStages.Mesh),
                    new LayoutElementDescription(4, ResourceType.StructuredBuffer, ShaderStages.Mesh),
                    new LayoutElementDescription(5, ResourceType.StructuredBuffer, ShaderStages.Amplification)
                    );

            this.resourceLayout = this.graphicsContext.Factory.CreateResourceLayout(ref resourceLayoutDescription);

            // ----------------------------
            // Prepare Pipeline           
            // ----------------------------
            var pipelineDescription = new MeshShaderPipelineDescription
            {
                PrimitiveTopology = PrimitiveTopology.TriangleList,
                ResourceLayouts = new[] { this.resourceLayout },
                Shaders = new MeshShaderStateDescription()
                {
                    AmplificationShader = amplificationShader,
                    MeshShader = meshShader,
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

            this.pipelineState = this.graphicsContext.Factory.CreateMeshShaderPipeline(ref pipelineDescription);
            this.commandQueue = this.graphicsContext.Factory.CreateCommandQueue();

            var swapChainDescription = this.swapChain?.SwapChainDescription;
            var width = swapChainDescription.HasValue ? swapChainDescription.Value.Width : this.surface.Width;
            var height = swapChainDescription.HasValue ? swapChainDescription.Value.Height : this.surface.Height;

            this.viewports = new Viewport[1];
            this.viewports[0] = new Viewport(0, 0, width, height);
            this.scissors = new Rectangle[1];
            this.scissors[0] = new Rectangle(0, 0, (int)width, (int)height);

            var resourceSetDescription = new ResourceSetDescription(this.resourceLayout,
                                                                    this.constantBuffer,
                                                                    this.positionBuffer,
                                                                    this.meshletBuffer,
                                                                    this.meshletVerticesBuffer,
                                                                    this.meshletTrianglesBuffer,
                                                                    this.meshletBoundsBuffer);
            this.resourceSet = this.graphicsContext.Factory.CreateResourceSet(ref resourceSetDescription);

            this.MarkAsLoaded();
        }

        protected override void InternalDrawCallback(TimeSpan gameTime)
        {
            // Update
            this.time += (float)gameTime.TotalSeconds;
            var world = Matrix4x4.CreateRotationY(this.time * 1.0f);

            var viewProj = Matrix4x4.Multiply(this.view, this.proj);
            this.worldInfo.WorldViewProj = world * viewProj;

            var view2Proj = Matrix4x4.Multiply(this.view2, this.proj);
            var worldViewProjCulling = world * view2Proj;

            var keyboardDispatcher = this.surface.KeyboardDispatcher;
            if (keyboardDispatcher?.ReadKeyState(Keys.Space) == ButtonState.Pressing)
            {
                this.IsCullingEnabled = !this.IsCullingEnabled;
            }

            this.worldInfo.WorldViewProj2 = this.IsCullingEnabled ? worldViewProjCulling : this.worldInfo.WorldViewProj;

            // Draw
            var commandBuffer = this.commandQueue.CommandBuffer();

            commandBuffer.Begin();

            commandBuffer.UpdateBufferData(this.constantBuffer, ref this.worldInfo);

            commandBuffer.SetViewports(this.viewports);
            commandBuffer.SetScissorRectangles(this.scissors);

            RenderPassDescription renderPassDescription = new RenderPassDescription(this.frameBuffer, new ClearValue(ClearFlags.All, 1, 0, new Color(0.23f, 0.23f, 0.31f) /*Color.CornflowerBlue*/));
            commandBuffer.BeginRenderPass(ref renderPassDescription);

            commandBuffer.SetMeshShaderPipelineState(this.pipelineState);
            commandBuffer.SetResourceSet(this.resourceSet);

            commandBuffer.DispatchMesh((uint)this.meshletCount, 1, 1);

            commandBuffer.EndRenderPass();
            commandBuffer.End();

            commandBuffer.Commit();

            this.commandQueue.Submit();
            this.commandQueue.WaitIdle();
        }
    }
}
