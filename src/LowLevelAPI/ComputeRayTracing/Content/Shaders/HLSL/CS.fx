struct Camera
{
	float3 Origin;
	float LensRadius;
	float3 LowerLeftCorner;
	uint Padding0;
	float3 Horizontal;
	uint Padding1;
	float3 Vertical;
	uint Padding2;
	float3 U;
	uint Padding3;
	float3 V;
	uint Padding4;
	float3 W;
	uint Padding5;
};

struct ComputeData
{
	float time;
	float width;
	float height;
	uint framecount;
	uint samples;
	uint recursion;
	uint spherecount;
	uint padding0;
	Camera cam;
};

struct Sphere
{
	float3 center;
	float radius;
};

struct Material
{
	float3 Albedo;
	float RefIndex;
	int Type;
	uint padding0;
	uint padding1;
	uint padding2;
};

StructuredBuffer<Sphere> Spheres : register(t0);
StructuredBuffer<Material> Materials : register(t1);

cbuffer ParamsBuffer : register(b0)
{
	ComputeData data;
}

RWTexture2D<float4> Output : register(u0);

//Helpers Functions
uint wang_hash(inout uint seed)
{
	seed = (seed ^ 61) ^ (seed >> 16);
	seed *= 9;
	seed = seed ^ (seed >> 4);
	seed *= 0x27d4eb2d;
	seed = seed ^ (seed >> 15);
	return seed;
}
uint XorShift(inout uint state)
{
	state ^= state << 13;
	state ^= state >> 17;
	state ^= state << 15;
	return state;
}

float RandomFloat(inout uint state)
{
	return wang_hash(state) * (1.f / 4294967296.f);
}

float3 RandomInUnitSphere(inout uint seed)
{
	float3 hash3 = float3(RandomFloat(seed), RandomFloat(seed), RandomFloat(seed));
	float3 h = hash3 * float3(2., 6.28318530718, 1.) - float3(1, 0, 0);
	float phi = h.y;
	float r = pow(h.z, 1. / 3.0);
	return r * float3(sqrt(1.0 - h.x * h.x) * float2(sin(phi), cos(phi)), h.x);
}

// Ray
struct Ray
{
	float3 Origin;
	float3 Direction;
};

Ray Ray_Create(float3 origin, float3 direction)
{
	Ray r;
	r.Origin = origin;
	r.Direction = direction;
	return r;
}

float3 Ray_PointAt(Ray r, float t)
{
	return r.Origin + r.Direction * t;
}

// Hit Record
struct RayHit
{
	float3 Position;
	float T;
	float3 Normal;
};

RayHit RayHit_Create(float3 position, float t, float normal)
{
	RayHit hit;
	hit.Position = position;
	hit.T = t;
	hit.Normal = normal;

	return hit;
}

// Hitable
struct Hitable
{
	float3 Center;
	float Radius;
};

bool HitableHit(const in float sphereRadius, const in float3 sphereCenter, const in Ray r, const in float tmin, const in float tmax, inout RayHit hit)
{
	float3 oc = r.Origin - sphereCenter;
	float a = dot(r.Direction, r.Direction);
	float b = dot(oc, r.Direction);
	float c = dot(oc, oc) - sphereRadius * sphereRadius;
	float discriminant = b * b - a * c;
	float tmp = sqrt(discriminant);

	if (discriminant > 0.0)
	{
		float t = (-b - tmp) / a;
		if (t < tmax && t > tmin)
		{
			hit.T = t;
			hit.Position = Ray_PointAt(r, hit.T);
			hit.Normal = (hit.Position - sphereCenter) / sphereRadius;
			return true;
		}
	}
	else
	{
		float t = (-b + tmp) / a;
		if (t < tmax && t > tmin)
		{
			hit.T = t;
			hit.Position = Ray_PointAt(r, hit.T);
			hit.Normal = (hit.Position - sphereCenter) / sphereRadius;
			return true;
		}
	}

	hit.Position = float3(0, 0, 0);
	hit.Normal = float3(0, 0, 0);
	hit.T = 0;
	return false;
}

Ray Camera_GetRay(Camera c, float2 uv, inout uint seed)
{
	float3 rd = c.LensRadius * RandomInUnitSphere(seed);
	float3 offset = c.U * rd.x + c.V * rd.y;
	return Ray_Create(c.Origin + offset, c.LowerLeftCorner + uv.x * c.Horizontal + uv.y * c.Vertical - c.Origin - offset);
}

// Color & Scene
float Schlick(float cosine, float refIndex)
{
	float r0 = (1.0 - refIndex) / (1.0 + refIndex);
	r0 = r0 * r0;
	return r0 + (1.0 - r0) * pow(1.0 - cosine, 5.0);
}

bool Refract(float3 view, float3 normal, float niOverNt, out float3 refracted)
{
	float3 uv = normalize(view);
	float dt = dot(uv, normal);
	float discriminant = 1.0 - niOverNt * niOverNt * (1.0 - dt * dt);

	if (discriminant > 0)
	{
		refracted = niOverNt * (uv - normal * dt) - normal * sqrt(discriminant);
		return true;
	}
	else
	{
		refracted = float3(0.0, 0.0, 0.0);
		return false;
	}
}

bool Ray_Scatter(Ray r, RayHit hit, Material m, inout uint seed, out float3 attenuation, out Ray scattered)
{
	switch (m.Type)
	{
		case 0:
		{
			scattered = Ray_Create(hit.Position, hit.Normal + RandomInUnitSphere(seed));
			attenuation = m.Albedo;
			return true;
		}
		case 1:
		{
			float3 reflected = reflect(normalize(r.Direction), hit.Normal);
			scattered = Ray_Create(hit.Position, reflected + m.RefIndex * RandomInUnitSphere(seed));
			attenuation = m.Albedo;
			return dot(scattered.Direction, hit.Normal) > 0;
		}
		case 2:
		{
			float3 outwardNormal;
			float niOverNt;
			attenuation = float3(1.0, 1.0, 1.0);
			float reflectProb;
			float3 refracted;
			float cosine;


			if (dot(r.Direction, hit.Normal) > 0)
			{
				outwardNormal = -hit.Normal;
				niOverNt = m.RefIndex;
				cosine = m.RefIndex * dot(r.Direction, hit.Normal) / length(r.Direction);
			}
			else
			{
				outwardNormal = hit.Normal;
				niOverNt = m.RefIndex;
				cosine = -dot(r.Direction, hit.Normal) / length(r.Direction);
			}

			if (Refract(r.Direction, outwardNormal, niOverNt, refracted))
			{
				reflectProb = Schlick(cosine, m.RefIndex);
			}
			else
			{
				reflectProb = 1.0;
			}

			if (RandomFloat(seed) < reflectProb)
			{
				scattered = Ray_Create(hit.Position, reflect(r.Direction, hit.Normal));
			}
			else
			{
				scattered = Ray_Create(hit.Position, refracted);
			}

			return true;
		}
		default:
		{
			attenuation = float3(0.0, 0.0, 0.0);
			scattered = Ray_Create(float3(0.0, 0.0, 0.0), float3(0.0, 0.0, 0.0));
			return false;
		}
	}
}

float3 ComputeColor(in Ray r, inout uint seed)
{
	float3 color = float3(0.0, 0.0, 0.0);
	float3 currentAttenuation = float3(1.0, 1.0, 1.0);
	for (uint depth = 0; depth < data.recursion; depth++)
	{
		RayHit hit;
		hit.Position = float3(0.0, 0.0, 0.0);
		hit.Normal = float3(0.0, 0.0, 0.0);
		hit.T = 0;
		float tmax = 9999999.0;
		bool hitAnything = false;
		uint hitID = 0;
		for (uint i = 0; i < data.spherecount; i++)
		{
			RayHit tempHit;
			if (HitableHit(Spheres[i].radius, Spheres[i].center, r, 0.001, tmax, tempHit))
			{
				hitAnything = true;
				hit = tempHit;
				hitID = i;
				tmax = hit.T;
			}
		}

		if (hitAnything)
		{
			float3 attenuation;
			Ray scattered;
			if (Ray_Scatter(r, hit, Materials[hitID], seed, attenuation, scattered))
			{
				currentAttenuation *= attenuation;
				r = scattered;
			}
			else
			{
				color += currentAttenuation;
				break;
			}
		}
		else
		{
			float3 unitDirection = normalize(r.Direction);
			float t = 0.5 * unitDirection.y + 0.5;
			color += currentAttenuation * lerp(float3(1, 1, 1), float3(0.5, 0.7, 1.0), t);
			break;
		}
	}

	return color;
}

[numthreads(8, 8, 1)]
void CS(uint3 threadID : SV_DispatchThreadID)
{
	uint seed = (threadID.x * 1973 + threadID.y * 9277 + data.framecount * 26699) | 1;

	float aspect = data.width / data.height;

	float3 color = float3(0.0, 0.0, 0.0);

	for (uint smp = 0; smp < data.samples; smp++)
	{
		float u = (threadID.x + RandomFloat(seed)) / data.width;
		float v = (threadID.y + RandomFloat(seed)) / data.height;
		v = 1 - v;

		Ray r = Camera_GetRay(data.cam, float2(u, v), seed);
		color += ComputeColor(r, seed);
	}

	color /= float(data.samples);

	Output[threadID.xy] = float4(color, 1.0);
}