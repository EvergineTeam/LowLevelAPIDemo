struct typeGlobals {
    worldViewProj: mat4x4<f32>,
}

struct VertexOutput {
    @builtin(position) member: vec4<f32>,
    @location(0) member_1: vec2<f32>,
}

@group(0) @binding(0) 
var<uniform> Globals: typeGlobals;
var<private> invarPOSITION_1: vec4<f32>;
var<private> invarTEXCOORD_1: vec2<f32>;
var<private> global: vec4<f32> = vec4<f32>(0f, 0f, 0f, 1f);
var<private> outvarTEXCOORD: vec2<f32>;

fn VS_1() {
    let _e6 = invarPOSITION_1;
    let _e7 = invarTEXCOORD_1;
    let _e9 = Globals.worldViewProj;
    global = (_e9 * _e6);
    outvarTEXCOORD = _e7;
    return;
}

@vertex 
fn VS(@location(0) invarPOSITION: vec4<f32>, @location(1) invarTEXCOORD: vec2<f32>) -> VertexOutput {
    invarPOSITION_1 = invarPOSITION;
    invarTEXCOORD_1 = invarTEXCOORD;
    VS_1();
    let _e6 = global;
    let _e7 = outvarTEXCOORD;
    return VertexOutput(_e6, _e7);
}
