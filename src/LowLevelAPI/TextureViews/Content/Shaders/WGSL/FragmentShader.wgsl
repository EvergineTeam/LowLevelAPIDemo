@group(0) @binding(60) 
var CubeTexture: texture_cube<f32>;
@group(0) @binding(40) 
var Sampler: sampler;
var<private> invarTEXCOORD0_1: vec3<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    let _e4 = invarTEXCOORD0_1;
    let _e5 = textureSample(CubeTexture, Sampler, _e4);
    outvarSV_Target = _e5;
    return;
}

@fragment 
fn PS(@location(0) invarTEXCOORD0_: vec3<f32>) -> @location(0) vec4<f32> {
    invarTEXCOORD0_1 = invarTEXCOORD0_;
    PS_1();
    let _e3 = outvarSV_Target;
    return _e3;
}
