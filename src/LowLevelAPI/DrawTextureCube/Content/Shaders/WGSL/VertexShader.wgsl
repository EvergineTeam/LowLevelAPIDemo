struct typeMatrices {
    worldViewProj: mat4x4<f32>,
}

struct VertexOutput {
    @builtin(position) member: vec4<f32>,
    @location(0) member_1: vec4<f32>,
    @location(1) member_2: vec2<f32>,
}

@group(0) @binding(0) 
var<uniform> Matrices: typeMatrices;
var<private> invarPOSITION_1: vec4<f32>;
var<private> invarCOLOR_1: vec4<f32>;
var<private> invarTEXCOORD_1: vec2<f32>;
var<private> global: vec4<f32> = vec4<f32>(0f, 0f, 0f, 1f);
var<private> outvarCOLOR: vec4<f32>;
var<private> outvarTEXCOORD: vec2<f32>;

fn VS_1() {
    let _e8 = invarPOSITION_1;
    let _e9 = invarCOLOR_1;
    let _e10 = invarTEXCOORD_1;
    let _e12 = Matrices.worldViewProj;
    global = (_e12 * _e8);
    outvarCOLOR = _e9;
    outvarTEXCOORD = _e10;
    return;
}

@vertex 
fn VS(@location(0) invarPOSITION: vec4<f32>, @location(1) invarCOLOR: vec4<f32>, @location(2) invarTEXCOORD: vec2<f32>) -> VertexOutput {
    invarPOSITION_1 = invarPOSITION;
    invarCOLOR_1 = invarCOLOR;
    invarTEXCOORD_1 = invarTEXCOORD;
    VS_1();
    let _e9 = global;
    let _e10 = outvarCOLOR;
    let _e11 = outvarTEXCOORD;
    return VertexOutput(_e9, _e10, _e11);
}
