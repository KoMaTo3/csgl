#version 330

uniform sampler2D texture0;
uniform sampler2D texture1;
uniform float time;

out vec4 out_frag_color;
//vec4 debug = vec4( 1.0, 1.0, 1.0, 0.1 );
vec3 lightDirection = normalize( vec3( 1, -1, 1 ) );

in FragData
{
  vec4 color;
  vec2 texCoords;
  vec3 normal;
};

void main() {
  //float coeff = ( texCoords.x + texCoords.y > 1 ? 0 : 1 );
  //out_frag_color = ( texture2D( texture0, texCoords ) * coeff + texture2D( texture1, texCoords ) * ( 1 - coeff ) ) * color * debug;

  //float diffuse = clamp( dot( normal, lightDirection ), 0, 1 );
  float diffuse = clamp( max( dot( normal, lightDirection ), 0.0 ), 0.0, 1.0 );
  vec4 vDiffuse = vec4( diffuse, diffuse, diffuse, 1 );

  //out_frag_color = texture2D( texture1, texCoords );
  //out_frag_color = vec4( normal, 1 );
  vec4 colorWithLight = texture2D( texture1, texCoords );
  colorWithLight.r += ( color.r - colorWithLight.r ) * color.a;
  colorWithLight.g += ( color.g - colorWithLight.g ) * color.a;
  colorWithLight.b += ( color.b - colorWithLight.b ) * color.a;
  colorWithLight *= vDiffuse;

  //vec4 colorWithLight = vec4(texture2D( texture0, texCoords ).xyz,1);
  //colorWithLight = vec4(1,0,texture2D( texture0, texCoords ).r*0.0,1);
  out_frag_color = colorWithLight;
}
