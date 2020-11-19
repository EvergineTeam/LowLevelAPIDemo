#version 450

layout(binding = 0, std140) uniform type_Matrices
{
    uint IsTextured;
    mat4 worldViewProj;
} Matrices;

uniform sampler2D SPIRV_Cross_CombinedDiffuseTextureSampler;

layout(location = 0) in vec4 in_var_COLOR;
layout(location = 1) in vec2 in_var_TEXCOORD;
layout(location = 0) out vec4 out_var_SV_Target;

void main()
{
    vec4 _48;
    for (;;)
    {
        if (Matrices.IsTextured != 0u)
        {
            _48 = texture(SPIRV_Cross_CombinedDiffuseTextureSampler, in_var_TEXCOORD);
            break;
        }
        else
        {
            _48 = in_var_COLOR;
            break;
        }
    }
    out_var_SV_Target = _48;
}

