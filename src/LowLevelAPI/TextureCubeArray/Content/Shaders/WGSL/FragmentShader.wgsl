@group(0) @binding(60) 
var CubeTexture: texture_cube_array<f32>;
@group(0) @binding(40) 
var Sampler: sampler;
var<private> global: vec4<f32>;
var<private> invarTEXCOORD0_1: vec4<f32>;
var<private> invarTEXCOORD1_1: vec3<f32>;
var<private> invarTEXCOORD2_1: vec3<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    let _e12 = invarTEXCOORD0_1;
    let _e13 = invarTEXCOORD1_1;
    let _e14 = invarTEXCOORD2_1;
    let _e17 = reflect(normalize(_e13), normalize(_e14));
    let _e33 = vec4<f32>(_e17.x, _e17.y, _e17.z, f32(i32((((vec2<f32>((_e12.xy / vec2(_e12.w)).x, f32()) + vec2<f32>(1f, 1f)) * 0.5f).x * 4f))));
    let _e40 = textureSample(CubeTexture, Sampler, vec3<f32>(_e33.x, _e33.y, _e33.z), i32(_e33.w));
    outvarSV_Target = vec4<f32>(_e40.x, _e40.y, _e40.z, 1f);
    return;
}

@fragment 
fn PS(@builtin(position) param: vec4<f32>, @location(0) invarTEXCOORD0_: vec4<f32>, @location(1) invarTEXCOORD1_: vec3<f32>, @location(2) invarTEXCOORD2_: vec3<f32>) -> @location(0) vec4<f32> {
    global = param;
    invarTEXCOORD0_1 = invarTEXCOORD0_;
    invarTEXCOORD1_1 = invarTEXCOORD1_;
    invarTEXCOORD2_1 = invarTEXCOORD2_;
    PS_1();
    let _e9 = outvarSV_Target;
    return _e9;
}
