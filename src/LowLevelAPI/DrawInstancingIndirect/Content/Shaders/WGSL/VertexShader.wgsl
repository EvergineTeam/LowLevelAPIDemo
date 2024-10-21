struct typeMatrices {
    viewProj: mat4x4<f32>,
}

struct VertexOutput {
    @builtin(position) member: vec4<f32>,
    @location(0) member_1: vec3<f32>,
    @location(1) member_2: vec3<f32>,
}

@group(0) @binding(0) 
var<uniform> Matrices: typeMatrices;
var<private> invarPOSITION_1: vec3<f32>;
var<private> invarNORMAL_1: vec3<f32>;
var<private> invarTEXCOORD0_1: vec2<f32>;
var<private> invarTEXCOORD1_1: vec4<f32>;
var<private> invarTEXCOORD2_1: vec4<f32>;
var<private> invarTEXCOORD3_1: vec4<f32>;
var<private> invarTEXCOORD4_1: vec4<f32>;
var<private> invarTEXCOORD5_1: i32;
var<private> global: vec4<f32> = vec4<f32>(0f, 0f, 0f, 1f);
var<private> outvarNORMAL: vec3<f32>;
var<private> outvarTEXCOORD: vec3<f32>;

fn VS_1() {
    let _e16 = invarPOSITION_1;
    let _e17 = invarNORMAL_1;
    let _e18 = invarTEXCOORD0_1;
    let _e19 = invarTEXCOORD1_1;
    let _e20 = invarTEXCOORD2_1;
    let _e21 = invarTEXCOORD3_1;
    let _e22 = invarTEXCOORD4_1;
    let _e23 = invarTEXCOORD5_1;
    let _e26 = Matrices.viewProj;
    let _e27 = (_e26 * mat4x4<f32>(_e19, _e20, _e21, _e22));
    let _e42 = vec3<f32>(_e18.x, _e18.y, vec3<f32>().z);
    global = (_e27 * vec4<f32>(_e16.x, _e16.y, _e16.z, 1f));
    outvarNORMAL = (_e27 * vec4<f32>(_e17.x, _e17.y, _e17.z, 0f)).xyz;
    outvarTEXCOORD = vec3<f32>(_e42.x, _e42.y, f32(_e23));
    return;
}

@vertex 
fn VS(@location(0) invarPOSITION: vec3<f32>, @location(1) invarNORMAL: vec3<f32>, @location(2) invarTEXCOORD0_: vec2<f32>, @location(3) invarTEXCOORD1_: vec4<f32>, @location(4) invarTEXCOORD2_: vec4<f32>, @location(5) invarTEXCOORD3_: vec4<f32>, @location(6) invarTEXCOORD4_: vec4<f32>, @location(7) invarTEXCOORD5_: i32) -> VertexOutput {
    invarPOSITION_1 = invarPOSITION;
    invarNORMAL_1 = invarNORMAL;
    invarTEXCOORD0_1 = invarTEXCOORD0_;
    invarTEXCOORD1_1 = invarTEXCOORD1_;
    invarTEXCOORD2_1 = invarTEXCOORD2_;
    invarTEXCOORD3_1 = invarTEXCOORD3_;
    invarTEXCOORD4_1 = invarTEXCOORD4_;
    invarTEXCOORD5_1 = invarTEXCOORD5_;
    VS_1();
    let _e19 = global;
    let _e20 = outvarNORMAL;
    let _e21 = outvarTEXCOORD;
    return VertexOutput(_e19, _e20, _e21);
}
