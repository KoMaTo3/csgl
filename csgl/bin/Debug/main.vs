#version 330

layout(location = 0) in vec4 vert;

uniform mat4 worldMatrix;

void main() {
  gl_Position = vert * worldMatrix;
}
