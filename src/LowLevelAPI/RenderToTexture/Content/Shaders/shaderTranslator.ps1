..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\CubeHLSL.fx -T vs_5_0 -E VS -Fo VK\CubeVertexShader.spirv
..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\CubeHLSL.fx -T ps_5_0 -E PS -Fo VK\CubeFragmentShader.spirv

..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\TriangleHLSL.fx -T vs_5_0 -E VS -Fo VK\TriangleVertexShader.spirv
..\..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\TriangleHLSL.fx -T ps_5_0 -E PS -Fo VK\TriangleFragmentShader.spirv

Write-Host 'Vulkan Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\CubeVertexShader.spirv --output MSL\CubeVertexShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\CubeFragmentShader.spirv --output MSL\CubeFragmentShader.msl

..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\TriangleVertexShader.spirv --output MSL\TriangleVertexShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\TriangleFragmentShader.spirv --output MSL\TriangleFragmentShader.msl

Write-Host 'Metal Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe VK\CubeVertexShader.spirv --output GLSL\CubeVertexShader.glsl
..\..\..\..\..\Binaries\SPIRV-Cross.exe VK\CubeFragmentShader.spirv --output GLSL\CubeFragmentShader.glsl

..\..\..\..\..\Binaries\SPIRV-Cross.exe VK\TriangleVertexShader.spirv --output GLSL\TriangleVertexShader.glsl
..\..\..\..\..\Binaries\SPIRV-Cross.exe VK\TriangleFragmentShader.spirv --output GLSL\TriangleFragmentShader.glsl

Write-Host 'OpenGL Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\CubeVertexShader.spirv --output ESSL\CubeVertexShader.essl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\CubeFragmentShader.spirv --output ESSL\CubeFragmentShader.essl
cp .\VK\CubeVertexShader.spirv .\VK\CubeVertexShader.spv
cp .\VK\CubeFragmentShader.spirv .\VK\CubeFragmentShader.spv
cp .\VK\TriangleVertexShader.spirv .\VK\TriangleVertexShader.spv
cp .\VK\TriangleFragmentShader.spirv .\VK\TriangleFragmentShader.spv

..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\CubeVertexShader.spv WGSL\CubeVertexShader.wgsl
..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\CubeFragmentShader.spv WGSL\CubeFragmentShader.wgsl
..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\TriangleVertexShader.spv WGSL\TriangleVertexShader.wgsl
..\..\..\..\..\Binaries\naga.exe --keep-coordinate-space VK\TriangleFragmentShader.spv WGSL\TriangleFragmentShader.wgsl

rm .\VK\*.spv

Write-Host 'WebGPU Shaders generated'

# These fail under WebGL
#..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\CubeVertexShader.spirv --output ESSL\CubeVertexShader.essl
#..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\CubeFragmentShader.spirv --output ESSL\CubeFragmentShader.essl

..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\TriangleVertexShader.spirv --output ESSL\TriangleVertexShader.essl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\TriangleFragmentShader.spirv --output ESSL\TriangleFragmentShader.essl

# Fixing the output WebGL shaders
$filePath = "ESSL\CubeVertexShader.essl"
$fileContent = Get-Content $filePath
$fileContent = $fileContent -replace "out_var", "var"
Set-Content -Path $filePath -Value $fileContent

$filePath = "ESSL\CubeFragmentShader.essl"
$fileContent = Get-Content $filePath
$fileContent = $fileContent -replace "in_var", "var"
Set-Content -Path $filePath -Value $fileContent

$filePath = "ESSL\TriangleVertexShader.essl"
$fileContent = Get-Content $filePath
$fileContent = $fileContent -replace "out_var", "var"
Set-Content -Path $filePath -Value $fileContent

$filePath = "ESSL\TriangleFragmentShader.essl"
$fileContent = Get-Content $filePath
$fileContent = $fileContent -replace "in_var", "var"
Set-Content -Path $filePath -Value $fileContent

Write-Host 'WebGL Shaders generated'

Write-Host -NoNewLine 'Press any key to continue...';