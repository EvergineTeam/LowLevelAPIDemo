#version 450

layout(binding = 0, std140) uniform type_Matrices
{
    mat4 viewProj;
} Matrices;

layout(location = 0) in vec3 in_var_POSITION;
layout(location = 1) in vec3 in_var_NORMAL;
layout(location = 2) in vec2 in_var_TEXCOORD0;
layout(location = 3) in vec4 in_var_TEXCOORD1;
layout(location = 4) in vec4 in_var_TEXCOORD2;
layout(location = 5) in vec4 in_var_TEXCOORD3;
layout(location = 6) in vec4 in_var_TEXCOORD4;
layout(location = 7) in int in_var_TEXCOORD5;
layout(location = 0) out vec3 out_var_NORMAL;
layout(location = 1) out vec3 out_var_TEXCOORD;

void main()
{
    mat4 _47 = Matrices.viewProj * mat4(in_var_TEXCOORD1, in_var_TEXCOORD2, in_var_TEXCOORD3, in_var_TEXCOORD4);
    vec3 _61 = vec3(in_var_TEXCOORD0.x, in_var_TEXCOORD0.y, vec3(0.0).z);
    _61.z = float(in_var_TEXCOORD5);
    gl_Position = _47 * vec4(in_var_POSITION, 1.0);
    out_var_NORMAL = (_47 * vec4(in_var_NORMAL, 0.0)).xyz;
    out_var_TEXCOORD = _61;
}

