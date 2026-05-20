#version 450

layout(binding = 0, std140) uniform type_Matrices
{
    mat4 worldViewProj;
} Matrices;

layout(location = 0) in vec3 in_var_POSITION;
layout(location = 0) out vec3 out_var_TEXCOORD0;

void main()
{
    gl_Position = Matrices.worldViewProj * vec4(in_var_POSITION, 1.0);
    out_var_TEXCOORD0 = in_var_POSITION;
}

