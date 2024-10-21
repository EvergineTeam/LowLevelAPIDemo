@group(0) @binding(60) 
var DiffuseTexture: texture_2d_array<f32>;
@group(0) @binding(40) 
var Sampler: sampler;
var<private> global: vec4<f32>;
var<private> invarNORMAL_1: vec3<f32>;
var<private> invarTEXCOORD_1: vec3<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    let _e11 = invarNORMAL_1;
    let _e12 = invarTEXCOORD_1;
    let _e18 = textureSample(DiffuseTexture, Sampler, vec2<f32>(_e12.x, _e12.y), i32(_e12.z));
    if ((_e18.w - 0.5f) < 0f) {
        discard;
    }
    outvarSV_Target = (_e18 * (0.5f + (clamp(dot(_e11, vec3<f32>(0f, 0f, -1f)), 0f, 1f) * 0.5f)));
    return;
}

@fragment 
fn PS(@builtin(position) param: vec4<f32>, @location(0) invarNORMAL: vec3<f32>, @location(1) invarTEXCOORD: vec3<f32>) -> @location(0) vec4<f32> {
    global = param;
    invarNORMAL_1 = invarNORMAL;
    invarTEXCOORD_1 = invarTEXCOORD;
    PS_1();
    let _e7 = outvarSV_Target;
    return _e7;
}
