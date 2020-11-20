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
	return DiffuseTexture.Sample(Sampler, input.tex);
}
