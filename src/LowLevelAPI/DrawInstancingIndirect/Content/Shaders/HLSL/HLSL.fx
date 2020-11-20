cbuffer Matrices : register(b0)
{
	float4x4 viewProj;
}
Texture2DArray DiffuseTexture : register(t0);
SamplerState Sampler          : register(s0);

struct VS_IN
{	
	float3 pos : POSITION;
	float3 nor: NORMAL;
	float2 tex : TEXCOORD0;

	//Per instance data
	float4 instanceMatrix0 : TEXCOORD1;
	float4 instanceMatrix1 : TEXCOORD2;
	float4 instanceMatrix2 : TEXCOORD3;
	float4 instanceMatrix3 : TEXCOORD4;
	int instanceTextIdx : TEXCOORD5;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float3 nor: NORMAL;
	float3 tex : TEXCOORD;
};

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	float4x4 world = float4x4(input.instanceMatrix0, input.instanceMatrix1, input.instanceMatrix2, input.instanceMatrix3);

	float4x4 MVP = mul(world, viewProj);
	output.pos = mul(float4(input.pos, 1), MVP);
	output.nor = mul(float4(input.nor, 0), MVP).xyz;
	output.tex.xy = input.tex.xy;
	output.tex.z = input.instanceTextIdx; //text index on z

	return output;
}

float4 PS(PS_IN input) : SV_Target
{
	float4 ret = DiffuseTexture.Sample(Sampler, input.tex);
	clip(ret.a - 0.5f); //alpha test
	
	float ambient = 0.5;
	float diffuse = saturate(dot(input.nor, float3(0, 0, -1)));
	return ret * (ambient + diffuse * 0.5);
}
