#version 450

uniform samplerCube SPIRV_Cross_CombinedCubeTextureSampler;

layout(location = 0) in vec3 in_var_TEXCOORD0;
layout(location = 0) out vec4 out_var_SV_Target;

void main()
{
    out_var_SV_Target = texture(SPIRV_Cross_CombinedCubeTextureSampler, in_var_TEXCOORD0);
}

