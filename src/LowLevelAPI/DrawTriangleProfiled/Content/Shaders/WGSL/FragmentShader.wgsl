var<private> global: vec4<f32>;
var<private> invarCOLOR_1: vec4<f32>;
var<private> outvarSV_Target: vec4<f32>;

fn PS_1() {
    let _e3 = invarCOLOR_1;
    outvarSV_Target = _e3;
    return;
}

@fragment 
fn PS(@builtin(position) param: vec4<f32>, @location(0) invarCOLOR: vec4<f32>) -> @location(0) vec4<f32> {
    global = param;
    invarCOLOR_1 = invarCOLOR;
    PS_1();
    let _e5 = outvarSV_Target;
    return _e5;
}
