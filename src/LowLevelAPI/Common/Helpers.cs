using System;
using System.IO;
using System.Threading.Tasks;
using Evergine.Common.Graphics;
using Evergine.Common.IO;

namespace Common
{
    public static class Helpers
    {
        public static Task<ShaderDescription> ReadAndCompileShader(this AssetsDirectory assetsDirectory, GraphicsContext graphicsContext, string hlslFileName, string translateFileName, ShaderStages stage, string entryPoint)
        {
            return ReadAndCompileShader(assetsDirectory, graphicsContext, hlslFileName, translateFileName, stage, entryPoint, CompilerParameters.Default);
        }

        public static async Task<ShaderDescription> ReadAndCompileShader(this AssetsDirectory assetsDirectory, GraphicsContext graphicsContext, string hlslFileName, string translateFileName, ShaderStages stage, string entryPoint, CompilerParameters compileParameters)
        {
            GraphicsBackend backend = graphicsContext.BackendType;

            string source;
            byte[] bytecode = null;

            switch (backend)
            {
                case GraphicsBackend.DirectX11:
                case GraphicsBackend.DirectX12:

                    source = await assetsDirectory.ReadAsStringAsync($"Shaders/HLSL/{hlslFileName}.fx");
                    bytecode = graphicsContext.ShaderCompile(source, entryPoint, stage, compileParameters).ByteCode;

                    break;
                case GraphicsBackend.OpenGL:

                    source = await assetsDirectory.ReadAsStringAsync($"Shaders/GLSL/{translateFileName}.glsl");
                    bytecode = graphicsContext.ShaderCompile(source, entryPoint, stage, compileParameters).ByteCode;

                    break;
                case GraphicsBackend.OpenGLES:
                case GraphicsBackend.WebGL1:
                case GraphicsBackend.WebGL2:

                    source = await assetsDirectory.ReadAsStringAsync($"Shaders/ESSL/{translateFileName}.essl");
                    bytecode = graphicsContext.ShaderCompile(source, entryPoint, stage, compileParameters).ByteCode;

                    break;
                case GraphicsBackend.Metal:

                    source = await assetsDirectory.ReadAsStringAsync($"Shaders/MSL/{translateFileName}.msl");
                    bytecode = graphicsContext.ShaderCompile(source, entryPoint, stage, compileParameters).ByteCode;

                    break;
                case GraphicsBackend.Vulkan:

                    using (var stream = assetsDirectory.Open($"Shaders/VK/{translateFileName}.spirv"))
                    using (var memstream = new MemoryStream())
                    {
                        stream.CopyTo(memstream);
                        bytecode = memstream.ToArray();
                    }

                    break;
                case GraphicsBackend.WebGPU:

                    using (var stream = assetsDirectory.Open($"Shaders/WGSL/{translateFileName}.wgsl"))
                    using (var memstream = new MemoryStream())
                    {
                        stream.CopyTo(memstream);
                        bytecode = memstream.ToArray();
                    }

                    break;
                default:
                    throw new Exception($"Backend not found {backend}");
            }

            ShaderDescription description = new ShaderDescription(stage, entryPoint, bytecode);

            return description;
        }
    }
}
