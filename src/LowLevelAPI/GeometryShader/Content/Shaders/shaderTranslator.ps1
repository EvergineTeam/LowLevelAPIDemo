﻿..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T vs_5_0 -E VS -Fo VK\VertexShader.spirv
..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T gs_5_0 -E GS -Fo VK\GeometryShader.spirv
..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T ps_5_0 -E PS -Fo VK\FragmentShader.spirv

Write-Host 'Vulkan Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\VertexShader.spirv --output MSL\VertexShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\GeometryShader.spirv --output MSL\GeometryShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\FragmentShader.spirv --output MSL\FragmentShader.msl

Write-Host 'Metal Shaders generated'

##################################
# The openGL version doesn't work.

#..\..\..\..\Binaries\SPIRV-Cross.exe VK\VertexShader.spirv --output GLSL\VertexShader.glsl
#..\..\..\..\Binaries\SPIRV-Cross.exe --version 330 VK\GeometryShader.spirv --output GLSL\GeometryShader.glsl
#..\..\..\..\Binaries\SPIRV-Cross.exe VK\FragmentShader.spirv --output GLSL\FragmentShader.glsl
#
#Write-Host 'OpenGL Shaders generated'
#
#..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 310 VK\VertexShader.spirv --output ESSL\VertexShader.essl
#..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 310 VK\GeometryShader.spirv --output ESSL\GeometryShader.glsl
#..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 310 VK\FragmentShader.spirv --output ESSL\FragmentShader.essl
#
#Write-Host 'OpenGLES Shaders generated'

Write-Host -NoNewLine 'Press any key to continue...';