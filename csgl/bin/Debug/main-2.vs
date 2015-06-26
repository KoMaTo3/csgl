#version 330

layout(location = 0) in vec4 vert;
layout(location = 1) in vec4 vColor;
layout(location = 2) in vec4 perVerticeInfo;

const int matrixStride = 16;

uniform samplerBuffer objectsData;
uniform mat4 worldMatrix;

out vec4 color;

void main() {
  //color = vColor + vec4( texelFetch( objectsData, 0 ).x, 0, 0, 0 );
  //color = vColor + vec4( v, 0, 0, 0 );
  int matrixIndex = int( perVerticeInfo.x );
  mat4 modelMatrix = mat4(
    texelFetch( objectsData, matrixIndex * matrixStride ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 1 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 2 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 3 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 4 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 5 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 6 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 7 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 8 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 9 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 10 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 11 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 12 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 13 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 14 ).x,
    texelFetch( objectsData, matrixIndex * matrixStride + 15 ).x
  );
  gl_Position = vert * modelMatrix * worldMatrix;
  //color = vec4( 1, 0, 0, 1 );
  color = vColor;
}
