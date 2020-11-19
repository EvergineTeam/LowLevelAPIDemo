#version 450

uniform sampler2D SPIRV_Cross_CombinedDiffuseTextureSampler;

layout(location = 0) in vec4 in_var_COLOR;
layout(location = 1) in vec2 in_var_TEXCOORD;
layout(location = 0) out vec4 out_var_SV_Target;

void main()
{
    out_var_SV_Target = texture(SPIRV_Cross_CombinedDiffuseTextureSampler, in_var_TEXCOORD);
}

