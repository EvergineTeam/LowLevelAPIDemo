..\..\..\..\..\Binaries\dxc.exe -fvk-use-scalar-layout -fspv-target-env='vulkan1.1' -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T as_6_5 -E AS -Fo VK\AmplificationShader.spirv
..\..\..\..\..\Binaries\dxc.exe -fvk-use-scalar-layout -fspv-target-env='vulkan1.1' -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T ms_6_5 -E MS -Fo VK\MeshShader.spirv
..\..\..\..\..\Binaries\dxc.exe -fvk-use-scalar-layout -fspv-target-env='vulkan1.1' -Zpr -fvk-u-shift 20 all -fvk-s-shift 40 all -fvk-t-shift 60 all -spirv HLSL\HLSL.fx -T ps_6_5 -E PS -Fo VK\FragmentShader.spirv

Write-Host 'Vulkan Shaders generated'

..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\AmplificationShader.spirv --output MSL\AmplificationShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\MeshShader.spirv --output MSL\MeshShader.msl
..\..\..\..\..\Binaries\SPIRV-Cross.exe --msl VK\FragmentShader.spirv --output MSL\FragmentShader.msl

Write-Host 'Metal Shaders generated'

Write-Host -NoNewLine 'Press any key to continue...';