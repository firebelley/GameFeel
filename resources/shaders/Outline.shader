shader_type canvas_item;

uniform vec4 _outlineColor : hint_color = vec4(1, 1, 1, 1);
uniform bool _enabled;

void fragment() {
	COLOR = texture(TEXTURE, UV);
	float alpha = COLOR.a;
	
	if (alpha == 0.0 && _enabled) {
		float adjacentAlpha = 0.0;
		adjacentAlpha += texture(TEXTURE, UV + vec2(TEXTURE_PIXEL_SIZE.x, 0)).a;
		adjacentAlpha += texture(TEXTURE, UV - vec2(TEXTURE_PIXEL_SIZE.x, 0)).a;
		adjacentAlpha += texture(TEXTURE, UV + vec2(0, TEXTURE_PIXEL_SIZE.y)).a;
		adjacentAlpha += texture(TEXTURE, UV - vec2(0, TEXTURE_PIXEL_SIZE.y)).a;
		if (adjacentAlpha > 0.0) {
			COLOR.rgba = _outlineColor;
		}
	}
}