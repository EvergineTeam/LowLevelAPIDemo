@group(0) @binding(60) 
var DiffuseTexture: texture_2d<f32>;
@group(0) @binding(40) 
var Sampler: sampler;
var<private> global: vec4<f32>;
var<private> invarTEXCOORD_1: vec2<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    let _e5 = invarTEXCOORD_1;
    let _e6 = textureSample(DiffuseTexture, Sampler, _e5);
    outvarSV_Target = _e6;
    return;
}

@fragment 
fn PS(@builtin(position) param: vec4<f32>, @location(0) invarTEXCOORD: vec2<f32>) -> @location(0) vec4<f32> {
    global = param;
    invarTEXCOORD_1 = invarTEXCOORD;
    PS_1();
    let _e5 = outvarSV_Target;
    return _e5;
}
