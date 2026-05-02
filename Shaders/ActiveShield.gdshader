shader_type canvas_item;

uniform bool active = true;
uniform float time_scale = 2.0;
uniform float edge_strength = 2.0;

void fragment()
{
	if(active) {
		vec2 uv = UV;

	    float dist = distance(uv, vec2(0.5));
	    float circle = 1.0 - smoothstep(0.45, 0.5, dist);

	    float edge = smoothstep(0.3, 0.5, dist);
	    edge = pow(edge, edge_strength);

	    float pulse = sin(TIME * time_scale + dist * 20.0) * 0.5 + 0.5;
		
	    vec3 shield_color = vec3(0.3, 0.75, 1.0);
		
		float noise = sin(UV.x * 40.0 + TIME * 3.0) *
					sin(UV.y * 40.0 + TIME * 2.0);

	    COLOR.rgb = shield_color * (edge + pulse * 0.2);
	    COLOR.a = circle * edge * sin(TIME * time_scale + dist * 25.0);
	}
}