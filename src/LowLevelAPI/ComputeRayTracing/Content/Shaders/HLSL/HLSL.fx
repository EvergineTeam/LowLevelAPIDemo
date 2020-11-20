cbuffer ParamsBuffer : register(b0)
{
	uint Samples;
	bool IsPathTracing;
}

Texture2D DiffuseTexture 			: register(t0);
SamplerState Sampler			 	: register(s0);

struct PS_IN
{
	float4 pos : SV_POSITION;
	float2 tex : TEXCOORD;
};

PS_IN VS(uint id: SV_VertexID)
{
	PS_IN output = (PS_IN)0;

	output.tex = float2((id << 1) & 2, id & 2);
	output.pos = float4(output.tex * float2(2, -2) + float2(-1, 1), 0, 1);

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float3 color = DiffuseTexture.Sample(Sampler, input.tex).rgb;
	float a = IsPathTracing ? 1.0 / Samples : 1.0;

	return float4(color, a);
}
