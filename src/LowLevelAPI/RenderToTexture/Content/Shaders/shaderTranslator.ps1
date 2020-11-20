..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\CubeHLSL.fx -T vs_5_0 -E VS -Fo VK\CubeVertexShader.spirv
..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\CubeHLSL.fx -T ps_5_0 -E PS -Fo VK\CubeFragmentShader.spirv

..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\TriangleHLSL.fx -T vs_5_0 -E VS -Fo VK\TriangleVertexShader.spirv
..\..\..\..\Binaries\dxc.exe -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\TriangleHLSL.fx -T ps_5_0 -E PS -Fo VK\TriangleFragmentShader.spirv

Write-Host 'Vulkan Shaders generated'

..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\CubeVertexShader.spirv --output MSL\CubeVertexShader.msl
..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\CubeFragmentShader.spirv --output MSL\CubeFragmentShader.msl

..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\TriangleVertexShader.spirv --output MSL\TriangleVertexShader.msl
..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\TriangleFragmentShader.spirv --output MSL\TriangleFragmentShader.msl

Write-Host 'Metal Shaders generated'

..\..\..\..\Binaries\SPIRV-Cross.exe VK\CubeVertexShader.spirv --output GLSL\CubeVertexShader.glsl
..\..\..\..\Binaries\SPIRV-Cross.exe VK\CubeFragmentShader.spirv --output GLSL\CubeFragmentShader.glsl

..\..\..\..\Binaries\SPIRV-Cross.exe VK\TriangleVertexShader.spirv --output GLSL\TriangleVertexShader.glsl
..\..\..\..\Binaries\SPIRV-Cross.exe VK\TriangleFragmentShader.spirv --output GLSL\TriangleFragmentShader.glsl

Write-Host 'OpenGL Shaders generated'

# These fail under WebGL
#..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\CubeVertexShader.spirv --output ESSL\CubeVertexShader.essl
#..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\CubeFragmentShader.spirv --output ESSL\CubeFragmentShader.essl

#..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\TriangleVertexShader.spirv --output ESSL\TriangleVertexShader.essl
#..\..\..\..\Binaries\SPIRV-Cross.exe --es --version 300 VK\TriangleFragmentShader.spirv --output ESSL\TriangleFragmentShader.essl

#Write-Host 'OpenGLES Shaders generated'

Write-Host -NoNewLine 'Press any key to continue...';