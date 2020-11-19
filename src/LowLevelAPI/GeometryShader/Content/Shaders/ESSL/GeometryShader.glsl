#version 310 es
#extension GL_EXT_geometry_shader : require
layout(triangles) in;
layout(max_vertices = 93, triangle_strip) out;

struct GS_IN
{
    vec4 pos;
    vec4 col;
};

layout(binding = 0, std140) uniform type_Globals
{
    mat4 worldViewProj;
} _Globals;

layout(location = 0) in vec4 in_var_COLOR[3];
layout(location = 0) out vec4 out_var_COLOR;

void main()
{
    GS_IN param_var_input[3] = GS_IN[](GS_IN(gl_Position[0], in_var_COLOR[0]), GS_IN(gl_Position[1], in_var_COLOR[1]), GS_IN(gl_Position[2], in_var_COLOR[2]));
    for (int _47 = 0; _47 < 3; )
    {
        gl_Position = _Globals.worldViewProj * param_var_input[_47].pos;
        out_var_COLOR = param_var_input[_47].col;
        EmitVertex();
        _47++;
        continue;
    }
    EndPrimitive();
    int _60;
    _60 = 1;
    for (; _60 <= 10; EndPrimitive(), _60++)
    {
        for (int _67 = 0; _67 < 3; )
        {
            gl_Position = _Globals.worldViewProj * (param_var_input[_67].pos + vec4(0.0, float(_60) * 0.100000001490116119384765625, 0.0, 0.0));
            out_var_COLOR = param_var_input[_67].col;
            EmitVertex();
            _67++;
            continue;
        }
    }
}

