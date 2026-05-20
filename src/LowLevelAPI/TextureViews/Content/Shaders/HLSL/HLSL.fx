cbuffer Matrices : register(b0)
{
    float4x4 worldViewProj : packoffset(c0);
};

TextureCube CubeTexture : register(t0);
SamplerState Sampler : register(s0);

struct VS_IN
{
    float3 Position : POSITION;
};

struct PS_IN
{
    float4 Position : SV_POSITION;
    float3 SampleDir : TEXCOORD0;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;

    output.Position = mul(float4(input.Position, 1), worldViewProj);
    output.SampleDir = input.Position;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    return CubeTexture.Sample(Sampler, input.SampleDir);
}
