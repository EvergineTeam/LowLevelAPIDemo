struct VertexOutput {
    @builtin(position) member: vec4<f32>,
    @location(0) member_1: vec2<f32>,
}

var<private> global: u32;
var<private> global_1: vec4<f32> = vec4<f32>(0f, 0f, 0f, 1f);
var<private> outvarTEXCOORD: vec2<f32>;

fn VS_1() {
    let _e12 = global;
    let _e19 = vec2<f32>(f32(((_e12 << bitcast<u32>(1u)) & 2u)), f32((_e12 & 2u)));
    let _e21 = ((_e19 * vec2<f32>(2f, -2f)) + vec2<f32>(-1f, 1f));
    global_1 = vec4<f32>(_e21.x, _e21.y, 0f, 1f);
    outvarTEXCOORD = _e19;
    return;
}

@vertex 
fn VS(@builtin(vertex_index) param: u32) -> VertexOutput {
    global = param;
    VS_1();
    let _e4 = global_1;
    let _e5 = outvarTEXCOORD;
    return VertexOutput(_e4, _e5);
}
