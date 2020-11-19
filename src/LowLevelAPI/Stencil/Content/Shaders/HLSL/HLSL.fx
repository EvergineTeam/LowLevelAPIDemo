
cbuffer Matrices : register(b0)
{
	bool IsTextured				: packoffset(c0);
	float4x4 worldViewProj		: packoffset(c1);
};

Texture2D DiffuseTexture 			: register(t0);
SamplerState Sampler			 	: register(s0);

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
	float2 tex : TEXCOORD;
};

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.pos = mul(input.pos, worldViewProj);
	output.col = input.col;
	output.tex = input.tex;

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	if (IsTextured)
		return DiffuseTexture.Sample(Sampler, input.tex);
	else
		return input.col;
}
