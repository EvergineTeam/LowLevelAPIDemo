#version 450

layout(binding = 0, std140) uniform type_Globals
{
    mat4 worldViewProj;
} _Globals;

layout(location = 0) in vec4 in_var_POSITION;
layout(location = 1) in vec4 in_var_COLOR;
layout(location = 0) out vec4 out_var_COLOR;

void main()
{
    gl_Position = _Globals.worldViewProj * in_var_POSITION;
    out_var_COLOR = in_var_COLOR;
}

