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

Write-Host -NoNewLine 'Press any key to continue...';