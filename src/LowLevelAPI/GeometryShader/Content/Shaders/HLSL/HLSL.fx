
float4x4 worldViewProj;

struct VS_IN
{
	float4 pos : POSITION;
	float4 col : COLOR;
};

struct GS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
	float4 col : COLOR;
};

GS_IN VS(VS_IN input)
{
	GS_IN output = (GS_IN)0;

	output.pos = input.pos;
	output.col = input.col;

	return output;
}

[maxvertexcount(93)]
void GS(triangle GS_IN input[3], inout TriangleStream<PS_IN> triStream)
{
	PS_IN v;

	// add original geometry
	for (int i = 0; i < 3; i++)
	{
		v.pos = mul(input[i].pos, worldViewProj);
		v.col = input[i].col;
		triStream.Append(v);
	}

	triStream.RestartStrip();

	for (int i = 1; i <= 10; i++)
	{
		for (int j = 0; j < 3; j++)
		{
			v.pos = mul(input[j].pos + float4(0, i / 10.0, 0, 0), worldViewProj);
			v.col = input[j].col;
			triStream.Append(v);
		}

		triStream.RestartStrip();
	}
}

float4 PS(PS_IN input) : SV_Target
{
	return input.col;
}
