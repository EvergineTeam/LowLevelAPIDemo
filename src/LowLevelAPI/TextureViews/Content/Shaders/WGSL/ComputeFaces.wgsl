struct typeParamsBuffer {
    time: f32,
    resolution: f32,
}

@group(0) @binding(0) 
var<uniform> ParamsBuffer: typeParamsBuffer;
@group(0) @binding(20) 
var Output: texture_storage_2d_array<rgba32float,read_write>;
var<private> global: vec3<u32>;

fn CS_1() {
    var local: vec3<f32>;
    var phi_214_: bool;
    var phi_220_: bool;
    var phi_228_: bool;
    var phi_383_: bool;
    var phi_391_: bool;
    var phi_396_: f32;
    var phi_400_: vec3<f32>;
    var phi_401_: f32;
    var phi_427_: vec4<f32>;
    var phi_531_: vec4<f32>;
    var phi_552_: vec4<f32>;
    var phi_592_: f32;
    var phi_596_: vec4<f32>;
    var phi_598_: vec2<f32>;
    var phi_601_: vec3<f32>;
    var phi_603_: f32;
    var local_1: vec3<f32>;
    var local_2: vec3<f32>;
    var local_3: vec3<f32>;
    var phi_638_: vec4<f32>;
    var phi_642_: f32;
    var phi_645_: f32;
    var phi_647_: i32;
    var local_4: f32;
    var local_5: f32;
    var local_6: f32;
    var phi_677_: vec4<f32>;
    var phi_678_: vec4<f32>;

    let _e172 = global;
    let _e176 = ParamsBuffer.resolution;
    let _e178 = (vec2<f32>(_e172.xy) * (1f / _e176));
    phi_214_ = true;
    if !((_e172.x < 2u)) {
        phi_214_ = (f32(_e172.x) >= (_e176 - 2f));
    }
    let _e186 = phi_214_;
    phi_220_ = true;
    if !(_e186) {
        phi_220_ = (_e172.y < 2u);
    }
    let _e191 = phi_220_;
    phi_228_ = true;
    if !(_e191) {
        phi_228_ = (f32(_e172.y) >= (_e176 - 2f));
    }
    let _e198 = phi_228_;
    if _e198 {
        phi_678_ = vec4<f32>(0.3f, 0.3f, 0.3f, 3f);
    } else {
        let _e200 = bitcast<i32>(_e172.z);
        if (_e200 == 0i) {
            let _e592 = ParamsBuffer.time;
            phi_642_ = _e592;
            phi_645_ = f32();
            phi_647_ = 0i;
            loop {
                let _e594 = phi_642_;
                let _e596 = phi_645_;
                let _e598 = phi_647_;
                local_4 = _e596;
                local_5 = _e596;
                local_6 = _e596;
                if (_e598 < 3i) {
                    continue;
                } else {
                    break;
                }
                continuing {
                    let _e600 = (_e178 - vec2<f32>(0.5f, 0.5f));
                    let _e601 = (_e594 + 0.07f);
                    let _e602 = length(_e600);
                    local[bitcast<u32>(_e598)] = (0.01f / length((fract((_e178 + (((_e600 / vec2(_e602)) * (sin(_e601) + 1f)) * abs(sin((((_e602 * 9f) - _e601) - _e601)))))) - vec2<f32>(0.5f, 0.5f))));
                    phi_642_ = _e601;
                    phi_645_ = _e602;
                    phi_647_ = (_e598 + 1i);
                }
            }
            let _e622 = local;
            let _e624 = local_4;
            let _e626 = local_5;
            let _e628 = local_6;
            let _e630 = (_e622 / vec3<f32>(_e624, _e626, _e628));
            phi_677_ = vec4<f32>(_e630.x, _e630.y, _e630.z, 1f);
        } else {
            if (_e200 == 1i) {
                phi_598_ = _e178;
                phi_601_ = vec3<f32>(0f, 0f, 0f);
                phi_603_ = 0f;
                loop {
                    let _e543 = phi_598_;
                    let _e545 = phi_601_;
                    let _e547 = phi_603_;
                    local_1 = _e545;
                    local_2 = _e545;
                    local_3 = _e545;
                    if (_e547 < 4f) {
                        continue;
                    } else {
                        break;
                    }
                    continuing {
                        let _e551 = (fract((_e543 * 1.5f)) - vec2<f32>(0.5f, 0.5f));
                        let _e553 = length(_e178);
                        let _e560 = ParamsBuffer.time;
                        phi_598_ = _e551;
                        phi_601_ = (_e545 + ((vec3<f32>(0.5f, 0.5f, 0.5f) + (vec3<f32>(0.5f, 0.5f, 0.5f) * cos((((vec3<f32>(1f, 1f, 1f) * ((_e553 + (_e547 * 0.4f)) + (_e560 * 0.4f))) + vec3<f32>(0.263f, 0.416f, 0.557f)) * 6.28318f)))) * pow((0.01f / abs((sin((((length(_e551) * exp(-(_e553))) * 8f) + _e560)) * 0.125f))), 1.2f)));
                        phi_603_ = (_e547 + 1f);
                    }
                }
                let _e580 = local_1;
                let _e583 = local_2;
                let _e586 = local_3;
                phi_638_ = vec4<f32>(_e580.x, _e583.y, _e586.z, 1f);
            } else {
                if (_e200 == 2i) {
                    let _e502 = ParamsBuffer.time;
                    let _e509 = (((_e178 - vec2<f32>(0.5f, 0.5f)) * 2f) * (1f + (0.2f * pow(abs(sin((2f * _e502))), 10f))));
                    let _e511 = (_e509.y + 0.5f);
                    switch bitcast<i32>(0u) {
                        default: {
                            let _e514 = abs(_e509.x);
                            let _e515 = vec2<f32>(_e514, _e511);
                            let _e516 = (_e511 + _e514);
                            if (_e516 > 1f) {
                                let _e518 = (_e515 - vec2<f32>(0.25f, 0.75f));
                                phi_592_ = (sqrt(dot(_e518, _e518)) - 0.35355338f);
                                break;
                            }
                            let _e522 = (_e515 - vec2<f32>(0f, 1f));
                            let _e526 = (_e515 - (vec2<f32>(_e516, 0f) * 0.5f));
                            phi_592_ = (sqrt(min(dot(_e522, _e522), dot(_e526, _e526))) * f32(i32(sign((_e514 - _e511)))));
                            break;
                        }
                    }
                    let _e536 = phi_592_;
                    phi_596_ = select(vec4<f32>(0f, 0f, 0f, 1f), vec4<f32>(1f, 0f, 0f, 1f), vec4((_e536 < 0f)));
                } else {
                    if (_e200 == 3i) {
                        let _e478 = ParamsBuffer.time;
                        let _e480 = (_e178 - vec2<f32>(0.5f, 0.5f));
                        let _e492 = mix(vec3<f32>(0f, 0.2f, 0f), vec3<f32>(0.3f, 0.6f, 0.3f), vec3(sin((((_e478 * 12f) + (sqrt(dot(_e480, _e480)) * 50f)) + (atan2(_e480.x, _e480.y) * 5f)))));
                        phi_552_ = vec4<f32>(_e492.x, _e492.y, _e492.z, 1f);
                    } else {
                        if (_e200 == 4i) {
                            let _e376 = floor((_e178 * vec2<f32>(320f, 200f)));
                            let _e381 = step(vec2<f32>(2f, 2f), (_e376 - (floor((_e376 * vec2<f32>(0.0625f, 0.0625f))) * 16f)));
                            let _e387 = mix(vec3<f32>(0.51f, 0.29f, 0.51f), mix(vec3<f32>(0.51f, 0.29f, 0.51f), vec3<f32>(0.66f, 0.66f, 0.66f), vec3(_e381.x)), vec3(_e381.y));
                            let _e388 = (_e376 * vec2<f32>(0.003125f, 0.005f));
                            let _e390 = ParamsBuffer.time;
                            let _e391 = (_e390 + 1f);
                            let _e395 = (_e391 - (6f * floor((_e391 * 0.16666667f))));
                            let _e397 = (step(_e395, 3f) - 0.5f);
                            let _e417 = vec2<f32>((_e388.x - ((((_e397 * -1.512f) * (_e395 - (3f * floor((_e395 * 0.33333334f))))) * 0.33333334f) + (_e397 * 0.756f))), (_e388.y - ((abs(sin((4.5f + (_e390 * 1.3f)))) * 0.5f) - 0.3f)));
                            let _e426 = ((_e417 * 2f) - vec2<f32>(1f, 1f));
                            let _e430 = normalize(vec3<f32>(_e426.x, _e426.y, 1.5f));
                            let _e431 = dot(_e430, vec3<f32>(0f, 0f, -4f));
                            let _e433 = ((_e431 * _e431) - 15.6f);
                            let _e440 = (normalize((vec3<f32>(0f, 0f, -4f) + (_e430 * (-(_e431) - sqrt(_e433))))) * mat3x3<f32>(vec3<f32>(0.9553f, -0.2955f, 0f), vec3<f32>(0.2955f, 0.9553f, 0f), vec3<f32>(0f, 0f, 1f)));
                            let _e456 = floor((vec2<f32>(((atan2(_e440.x, _e440.z) * 0.31830987f) + (floor(((_e390 * (_e397 * 2f)) * 60f)) * 0.008333334f)), (acos(_e440.y) * 0.31830987f)) * 8f));
                            let _e459 = (_e456.x + _e456.y);
                            let _e470 = mix(mix(_e387, (_e387 - vec3<f32>(0.2f, 0.2f, 0.2f)), vec3((1f - step(0.12f, length((_e417 - vec2<f32>(0.57f, 0.29f))))))), mix(vec3<f32>(1f, 0f, 0f), vec3<f32>(1f, 1f, 1f), vec3(clamp((_e459 - (2f * floor((_e459 * 0.5f)))), 0f, 1f))), vec3((1f - step(_e433, 0f))));
                            phi_531_ = vec4<f32>(_e470.x, _e470.y, _e470.z, 1f);
                        } else {
                            let _e208 = (((_e178 * 1.6f) + vec2<f32>(-0.2f, -0.7f)) * mat2x2<f32>(vec2<f32>(0.9250772f, -0.3797791f), vec2<f32>(0.3797791f, 0.9250772f)));
                            switch bitcast<i32>(0u) {
                                default: {
                                    if (_e208.x <= 0.54f) {
                                        let _e294 = (((1.276f * pow(_e208.x, 3f)) - ((1.4624f * _e208.x) * _e208.x)) + (1.4154f * _e208.x));
                                        let _e296 = floor((_e294 * 11.111111f));
                                        let _e297 = (_e296 * 0.09f);
                                        let _e300 = ParamsBuffer.time;
                                        let _e308 = (_e294 - _e297);
                                        let _e311 = ((sin((((_e297 + 0.016f) * 6.2831855f) + _e300)) * 0.076f) + (((cos((3.1415927f + _e300)) * 0.076f) * _e308) * 5.452f));
                                        let _e322 = ((_e208.y - _e311) * 10.144928f);
                                        let _e324 = floor(_e322);
                                        let _e326 = ((_e296 * 0.082f) + 0.452f);
                                        phi_383_ = false;
                                        if (_e324 > 0.9f) {
                                            phi_383_ = (_e324 < 2.1f);
                                        }
                                        let _e330 = phi_383_;
                                        if _e330 {
                                            phi_400_ = vec3<f32>(0.435f, 0.682f, 0.843f);
                                            phi_401_ = (_e326 * 0.8f);
                                        } else {
                                            phi_391_ = false;
                                            if (_e324 > 3.9f) {
                                                phi_391_ = (_e324 < 5.1f);
                                            }
                                            let _e334 = phi_391_;
                                            if _e334 {
                                                phi_396_ = (_e326 * 0.8f);
                                            } else {
                                                phi_396_ = _e326;
                                            }
                                            let _e337 = phi_396_;
                                            phi_400_ = select(vec3<f32>(0f, 0f, 0f), vec3<f32>(0.941f, 0.439f, 0.404f), vec3(_e334));
                                            phi_401_ = _e337;
                                        }
                                        let _e342 = phi_400_;
                                        let _e344 = phi_401_;
                                        let _e345 = (1f / _e344);
                                        let _e348 = ((_e344 - 1f) / (2f * _e344));
                                        let _e354 = (vec3<f32>((_e308 * 11.111111f), fract(_e322), 1f) * mat3x3<f32>(vec3<f32>(_e345, 0f, _e348), vec3<f32>(0f, _e345, _e348), vec3<f32>(0f, 0f, 1f))).xy;
                                        let _e355 = step(vec2<f32>(0f, 0f), _e354);
                                        let _e357 = step(vec2<f32>(0f, 0f), (vec2<f32>(1f, 1f) - _e354));
                                        phi_427_ = vec4<f32>(_e342.x, _e342.y, _e342.z, ((((_e355.x * _e355.y) * _e357.x) * _e357.y) * ((step(_e311, _e208.y) * (1f - step((0.69f + _e311), _e208.y))) * step(0f, _e294))));
                                        break;
                                    } else {
                                        let _e214 = ParamsBuffer.time;
                                        let _e220 = ((_e208 + vec2<f32>(0f, (sin(((_e208.x * 6.2831855f) + _e214)) * -0.076f))) + vec2<f32>(-0.54f, 0f));
                                        let _e221 = (_e220 * mat2x2<f32>(vec2<f32>(2.0833333f, 0f), vec2<f32>(0f, 1.4492754f)));
                                        let _e222 = step(vec2<f32>(0f, 0f), _e221);
                                        let _e224 = step(vec2<f32>(0f, 0f), (vec2<f32>(1f, 1f) - _e221));
                                        let _e232 = (_e220 * mat2x2<f32>(vec2<f32>(6.25f, 0f), vec2<f32>(0f, 4.5454545f)));
                                        let _e234 = step(vec2<f32>(0f, 0f), (_e232 + vec2<f32>(0f, -1.75f)));
                                        let _e236 = step(vec2<f32>(0f, 0f), (vec2<f32>(1f, 2.75f) - _e232));
                                        let _e245 = step(vec2<f32>(0f, 0f), (_e232 + vec2<f32>(0f, -0.375f)));
                                        let _e247 = step(vec2<f32>(0f, 0f), (vec2<f32>(1f, 1.375f) - _e232));
                                        let _e256 = step(vec2<f32>(0f, 0f), (_e232 + vec2<f32>(-1.5f, -1.75f)));
                                        let _e258 = step(vec2<f32>(0f, 0f), (vec2<f32>(2.5f, 2.75f) - _e232));
                                        let _e267 = step(vec2<f32>(0f, 0f), (_e232 + vec2<f32>(-1.5f, -0.375f)));
                                        let _e269 = step(vec2<f32>(0f, 0f), (vec2<f32>(2.5f, 1.375f) - _e232));
                                        let _e283 = ((((vec3<f32>(0.941f, 0.439f, 0.404f) * (((_e234.x * _e234.y) * _e236.x) * _e236.y)) + (vec3<f32>(0.435f, 0.682f, 0.843f) * (((_e245.x * _e245.y) * _e247.x) * _e247.y))) + (vec3<f32>(0.659f, 0.808f, 0.506f) * (((_e256.x * _e256.y) * _e258.x) * _e258.y))) + (vec3<f32>(0.996f, 0.859f, 0.114f) * (((_e267.x * _e267.y) * _e269.x) * _e269.y)));
                                        phi_427_ = vec4<f32>(_e283.x, _e283.y, _e283.z, (((_e222.x * _e222.y) * _e224.x) * _e224.y));
                                        break;
                                    }
                                }
                            }
                            let _e371 = phi_427_;
                            phi_531_ = mix(vec4<f32>(0f, 0.5f, 0.5f, 1f), _e371, vec4(_e371.w));
                        }
                        let _e476 = phi_531_;
                        phi_552_ = _e476;
                    }
                    let _e498 = phi_552_;
                    phi_596_ = _e498;
                }
                let _e541 = phi_596_;
                phi_638_ = _e541;
            }
            let _e590 = phi_638_;
            phi_677_ = _e590;
        }
        let _e636 = phi_677_;
        phi_678_ = _e636;
    }
    let _e638 = phi_678_;
    let _e640 = pow(_e638.xyz, vec3<f32>(0.45454547f, 0.45454547f, 0.45454547f));
    textureStore(Output, vec2<u32>(_e172.x, _e172.y), i32(_e172.z), vec4<f32>(_e640.x, _e640.y, _e640.z, _e638.w));
    return;
}

@compute @workgroup_size(8, 8, 1) 
fn CS(@builtin(global_invocation_id) param: vec3<u32>) {
    global = param;
    CS_1();
}
