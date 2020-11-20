#version 450

uniform samplerCubeArray SPIRV_Cross_CombinedCubeTextureSampler;

layout(location = 0) in vec4 in_var_TEXCOORD0;
layout(location = 1) in vec3 in_var_TEXCOORD1;
layout(location = 2) in vec3 in_var_TEXCOORD2;
layout(location = 0) out vec4 out_var_SV_Target;

float _29;

void main()
{
    out_var_SV_Target = vec4(texture(SPIRV_Cross_CombinedCubeTextureSampler, vec4(reflect(normalize(in_var_TEXCOORD1), normalize(in_var_TEXCOORD2)), float(int(((vec2((in_var_TEXCOORD0.xy / vec2(in_var_TEXCOORD0.w)).x, _29) + vec2(1.0)) * 0.5).x * 4.0)))).xyz, 1.0);
}

