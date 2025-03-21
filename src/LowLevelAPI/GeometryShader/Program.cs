﻿using System;
using Evergine.Common.Graphics;

namespace GeometryShader
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            uint width = 1280;
            uint height = 720;
            using (var test = new GeometryShaderTest())
            {
                test.Initialize();

                // Create Window
                string windowsTitle = $"{typeof(GeometryShaderTest).Name}";
                var windowSystem = test.WindowSystem;
                var window = windowSystem.CreateWindow(windowsTitle, width, height);
                test.Surface = window;
                test.FPSUpdateCallback = (fpsString) =>
                {
                    window.Title = $"{windowsTitle}  {fpsString}";
                };

                // Managers
                var swapChainDescriptor = test.CreateSwapChainDescription(window.Width, window.Height);
                swapChainDescriptor.SurfaceInfo = window.SurfaceInfo;

                var graphicsContext = test.CreateGraphicsContext(swapChainDescriptor, GraphicsBackend.Vulkan);
                windowsTitle = $"{windowsTitle} [{graphicsContext.BackendType}]";

                test.Run();
            }
        }
    }
}
