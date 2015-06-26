#version 330

out vec4 out_frag_color;
vec4 debug = vec4( 1.0, 1.0, 1.0, 0.1 );

void main() {
  out_frag_color = vec4( 0.0, 0.0, 1.0, 1.0 ) * debug;
}
