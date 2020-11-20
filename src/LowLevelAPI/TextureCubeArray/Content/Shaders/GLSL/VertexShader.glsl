#version 450

layout(binding = 0, std140) uniform type_Matrices
{
    mat4 worldViewProj;
    mat4 World;
    mat4 WorldInverseTranspose;
    vec3 CameraPosition;
} Matrices;

layout(location = 0) in vec4 in_var_POSITION;
layout(location = 1) in vec3 in_var_NORMAL;
layout(location = 2) in vec2 in_var_TEXCOORD;
layout(location = 0) out vec4 out_var_TEXCOORD0;
layout(location = 1) out vec3 out_var_TEXCOORD1;
layout(location = 2) out vec3 out_var_TEXCOORD2;

void main()
{
    vec4 _37 = Matrices.worldViewProj * in_var_POSITION;
    gl_Position = _37;
    out_var_TEXCOORD0 = _37;
    out_var_TEXCOORD1 = (Matrices.World * in_var_POSITION).xyz - Matrices.CameraPosition;
    out_var_TEXCOORD2 = mat3(Matrices.WorldInverseTranspose[0].xyz, Matrices.WorldInverseTranspose[1].xyz, Matrices.WorldInverseTranspose[2].xyz) * in_var_NORMAL;
}

