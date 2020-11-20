#version 460

uniform sampler2D SPIRV_Cross_CombinedDiffuseTextureSampler;

layout(location = 0) in vec2 in_var_TEXCOORD;
layout(location = 0) out vec4 out_var_SV_Target;

void main()
{
    out_var_SV_Target = texture(SPIRV_Cross_CombinedDiffuseTextureSampler, in_var_TEXCOORD);
}

