#version 450

layout(binding = 0, std140) uniform type_Matrices
{
    mat4 worldViewProj;
} Matrices;

layout(location = 0) in vec4 in_var_POSITION;
layout(location = 1) in vec4 in_var_COLOR;
layout(location = 2) in vec2 in_var_TEXCOORD;
layout(location = 0) out vec4 out_var_COLOR;
layout(location = 1) out vec2 out_var_TEXCOORD;

void main()
{
    gl_Position = Matrices.worldViewProj * in_var_POSITION;
    out_var_COLOR = in_var_COLOR;
    out_var_TEXCOORD = in_var_TEXCOORD;
}

