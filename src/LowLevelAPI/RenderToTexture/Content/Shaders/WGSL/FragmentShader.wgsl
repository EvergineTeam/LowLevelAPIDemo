struct typeMatrices {
    IsTextured: u32,
    worldViewProj: mat4x4<f32>,
}

@group(0) @binding(0) 
var<uniform> Matrices: typeMatrices;
@group(0) @binding(60) 
var DiffuseTexture: texture_2d<f32>;
@group(0) @binding(40) 
var Sampler: sampler;
var<private> global: vec4<f32>;
var<private> invarCOLOR_1: vec4<f32>;
var<private> invarTEXCOORD_1: vec2<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    var phi_46_: vec4<f32>;

    let _e9 = invarCOLOR_1;
    let _e10 = invarTEXCOORD_1;
    switch bitcast<i32>(0u) {
        default: {
            let _e13 = Matrices.IsTextured;
            if (_e13 != 0u) {
                let _e15 = textureSample(DiffuseTexture, Sampler, _e10);
                phi_46_ = _e15;
                break;
            } else {
                phi_46_ = _e9;
                break;
            }
        }
    }
    let _e17 = phi_46_;
    outvarSV_Target = _e17;
    return;
}

@fragment 
fn PS(@builtin(position) param: vec4<f32>, @location(0) invarCOLOR: vec4<f32>, @location(1) invarTEXCOORD: vec2<f32>) -> @location(0) vec4<f32> {
    global = param;
    invarCOLOR_1 = invarCOLOR;
    invarTEXCOORD_1 = invarTEXCOORD;
    PS_1();
    let _e7 = outvarSV_Target;
    return _e7;
}
