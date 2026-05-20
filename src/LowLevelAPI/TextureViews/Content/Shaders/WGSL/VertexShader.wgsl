struct typeMatrices {
    worldViewProj: mat4x4<f32>,
}

struct VertexOutput {
    @builtin(position) member: vec4<f32>,
    @location(0) member_1: vec3<f32>,
}

@group(0) @binding(0) 
var<uniform> Matrices: typeMatrices;
var<private> invarPOSITION_1: vec3<f32>;
var<private> global: vec4<f32> = vec4<f32>(0f, 0f, 0f, 1f);
var<private> outvarTEXCOORD0_: vec3<f32>;

fn VS_1() {
    let _e6 = invarPOSITION_1;
    let _e12 = Matrices.worldViewProj;
    global = (_e12 * vec4<f32>(_e6.x, _e6.y, _e6.z, 1f));
    outvarTEXCOORD0_ = _e6;
    return;
}

@vertex 
fn VS(@location(0) invarPOSITION: vec3<f32>) -> VertexOutput {
    invarPOSITION_1 = invarPOSITION;
    VS_1();
    let _e4 = global;
    let _e5 = outvarTEXCOORD0_;
    return VertexOutput(_e4, _e5);
}
