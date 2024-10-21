struct typeMatrices {
    worldViewProj: mat4x4<f32>,
    World: mat4x4<f32>,
    WorldInverseTranspose: mat4x4<f32>,
    CameraPosition: vec3<f32>,
}

struct VertexOutput {
    @builtin(position) member: vec4<f32>,
    @location(0) member_1: vec4<f32>,
    @location(1) member_2: vec3<f32>,
    @location(2) member_3: vec3<f32>,
}

@group(0) @binding(0) 
var<uniform> Matrices: typeMatrices;
var<private> invarPOSITION_1: vec4<f32>;
var<private> invarNORMAL_1: vec3<f32>;
var<private> invarTEXCOORD_1: vec2<f32>;
var<private> global: vec4<f32> = vec4<f32>(0f, 0f, 0f, 1f);
var<private> outvarTEXCOORD0_: vec4<f32>;
var<private> outvarTEXCOORD1_: vec3<f32>;
var<private> outvarTEXCOORD2_: vec3<f32>;

fn VS_1() {
    let _e12 = invarPOSITION_1;
    let _e13 = invarNORMAL_1;
    let _e15 = Matrices.worldViewProj;
    let _e16 = (_e15 * _e12);
    let _e18 = Matrices.World;
    let _e22 = Matrices.CameraPosition;
    let _e25 = Matrices.WorldInverseTranspose;
    global = _e16;
    outvarTEXCOORD0_ = _e16;
    outvarTEXCOORD1_ = ((_e18 * _e12).xyz - _e22);
    outvarTEXCOORD2_ = (mat3x3<f32>(_e25[0].xyz, _e25[1].xyz, _e25[2].xyz) * _e13);
    return;
}

@vertex 
fn VS(@location(0) invarPOSITION: vec4<f32>, @location(1) invarNORMAL: vec3<f32>, @location(2) invarTEXCOORD: vec2<f32>) -> VertexOutput {
    invarPOSITION_1 = invarPOSITION;
    invarNORMAL_1 = invarNORMAL;
    invarTEXCOORD_1 = invarTEXCOORD;
    VS_1();
    let _e10 = global;
    let _e11 = outvarTEXCOORD0_;
    let _e12 = outvarTEXCOORD1_;
    let _e13 = outvarTEXCOORD2_;
    return VertexOutput(_e10, _e11, _e12, _e13);
}
