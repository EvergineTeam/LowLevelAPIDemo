#version 460
layout(local_size_x = 8, local_size_y = 8, local_size_z = 1) in;

struct ComputeData
{
    float time;
    float width;
    float height;
    float padding;
};

layout(binding = 0, std140) uniform type_ParamsBuffer
{
    ComputeData data;
} ParamsBuffer;

layout(binding = 20, rgba32f) uniform writeonly image2D Output;

void main()
{
    vec2 _34 = vec2(ParamsBuffer.data.width, ParamsBuffer.data.height);
    imageStore(Output, ivec2(gl_GlobalInvocationID.xy), vec4((vec2(gl_GlobalInvocationID.xy) / _34) + (vec2(0.5) / _34), 0.5 + (0.5 * sin(ParamsBuffer.data.time)), 1.0));
}

