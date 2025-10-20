cbuffer Matrices : register(b0)
{
    float4x4 worldViewProj;
    float4x4 worldViewProj2;
};

struct Meshlet {
    uint VertexOffset;
    uint TriangleOffset;
    uint VertexCount;
    uint TriangleCount;
};

struct MeshletBounds {
    float3 center;
    float  radius;
};

StructuredBuffer<float3>  Vertices             : register(t1);
StructuredBuffer<Meshlet> Meshlets             : register(t2);
StructuredBuffer<uint>    VertexIndices        : register(t3);
StructuredBuffer<uint>    TriangleIndices      : register(t4);
StructuredBuffer<MeshletBounds> Bounds         : register(t5);

struct MeshOutput {
    float4 Position : SV_POSITION;
    float3 Color : COLOR;
};

struct Payload {
    uint meshletIndex;
};

groupshared Payload g_Payload;
groupshared uint    g_DispatchCount;

[shader("amplification")]
[numthreads(1, 1, 1)]
void AS(
    uint3 dispatchID : SV_DispatchThreadID)
{
    uint idx = dispatchID.x;
    g_Payload.meshletIndex = idx;

    MeshletBounds mb = Bounds[idx];

    float4 clipC = mul(float4(mb.center, 1.0f), worldViewProj2);

    bool visible =
        clipC.x >= -clipC.w && clipC.x <= clipC.w &&   // -w <= x <= w
        clipC.y >= -clipC.w && clipC.y <= clipC.w &&   // -w <= y <= w
        clipC.z >= 0 && clipC.z <= clipC.w;   //  0 <= z <= w

    g_DispatchCount = visible ? 1 : 0;

    GroupMemoryBarrierWithGroupSync();

    DispatchMesh(g_DispatchCount, 1, 1, g_Payload);
}

[shader("mesh")]
[outputtopology("triangle")]
[numthreads(32, 1, 1)]
void MS(
    uint       gtid : SV_GroupIndex,
    in payload Payload payload,
    out indices  uint3      triangles[128],
    out vertices MeshOutput vertices[64])
{
    Meshlet m = Meshlets[payload.meshletIndex];
    SetMeshOutputCounts(m.VertexCount, m.TriangleCount);

    int index = gtid * 4;
    int end = index + 4;
    if (index < m.TriangleCount)
    {
        for (int i = index; i < end; i++) {
            uint packed = TriangleIndices[m.TriangleOffset + i];
            uint vIdx0 = (packed >> 0) & 0xFF;
            uint vIdx1 = (packed >> 8) & 0xFF;
            uint vIdx2 = (packed >> 16) & 0xFF;
            triangles[i] = uint3(vIdx0, vIdx1, vIdx2);
        }
    }

    if (index < m.VertexCount) {
        for (int v = index; v < end; v++) {
            uint vertexIndex = m.VertexOffset + v;
            vertexIndex = VertexIndices[vertexIndex];

            vertices[v].Position = mul(float4(Vertices[vertexIndex], 1), worldViewProj);

            float3 color = float3(
                float(payload.meshletIndex & 1),
                float(payload.meshletIndex & 3) / 4,
                float(payload.meshletIndex & 7) / 8);
            vertices[v].Color = color;
        }
    }
}

[shader("pixel")]
float4 PS(MeshOutput input) : SV_TARGET
{
    return float4(input.Color, 1);
}