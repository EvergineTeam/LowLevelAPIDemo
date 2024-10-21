@group(0) @binding(60) 
var DiffuseTexture: texture_2d_array<f32>;
@group(0) @binding(40) 
var Sampler: sampler;
var<private> global: vec4<f32>;
var<private> invarTEXCOORD_1: vec2<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    let _e6 = invarTEXCOORD_1;
    let _e12 = vec3<f32>(_e6.x, _e6.y, f32(i32((_e6.x * 4f))));
    let _e18 = textureSample(DiffuseTexture, Sampler, vec2<f32>(_e12.x, _e12.y), i32(_e12.z));
    outvarSV_Target = _e18;
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
