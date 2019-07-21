shader_type canvas_item;

uniform float _hitShadePercent : hint_range(0.0, 1.0);

void fragment() {
	vec4 texColor = texture(TEXTURE, UV);
	
	vec3 diff = vec3(.2, .8, .8);
	COLOR.rgb = mix(texColor.rgb, vec3(.8, .2, .2) + diff * _hitShadePercent * _hitShadePercent, _hitShadePercent);
	COLOR.a = texColor.a;
}