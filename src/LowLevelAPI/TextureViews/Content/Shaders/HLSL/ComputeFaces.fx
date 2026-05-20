cbuffer ParamsBuffer : register(b0)
{
    float time;
    float resolution;
};
RWTexture2DArray<float4> Output : register(u0);

#define PI 3.1415926535897932384626433832795

// --- EFFECT 0 ---------------------------------------------------------------------------
// https://www.shadertoy.com/view/XsXXDn
float4 effect_0(float2 tc)
{
    float3 c;
    float z = time;
    float l;
    for (int i = 0; i < 3; i++)
    {
        float2 uv, p = tc;
        uv = p;
        p -= 0.5;
        z += 0.07;
        l = length(p);
        uv += p / l * (sin(z) + 1.) * abs(sin(l * 9.0 - z - z));
        c[i] = .01 / length(frac(uv) - .5);
    }
    return float4(c / l, 1);
}

// --- EFFECT 1 ---------------------------------------------------------------------------
// https://www.shadertoy.com/view/mtyGWy
float3 palette(float t)
{
    float3 a = float3(0.5, 0.5, 0.5);
    float3 b = float3(0.5, 0.5, 0.5);
    float3 c = float3(1.0, 1.0, 1.0);
    float3 d = float3(0.263, 0.416, 0.557);

    return a + b * cos(6.28318 * (c * t + d));
}
float4 effect_1(float2 tc)
{
    float2 uv = tc;
    float3 finalColor = float3(0, 0, 0);
    
    for (float i = 0.0; i < 4.0; i++)
    {
        uv = frac(uv * 1.5) - 0.5;

        float d = length(uv) * exp(-length(tc));

        float3 col = palette(length(tc) + i * 0.4 + time * 0.4);

        d = sin(d * 8. + time) / 8.;
        d = abs(d);

        d = pow(0.01 / d, 1.2);

        finalColor += col * d;
    }
        
    return float4(finalColor, 1.0);
}

// --- EFFECT 2 -----------------------------------------------------------------------------
// https://www.shadertoy.com/view/3tyBzV
float dot2(in float2 v)
{
    return dot(v, v);
}
float sdHeart(in float2 p)
{
    p.x = abs(p.x);

    if (p.y + p.x > 1.0)
        return sqrt(dot2(p - float2(0.25, 0.75))) - sqrt(2.0) / 4.0;
    return sqrt(min(dot2(p - float2(0.00, 1.00)),
                    dot2(p - 0.5 * float2(p.x + p.y, 0.0)))) * sign(p.x - p.y);
}
float4 effect_2(float2 tc)
{
    float2 p = 2 * (tc - 0.5);
    p = (1.0 + 0.2 * pow(abs(sin(2.0 * time)), 10.0)) * p;
    p.y += 0.5;
    float d = sdHeart(p);
    return d < 0 ? float4(1, 0, 0, 1) : float4(0, 0, 0, 1);
}

// --- EFFECT 3 -----------------------------------------------------------------------------
// https://www.shadertoy.com/view/4dlGRM

float4 effect_3(float2 tc)
{
    float period = 5.0;
    float twist = 500.0;
    float rotation = time * 12.0;
    float3 background = float3(0.0, 0.2, 0.0);
    float3 foreground = float3(0.3, 0.6, 0.3);
		
    float2 shift = tc - float2(0.5, 0.5);
        
    float offset = rotation + sqrt(dot(shift, shift)) * twist / 10.0;
    float val = sin((offset + atan2(shift.x, shift.y) * period));

    return float4(lerp(background, foreground, val), 1.0);
}

// --- EFFECT 4 ------------------------------------------------------------------------------
// https://www.shadertoy.com/view/4ssGWn
#define glsl_mod(x, y) ((x) - (y) * floor((x) / (y)))
float4 effect_4(float2 tc)
{
    const float2 res = float2(320.0, 200.0);
    const float3x3 mRot = float3x3(0.9553, -0.2955, 0.0, 0.2955, 0.9553, 0.0, 0.0, 0.0, 1.0);
    const float3 ro = float3(0.0, 0.0, -4.0);
    const float3 cRed = float3(1.0, 0.0, 0.0);
    const float3 cWhite = float3(1.0, 1.0, 1.0);
    const float3 cGrey = float3(0.66, 0.66, 0.66);
    const float3 cPurple = float3(0.51, 0.29, 0.51);
    const float maxx = 0.378;
    
    float2 uv = tc;
    float2 uvR = floor(uv * res);
    float2 g = step(2.0, glsl_mod(uvR, 16.0));
    float3 bgcol = lerp(cPurple, lerp(cPurple, cGrey, g.x), g.y);
    uv = uvR / res;
    float xt = glsl_mod(time + 1.0, 6.0);
    float dir = (step(xt, 3.0) - .5) * -2.0;
    uv.x -= (maxx * 2.0 * dir) * glsl_mod(xt, 3.0) / 3.0 + (-maxx * dir);
    uv.y -= abs(sin(4.5 + time * 1.3)) * 0.5 - 0.3;
    bgcol = lerp(bgcol, bgcol - float3(0.2, 0.2, 0.2), 1.0 - step(0.12, length(float2(uv.x, uv.y) - float2(0.57, 0.29))));
    float3 rd = normalize(float3((uv * 2.0 - 1.0) * float2(1.0, 1.0), 1.5));
    float b = dot(rd, ro);
    float t1 = b * b - 15.6;
    float t = -b - sqrt(t1);
    float3 nor = mul(mRot, normalize(ro + rd * t));
    float2 tuv = floor(float2(atan2(nor.x, nor.z) / PI + ((floor((time * -dir) * 60.0) / 60.0) * 0.5), acos(nor.y) / PI) * 8.0);
    return float4(lerp(bgcol, lerp(cRed, cWhite, clamp(glsl_mod(tuv.x + tuv.y, 2.0), 0.0, 1.0)), 1.0 - step(t1, 0.0)), 1.0);
}

// --- EFFECT 5 ------------------------------------------------------------------------------
// https://www.shadertoy.com/view/3XtfW7

#define e5_wave_amplitude 0.076
#define e5_period (2.0 * PI)

float e5_square(float2 st)
{
    float2 bl = step(float2(0, 0), st); // bottom-left
    float2 tr = step(float2(0, 0), 1.0 - st); // top-right
    return bl.x * bl.y * tr.x * tr.y;
}

float4 e5_frame(float2 st)
{
    float tushka = e5_square(mul(float2x2((1. / .48), 0., 0., (1. / .69)), st));
    
    float2x2 sector_mat = float2x2(1. / .16, 0., 0., 1. / .22);
    float sectors[4];
    sectors[0] = e5_square(mul(sector_mat, st) + (1. / .16) * float2(0.000, -0.280));
    sectors[1] = e5_square(mul(sector_mat, st) + (1. / .16) * float2(0.000, -0.060));
    sectors[2] = e5_square(mul(sector_mat, st) + (1. / .16) * float2(-0.240, -0.280));
    sectors[3] = e5_square(mul(sector_mat, st) + (1. / .16) * float2(-0.240, -0.060));
    float3 sector_colors[4];
    sector_colors[0] = float3(0.941, 0.439, 0.404) * sectors[0];
    sector_colors[1] = float3(0.435, 0.682, 0.843) * sectors[1];
    sector_colors[2] = float3(0.659, 0.808, 0.506) * sectors[2];
    sector_colors[3] = float3(0.996, 0.859, 0.114) * sectors[3];
    
    return float4(float3(sector_colors[0] + sector_colors[1] +
                     sector_colors[2] + sector_colors[3]), tushka);
}

float4 e5_trail_piece(float2 st, float2 index, float scale)
{
    scale = index.x * 0.082 + 0.452;
    
    float3 color;
    if (index.y > 0.9 && index.y < 2.1)
    {
        color = float3(0.435, 0.682, 0.843);
        scale *= .8;
    }
    else if (index.y > 3.9 && index.y < 5.1)
    {
        color = float3(0.941, 0.439, 0.404);
        scale *= .8;
    }
    else
    {
        color = float3(0., 0., 0.);
    }
    
    float scale1 = 1. / scale;
    float shift = -(1. - scale) / (2. * scale);
    float2 st2 = mul(float3x3(scale1, 0., shift, 0., scale1, shift, 0., 0., 1.), float3(st, 1.)).xy;
    float mask = e5_square(st2);

    return float4(color, mask);
}

float4 e5_trail(float2 st)
{
    // actually 1/width, 1/height
    const float piece_height = 7. / .69;
    const float piece_width = 6. / .54;
  
    // make distance between smaller segments slightly lower
    st.x = 1.2760 * pow(st.x, 3.0) - 1.4624 * st.x * st.x + 1.4154 * st.x;
    
    float x_at_cell = floor(st.x * piece_width) / piece_width;
    float x_at_cell_center = x_at_cell + 0.016;
    float incline = cos(0.5 * e5_period + time) * e5_wave_amplitude;
    
    float offset = sin(x_at_cell_center * e5_period + time) * e5_wave_amplitude +
        incline * (st.x - x_at_cell) * 5.452;
    
    float mask = step(offset, st.y) * (1. - step(.69 + offset, st.y)) * step(0., st.x);
    
    float2 cell_coord = float2((st.x - x_at_cell) * piece_width,
                           frac((st.y - offset) * piece_height));
    float2 cell_index = float2(x_at_cell * piece_width,
                           floor((st.y - offset) * piece_height));
    
    float4 pieces = e5_trail_piece(cell_coord, cell_index, 0.752);
    
    return float4(pieces.xyz, pieces.a * mask);
}

float4 e5_logo(float2 st)
{
    if (st.x <= .54) {
        return e5_trail(st);
    }
    else {
        float2 st2 = st + float2(0., -sin(st.x * e5_period + time) * e5_wave_amplitude);
        return e5_frame(st2 + float2(-.54, 0));
    }
}

float4 effect_5(float2 tc)
{
    float2 st = tc;

    st *= 1.6;
    st += float2(-0.2, -0.7);
    float rot = PI * -0.124;
    st = mul(float2x2(cos(rot), sin(rot), -sin(rot), cos(rot)), st);
    float3 color = float3(1, 1, 1);
    
    float4 logo_ = e5_logo(st);
    return lerp(float4(0., .5, .5, 1.000), logo_, logo_.a);
}

// --- MAIN -----------------------------------------------------------------------------
[numthreads(8, 8, 1)]
void CS(uint3 threadID : SV_DispatchThreadID)
{
    float4 faceColors[6] =
    {
        float4(1, 0, 0, 1),
        float4(0, 1, 0, 1),
        float4(0, 0, 1, 1),
        float4(1, 1, 0, 1),
        float4(1, 0, 1, 1),
        float4(0, 1, 1, 1),
    };
    
    float2 tc = threadID.xy * (1.0 / resolution);
    
    float4 color;
    const int lineThickness = 2;
    if (threadID.x < lineThickness || threadID.x >= resolution - lineThickness || threadID.y < lineThickness || threadID.y >= resolution - lineThickness)
        color = float4(0.3, 0.3, 0.3, 3);
    else {
        int face = threadID.z;
        color = faceColors[threadID.z];
        if (face == 0)
            color = effect_0(tc);
        else if (face == 1)
            color = effect_1(tc);
        else if (face == 2)
            color = effect_2(tc);
        else if (face == 3)
            color = effect_3(tc);
        else if (face == 4)
            color = effect_4(tc);
        else //if (face == 5)
            color = effect_5(tc);
    }
    color.xyz = pow(color.xyz, 1.0/2.2);
	Output[threadID.xyz] = color;
    }