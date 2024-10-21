..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\CS.fx -T cs_5_0 -E CS -Fo VK\ComputeShader.spirv
..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T vs_5_0 -E VS -Fo VK\VertexShader.spirv
..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T ps_5_0 -E PS -Fo VK\FragmentShader.spirv

Write-Host 'Vulkan Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\ComputeShader.spirv --output MSL\ComputeShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\VertexShader.spirv --output MSL\VertexShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\FragmentShader.spirv --output MSL\FragmentShader.msl

Write-Host 'Metal Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe --version 460 VK\ComputeShader.spirv --output GLSL\ComputeShader.glsl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --version 460 VK\VertexShader.spirv --output GLSL\VertexShader.glsl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --version 460 VK\FragmentShader.spirv --output GLSL\FragmentShader.glsl

Write-Host 'OpenGL Shaders generated'

cp .\VK\ComputeShader.spirv .\VK\ComputeShader.spv
cp .\VK\VertexShader.spirv .\VK\VertexShader.spv
cp .\VK\FragmentShader.spirv .\VK\FragmentShader.spv

..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\ComputeShader.spv WGSL\ComputeShader.wgsl
..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\VertexShader.spv WGSL\VertexShader.wgsl
..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\FragmentShader.spv WGSL\FragmentShader.wgsl

rm .\VK\*.spv

Write-Host 'WebGPU Shaders generated'

Write-Host -NoNewLine 'Press any key to continue...';