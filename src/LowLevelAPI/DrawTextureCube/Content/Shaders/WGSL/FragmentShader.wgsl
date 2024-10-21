@group(0) @binding(60) 
var DiffuseTexture: texture_2d<f32>;
@group(0) @binding(40) 
var Sampler: sampler;
var<private> global: vec4<f32>;
var<private> invarCOLOR_1: vec4<f32>;
var<private> invarTEXCOORD_1: vec2<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    let _e6 = invarTEXCOORD_1;
    let _e7 = textureSample(DiffuseTexture, Sampler, _e6);
    outvarSV_Target = _e7;
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
