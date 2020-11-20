#version 450

uniform sampler2DArray SPIRV_Cross_CombinedDiffuseTextureSampler;

layout(location = 0) in vec3 in_var_NORMAL;
layout(location = 1) in vec3 in_var_TEXCOORD;
layout(location = 0) out vec4 out_var_SV_Target;

void main()
{
    vec4 _34 = texture(SPIRV_Cross_CombinedDiffuseTextureSampler, in_var_TEXCOORD);
    if ((_34.w - 0.5) < 0.0)
    {
        discard;
    }
    out_var_SV_Target = _34 * (0.5 + (clamp(dot(in_var_NORMAL, vec3(0.0, 0.0, -1.0)), 0.0, 1.0) * 0.5));
}

