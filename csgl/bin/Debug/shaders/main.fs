#version 330

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform float time;

out vec4 out_frag_color;
vec4 debug = vec4( 1.0, 1.0, 1.0, 0.1 );

in FragData
{
  vec4 color;
  vec2 texCoords;
};

void main() {
  out_frag_color = texture2D( texture1, texCoords ) /* * debug * vec4( 1, 1, 1, sin( time * 10 ) * 0.5 + 0.5 ) */ ;
}
