#version 450

layout(binding = 0, std140) uniform type_ParamsBuffer
{
    uint Samples;
    uint IsPathTracing;
} ParamsBuffer;

uniform sampler2D SPIRV_Cross_CombinedDiffuseTextureSampler;

layout(location = 0) in vec2 in_var_TEXCOORD;
layout(location = 0) out vec4 out_var_SV_Target;

void main()
{
    out_var_SV_Target = vec4(texture(SPIRV_Cross_CombinedDiffuseTextureSampler, in_var_TEXCOORD).xyz, (ParamsBuffer.IsPathTracing != 0u) ? (1.0 / float(ParamsBuffer.Samples)) : 1.0);
}

