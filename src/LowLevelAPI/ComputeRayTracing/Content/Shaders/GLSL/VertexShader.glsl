#version 450

layout(location = 0) out vec2 out_var_TEXCOORD;

void main()
{
    vec2 _30 = vec2(float((uint(gl_VertexID) << 1u) & 2u), float(uint(gl_VertexID) & 2u));
    gl_Position = vec4((_30 * vec2(2.0, -2.0)) + vec2(-1.0, 1.0), 0.0, 1.0);
    out_var_TEXCOORD = _30;
}

