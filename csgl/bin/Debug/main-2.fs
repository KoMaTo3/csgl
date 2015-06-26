#version 330

out vec4 out_frag_color;
vec4 debug = vec4( 1.0, 1.0, 1.0, 0.1 );

in vec4 color;

void main() {
  out_frag_color = color * debug;
}
