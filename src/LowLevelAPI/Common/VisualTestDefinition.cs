using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Evergine.Common;
using Evergine.Common.Audio;
using Evergine.Common.Graphics;
using Evergine.Common.IO;
using static Evergine.Common.Graphics.SurfaceInfo;

namespace Common
{
    public abstract class VisualTestDefinition : IDisposable
    {
        protected AssetsDirectory assetsDirectory;
        protected WindowsSystem windowSystem;
        protected Surface surface;

        protected FrameBuffer frameBuffer;
        protected GraphicsContext graphicsContext;
        protected SwapChain swapChain;

        protected AudioDevice audioDevice;

        private Stopwatch clockTimer;
        private Stopwatch fpsTimer;
        private int fpsCounter;
        private bool windowResized;
        private bool isLoadingPending = true;

        protected string assetsRootPath;

        protected bool doPresent = true;

        protected TextureSampleCount SampleCount = TextureSampleCount.None;

        public WindowsSystem WindowSystem => this.windowSystem;

        public SwapChain SwapChain => this.swapChain;

        public Surface Surface
        {
            get => this.surface;
            set => this.surface = value;
        }

        public Action<string> FPSUpdateCallback;

        public VisualTestDefinition()
            : this(string.Empty)
        {
        }

        public VisualTestDefinition(string contentSubPath)
        {
            var assemblyPath = Assembly.GetEntryAssembly()?.Location ?? string.Empty;
            this.assetsRootPath = Path.Combine(Path.GetDirectoryName(assemblyPath), AssetsDirectory.DefaultFolderName, contentSubPath);
        }

        public void Initialize()
        {
            this.assetsDirectory = new AssetsDirectory(this.assetsRootPath);
            this.windowSystem = GetInstance<WindowsSystem>("Evergine.Forms", "FormsWindowsSystem");
        }

        public GraphicsContext CreateGraphicsContext(SwapChainDescription? swapChainDescriptor = null, GraphicsBackend? preferredBackend = null)
        {
            string graphicsContextTypeName = string.Empty;
            switch (preferredBackend)
            {
                case GraphicsBackend.DirectX11:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("Evergine.DirectX11", "DX11GraphicsContext");
                    break;
                case GraphicsBackend.DirectX12:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("Evergine.DirectX12", "DX12GraphicsContext");
                    break;
                case GraphicsBackend.OpenGL:
                case GraphicsBackend.OpenGLES:
                case GraphicsBackend.WebGL2:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("Evergine.OpenGL", "GLGraphicsContext");
                    break;
                case GraphicsBackend.Vulkan:
                    this.graphicsContext = this.GetInstance<GraphicsContext>(
                        "Evergine.Vulkan",
                        "VKGraphicsContext",
                        new[]
                        {
                            "VK_KHR_multiview",
                            "VK_KHR_external_memory",
                            "VK_KHR_external_memory_win32",
                            "VK_KHR_external_fence",
                            "VK_KHR_external_fence_win32",
                            "VK_KHR_external_semaphore",
                            "VK_KHR_external_semaphore_win32",
                        },
                        new[]
                        {
                            "VK_KHR_get_physical_device_properties2",
                            "VK_KHR_external_memory_capabilities",
                            "VK_KHR_external_fence_capabilities",
                            "VK_KHR_external_semaphore_capabilities",
                        });
                    break;
                case GraphicsBackend.Metal:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("Evergine.Metal", "MTLGraphicsContext");
                    break;
                case GraphicsBackend.WebGPU:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("Evergine.WebGPU", "WGPUGraphicsContext");
                    break;
                default:
                    throw new InvalidOperationException("Invalid render backend");
            }

#if DEBUG
            this.graphicsContext.CreateDevice(new ValidationLayer(ValidationLayer.NotifyMethod.Trace));
#else
            this.graphicsContext.CreateDevice();
#endif

            if (swapChainDescriptor != null)
            {
                this.swapChain = this.graphicsContext.CreateSwapChain(swapChainDescriptor.Value);
                this.swapChain.VerticalSync = true;
            }

            return this.graphicsContext;
        }

        protected T GetInstance<T>(string assemblyName, string typeName, params object[] arguments)
        {
            var assembly = Assembly.Load(assemblyName);
            typeName = $"{assemblyName}.{typeName}";
            if (arguments?.Length > 0)
            {
                return (T)Activator.CreateInstance(assembly.GetType(typeName), arguments);
            }
            else
            {
                return (T)Activator.CreateInstance(assembly.GetType(typeName));
            }
        }

        public void Run()
        {
            this.windowSystem.Run(this.Load, this.Draw);
        }

        public void Load()
        {
            if (this.surface != null)
            {
                this.surface.OnScreenSizeChanged += (s, e) => { this.windowResized = true; };
            }

            this.frameBuffer = this.swapChain?.FrameBuffer;

            this.InternalLoad();

            this.clockTimer = Stopwatch.StartNew();
            this.fpsTimer = Stopwatch.StartNew();
        }

        public void Draw()
        {
            if (isLoadingPending)
            {
                return;
            }

            this.Surface?.KeyboardDispatcher?.DispatchEvents();
            this.Surface?.MouseDispatcher?.DispatchEvents();
            this.Surface?.TouchDispatcher?.DispatchEvents();

            if (this.doPresent)
            {
                if (this.windowResized)
                {
                    this.windowResized = false;
                    this.swapChain?.ResizeSwapChain(this.surface.Width, this.surface.Height);
                    this.frameBuffer = this.swapChain?.FrameBuffer;
                    this.OnResized(this.surface.Width, this.surface.Height);
                }

                this.swapChain?.InitFrame();
            }

            this.CalculateFPS();

            var gameTime = this.clockTimer.Elapsed;
            this.clockTimer.Restart();

            this.InternalDrawCallback(gameTime);

            if (this.doPresent)
            {
                this.swapChain?.Present();
            }
        }

        public virtual SwapChainDescription CreateSwapChainDescription(uint width, uint height)
        {
            return new SwapChainDescription()
            {
                Width = width,
                Height = height,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = this.SampleCount,
                IsWindowed = true,
                RefreshRate = 60,
            };
        }


        protected abstract void OnResized(uint width, uint height);

        protected abstract void InternalLoad();

        protected abstract void InternalDrawCallback(TimeSpan gameTime);

        protected void MarkAsLoaded() => isLoadingPending = false;

        private void CalculateFPS()
        {
            this.fpsCounter++;
            if (this.fpsTimer.ElapsedMilliseconds > 1000)
            {
                var fpsString = string.Format("FPS: {0:F2} ({1:F2}ms)", 1000.0 * this.fpsCounter / this.fpsTimer.ElapsedMilliseconds, (float)this.fpsTimer.ElapsedMilliseconds / this.fpsCounter);
                this.FPSUpdateCallback?.Invoke(fpsString);

                this.fpsTimer.Restart();
                this.fpsCounter = 0;
            }
        }

        public void Dispose()
        {
        }
    }
}
