cbuffer Matrices : register(b0)
{
	float4x4 worldViewProj			: packoffset(c0);
	float4x4 World					: packoffset(c4);
	float4x4 WorldInverseTranspose	: packoffset(c8);
	float3 CameraPosition			: packoffset(c12);
};

TextureCubeArray CubeTexture 	: register(t0);
SamplerState Sampler			: register(s0);

struct VS_IN
{
	float4 Position : POSITION;
	float3 Normal	: NORMAL;
	float2 TexCoord : TEXCOORD;
};

struct PS_IN
{
	float4 Position		: SV_POSITION;
	float4 PositionCS	: TEXCOORD0;
	float3 CameraVector	: TEXCOORD1;
	float3 NormalWS		: TEXCOORD2;
};

PS_IN VS(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	output.Position = mul(input.Position, worldViewProj);

	output.PositionCS = output.Position;

	float3 positionWS = mul(input.Position, World).xyz;

	output.CameraVector = positionWS - CameraPosition;	

	output.NormalWS = mul(input.Normal, (float3x3)WorldInverseTranspose);

	return output;
}

inline float2 ComputeScreenPosition(float4 pos)
{
	float2 screenPos = pos.xy / pos.w;
	return (0.5f * (float2(screenPos.x, -screenPos.y) + 1));
}

float4 PS(PS_IN input) : SV_Target
{
	float3 nomalizedCameraVector = normalize(input.CameraVector);
	float3 normal = normalize(input.NormalWS);
	float3 envCoord = reflect(nomalizedCameraVector, normal);

	float2 screenPosition = ComputeScreenPosition(input.PositionCS);
	float index = (float)(int)(screenPosition.x * 4.0f);

	float3 enviromentMap = CubeTexture.Sample(Sampler, float4(envCoord, index)).xyz;

	return float4(enviromentMap, 1);
}
