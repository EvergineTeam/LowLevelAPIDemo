using System;
using System.Diagnostics;
using System.Reflection;
using WaveEngine.Common.Audio;
using WaveEngine.Common.Graphics;
using WaveEngine.Platform;
using static WaveEngine.Common.Graphics.SurfaceInfo;

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

        public AudioDevice AudioDevice => this.audioDevice;

        public FrameBuffer FrameBuffer { get => this.frameBuffer; set => this.frameBuffer = value; }

        public Action<string> FPSUpdateCallback;

        public VisualTestDefinition()
        {
            this.assetsRootPath = AssetsDirectory.DefaultFolderName;
        }

        public void Initialize()
        {
            this.assetsDirectory = new AssetsDirectory(this.assetsRootPath);
            this.windowSystem = GetInstance<WindowsSystem>("WaveEngine.SDL", "SDLWindowsSystem");
        }

        public GraphicsContext CreateGraphicsContext(SwapChainDescription? swapChainDescriptor = null, GraphicsBackend? prefferedBackend = null)
        {
            string graphicsContextTypeName = string.Empty;
            switch (prefferedBackend)
            {
                case GraphicsBackend.DirectX11:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("WaveEngine.DirectX11", "DX11GraphicsContext");
                    break;
                case GraphicsBackend.DirectX12:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("WaveEngine.DirectX12", "DX12GraphicsContext");
                    break;
                case GraphicsBackend.OpenGL:
                case GraphicsBackend.OpenGLES:
                case GraphicsBackend.WebGL2:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("WaveEngine.OpenGL", "GLGraphicsContext");
                    break;
                case GraphicsBackend.Vulkan:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("WaveEngine.Vulkan", "VKGraphicsContext");
                    break;
                case GraphicsBackend.Metal:
                    this.graphicsContext = this.GetInstance<GraphicsContext>("WaveEngine.Metal", "MTLGraphicsContext");
                    break;
                default:
                    throw new InvalidOperationException("Invalid render backend");
            }

            this.graphicsContext.DefaultTextureUploaderSize = 128 * 1024 * 1024;
            this.graphicsContext.DefaultBufferUploaderSize = 64 * 1024 * 1024;
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

        protected bool TryGetInstance<T>(string assemblyName, string typeName, out T instance, params object[] arguments)
        {
            try
            {
                instance = this.GetInstance<T>(assemblyName, typeName, arguments);
                return true;
            }
            catch
            {
                instance = default;
                return false;
            }
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

            if (this.doPresent)
            {
                if (this.windowResized)
                {
                    this.windowResized = false;
                    this.swapChain?.ResizeSwapChain(this.surface.Width, this.surface.Height);
                    this.frameBuffer = this.swapChain?.FrameBuffer;
                    this.OnResized(this.surface.Width, this.surface.Height);
                }
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

        public virtual SwapChainDescription CreateSwapChainDescription(uint width, uint height, SurfaceInfo info)
        {
            return new SwapChainDescription()
            {
                Width = width,
                Height = height,
                SurfaceInfo = info,
                ColorTargetFormat = PixelFormat.R8G8B8A8_UNorm,
                ColorTargetFlags = TextureFlags.RenderTarget | TextureFlags.ShaderResource,
                DepthStencilTargetFormat = PixelFormat.D24_UNorm_S8_UInt,
                DepthStencilTargetFlags = TextureFlags.DepthStencil,
                SampleCount = this.SampleCount,
                IsWindowed = true,
                RefreshRate = 60,
            };
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
