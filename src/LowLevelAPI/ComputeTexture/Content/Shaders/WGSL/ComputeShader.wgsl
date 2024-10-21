struct ComputeData {
    time: f32,
    width: f32,
    height: f32,
    padding: f32,
}

struct typeParamsBuffer {
    data: ComputeData,
}

@group(0) @binding(0) 
var<uniform> ParamsBuffer: typeParamsBuffer;
@group(0) @binding(20) 
var Output: texture_storage_2d<rgba8unorm,read_write>;
var<private> global: vec3<u32>;

fn CS_1() {
    let _e9 = global;
    let _e12 = ParamsBuffer.data.width;
    let _e15 = ParamsBuffer.data.height;
    let _e16 = vec2<f32>(_e12, _e15);
    let _e17 = _e9.xy;
    let _e21 = ((vec2<f32>(_e17) / _e16) + (vec2<f32>(0.5f, 0.5f) / _e16));
    let _e24 = ParamsBuffer.data.time;
    textureStore(Output, _e17, vec4<f32>(_e21.x, _e21.y, (0.5f + (0.5f * sin(_e24))), 1f));
    return;
}

@compute @workgroup_size(8, 8, 1) 
fn CS(@builtin(global_invocation_id) param: vec3<u32>) {
    global = param;
    CS_1();
}
