﻿..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T vs_5_0 -E VS -Fo VK\VertexShader.spirv
..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T ps_5_0 -E PS -Fo VK\FragmentShader.spirv

Write-Host 'Vulkan Shaders generated'

#..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\VertexShader.spirv --output MSL\VertexShader.msl
#..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\FragmentShader.spirv --output MSL\FragmentShader.msl

Write-Host 'Metal Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe VK\VertexShader.spirv --output GLSL\VertexShader.glsl
..\..\..\..\..\Binaries\SPIRV-Cross.exe VK\FragmentShader.spirv --output GLSL\FragmentShader.glsl

Write-Host 'OpenGL Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\VertexShader.spirv --output ESSL\VertexShader.essl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\FragmentShader.spirv --output ESSL\FragmentShader.essl

# Fixing the output WebGL shaders
$filePath = "ESSL\VertexShader.essl"
$fileContent = Get-Content $filePath
$fileContent = $fileContent -replace "out_var", "var"
Set-Content -Path $filePath -Value $fileContent

$filePath = "ESSL\FragmentShader.essl"
$fileContent = Get-Content $filePath
$fileContent = $fileContent -replace "in_var", "var"
Set-Content -Path $filePath -Value $fileContent

Write-Host 'WebGL Shaders generated'

cp .\VK\VertexShader.spirv .\VK\VertexShader.spv
cp .\VK\FragmentShader.spirv .\VK\FragmentShader.spv

..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\VertexShader.spv WGSL\VertexShader.wgsl
..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\FragmentShader.spv WGSL\FragmentShader.wgsl

rm .\VK\*.spv

Write-Host 'WebGPU Shaders generated'

Write-Host -NoNewLine 'Press any key to continue...';