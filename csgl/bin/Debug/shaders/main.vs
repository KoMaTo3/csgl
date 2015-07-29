#version 330

layout(location = 0) in vec4 vert;
layout(location = 1) in vec4 vColor;
layout(location = 2) in vec4 perVerticeInfo;
layout(location = 3) in vec4 texCoords;

const int matrixStride = 16;

uniform samplerBuffer objectsData;
uniform mat4 worldMatrix;
uniform mat4 projectionMatrix;

out FragData
{
  vec4 color;
  vec2 texCoords;
} fragOut;

void main() {
  int objectMatrixIndex = int( perVerticeInfo.x ) * 2 * matrixStride;
  mat4 modelMatrix = mat4(
    texelFetch( objectsData, objectMatrixIndex ).x,
    texelFetch( objectsData, objectMatrixIndex + 1 ).x,
    texelFetch( objectsData, objectMatrixIndex + 2 ).x,
    texelFetch( objectsData, objectMatrixIndex + 3 ).x,
    texelFetch( objectsData, objectMatrixIndex + 4 ).x,
    texelFetch( objectsData, objectMatrixIndex + 5 ).x,
    texelFetch( objectsData, objectMatrixIndex + 6 ).x,
    texelFetch( objectsData, objectMatrixIndex + 7 ).x,
    texelFetch( objectsData, objectMatrixIndex + 8 ).x,
    texelFetch( objectsData, objectMatrixIndex + 9 ).x,
    texelFetch( objectsData, objectMatrixIndex + 10 ).x,
    texelFetch( objectsData, objectMatrixIndex + 11 ).x,
    texelFetch( objectsData, objectMatrixIndex + 12 ).x,
    texelFetch( objectsData, objectMatrixIndex + 13 ).x,
    texelFetch( objectsData, objectMatrixIndex + 14 ).x,
    texelFetch( objectsData, objectMatrixIndex + 15 ).x
  );
  gl_Position = vert * modelMatrix * worldMatrix * projectionMatrix;
  fragOut.color = vColor;
  fragOut.texCoords = texCoords.xy;
}
