// Compiled shader for PC, Mac & Linux Standalone, uncompressed size: 20.1KB

// Skipping shader variants that would not be included into build of current scene.

Shader "Sprites/Default" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
 _Color ("Tint", Color) = (1.000000,1.000000,1.000000,1.000000)
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0.000000
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }


 // Stats for Vertex shader:
 //       metal : 6 avg math (3..10)
 //      opengl : 2 math, 1 avg texture (1..2)
 // Stats for Fragment shader:
 //       metal : 2 math, 1 avg texture (1..2)
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha
  GpuProgramID 35710
Program "vp" {
SubProgram "opengl " {
// Stats: 2 math, 1 textures
"#version 120

#ifdef VERTEX

uniform vec4 _Color;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = gl_Vertex.xyz;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_1);
  xlv_COLOR = (gl_Color * _Color);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 c_1;
  vec4 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 3 math
Bind "vertex" ATTR0
Bind "color" ATTR1
Bind "texcoord" ATTR2
ConstBuffer "$Globals" 72
Matrix 0 [glstate_matrix_mvp]
VectorHalf 64 [_Color] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float4 _glesColor [[attribute(1)]];
  float4 _glesMultiTexCoord0 [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  half4 _Color;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 tmpvar_1;
  float4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = _mtl_i._glesVertex.xyz;
  tmpvar_1 = half4((_mtl_i._glesColor * (float4)_mtl_u._Color));
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * tmpvar_2);
  _mtl_o.xlv_COLOR = tmpvar_1;
  _mtl_o.xlv_TEXCOORD0 = _mtl_i._glesMultiTexCoord0.xy;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform 	vec4 hlslcc_mtx4glstate_matrix_mvp[4];
uniform 	vec4 _Color;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
in  vec2 in_TEXCOORD0;
out vec4 vs_COLOR0;
out vec2 vs_TEXCOORD0;
vec4 u_xlat0;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4glstate_matrix_mvp[1];
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat0;
    gl_Position = u_xlat0 + hlslcc_mtx4glstate_matrix_mvp[3];
    vs_COLOR0 = in_COLOR0 * _Color;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
    return;
}
#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform  sampler2D _MainTex;
in  vec4 vs_COLOR0;
in  vec2 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
lowp vec4 u_xlat10_0;
void main()
{
    u_xlat10_0 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat0 = u_xlat10_0 * vs_COLOR0;
    SV_Target0.xyz = u_xlat0.www * u_xlat0.xyz;
    SV_Target0.w = u_xlat0.w;
    return;
}
#endif
"
}
SubProgram "opengl " {
// Stats: 2 math, 2 textures
Keywords { "ETC1_EXTERNAL_ALPHA" }
"#version 120

#ifdef VERTEX

uniform vec4 _Color;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  tmpvar_1.w = 1.0;
  tmpvar_1.xyz = gl_Vertex.xyz;
  gl_Position = (gl_ModelViewProjectionMatrix * tmpvar_1);
  xlv_COLOR = (gl_Color * _Color);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform sampler2D _AlphaTex;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 c_1;
  vec4 color_2;
  color_2.xyz = texture2D (_MainTex, xlv_TEXCOORD0).xyz;
  color_2.w = texture2D (_AlphaTex, xlv_TEXCOORD0).x;
  vec4 tmpvar_3;
  tmpvar_3 = (color_2 * xlv_COLOR);
  c_1.w = tmpvar_3.w;
  c_1.xyz = (tmpvar_3.xyz * tmpvar_3.w);
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 3 math
Keywords { "ETC1_EXTERNAL_ALPHA" }
Bind "vertex" ATTR0
Bind "color" ATTR1
Bind "texcoord" ATTR2
ConstBuffer "$Globals" 72
Matrix 0 [glstate_matrix_mvp]
VectorHalf 64 [_Color] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float4 _glesColor [[attribute(1)]];
  float4 _glesMultiTexCoord0 [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderUniform {
  float4x4 glstate_matrix_mvp;
  half4 _Color;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 tmpvar_1;
  float4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = _mtl_i._glesVertex.xyz;
  tmpvar_1 = half4((_mtl_i._glesColor * (float4)_mtl_u._Color));
  _mtl_o.gl_Position = (_mtl_u.glstate_matrix_mvp * tmpvar_2);
  _mtl_o.xlv_COLOR = tmpvar_1;
  _mtl_o.xlv_TEXCOORD0 = _mtl_i._glesMultiTexCoord0.xy;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
Keywords { "ETC1_EXTERNAL_ALPHA" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform 	vec4 hlslcc_mtx4glstate_matrix_mvp[4];
uniform 	vec4 _Color;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
in  vec2 in_TEXCOORD0;
out vec4 vs_COLOR0;
out vec2 vs_TEXCOORD0;
vec4 u_xlat0;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4glstate_matrix_mvp[1];
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat0;
    gl_Position = u_xlat0 + hlslcc_mtx4glstate_matrix_mvp[3];
    vs_COLOR0 = in_COLOR0 * _Color;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
    return;
}
#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform  sampler2D _MainTex;
uniform  sampler2D _AlphaTex;
in  vec4 vs_COLOR0;
in  vec2 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
lowp vec4 u_xlat10_1;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat10_1 = texture(_AlphaTex, vs_TEXCOORD0.xy);
    u_xlat0.w = u_xlat10_1.x;
    u_xlat0 = u_xlat0 * vs_COLOR0;
    SV_Target0.xyz = u_xlat0.www * u_xlat0.xyz;
    SV_Target0.w = u_xlat0.w;
    return;
}
#endif
"
}
SubProgram "opengl " {
// Stats: 2 math, 1 textures
Keywords { "PIXELSNAP_ON" }
"#version 120

#ifdef VERTEX
uniform vec4 _ScreenParams;

uniform vec4 _Color;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = gl_Vertex.xyz;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * tmpvar_2);
  vec4 pos_3;
  pos_3.zw = tmpvar_1.zw;
  vec2 tmpvar_4;
  tmpvar_4 = (_ScreenParams.xy * 0.5);
  pos_3.xy = ((floor(
    (((tmpvar_1.xy / tmpvar_1.w) * tmpvar_4) + vec2(0.5, 0.5))
  ) / tmpvar_4) * tmpvar_1.w);
  gl_Position = pos_3;
  xlv_COLOR = (gl_Color * _Color);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 c_1;
  vec4 tmpvar_2;
  tmpvar_2 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 10 math
Keywords { "PIXELSNAP_ON" }
Bind "vertex" ATTR0
Bind "color" ATTR1
Bind "texcoord" ATTR2
ConstBuffer "$Globals" 88
Matrix 16 [glstate_matrix_mvp]
Vector 0 [_ScreenParams]
VectorHalf 80 [_Color] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float4 _glesColor [[attribute(1)]];
  float4 _glesMultiTexCoord0 [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderUniform {
  float4 _ScreenParams;
  float4x4 glstate_matrix_mvp;
  half4 _Color;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 tmpvar_1;
  float4 tmpvar_2;
  float4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = _mtl_i._glesVertex.xyz;
  tmpvar_2 = (_mtl_u.glstate_matrix_mvp * tmpvar_3);
  tmpvar_1 = half4((_mtl_i._glesColor * (float4)_mtl_u._Color));
  float4 pos_4;
  pos_4.zw = tmpvar_2.zw;
  float2 tmpvar_5;
  tmpvar_5 = (_mtl_u._ScreenParams.xy * 0.5);
  pos_4.xy = ((floor(
    (((tmpvar_2.xy / tmpvar_2.w) * tmpvar_5) + float2(0.5, 0.5))
  ) / tmpvar_5) * tmpvar_2.w);
  _mtl_o.gl_Position = pos_4;
  _mtl_o.xlv_COLOR = tmpvar_1;
  _mtl_o.xlv_TEXCOORD0 = _mtl_i._glesMultiTexCoord0.xy;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
Keywords { "PIXELSNAP_ON" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform 	vec4 _ScreenParams;
uniform 	vec4 hlslcc_mtx4glstate_matrix_mvp[4];
uniform 	vec4 _Color;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
in  vec2 in_TEXCOORD0;
out vec4 vs_COLOR0;
out vec2 vs_TEXCOORD0;
vec4 u_xlat0;
vec2 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4glstate_matrix_mvp[1];
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4glstate_matrix_mvp[3];
    u_xlat0.xy = u_xlat0.xy / u_xlat0.ww;
    u_xlat1.xy = _ScreenParams.xy * vec2(0.5, 0.5);
    u_xlat0.xy = u_xlat0.xy * u_xlat1.xy;
    u_xlat0.xy = roundEven(u_xlat0.xy);
    u_xlat0.xy = u_xlat0.xy / u_xlat1.xy;
    gl_Position.xy = u_xlat0.ww * u_xlat0.xy;
    gl_Position.zw = u_xlat0.zw;
    vs_COLOR0 = in_COLOR0 * _Color;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
    return;
}
#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform  sampler2D _MainTex;
in  vec4 vs_COLOR0;
in  vec2 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
lowp vec4 u_xlat10_0;
void main()
{
    u_xlat10_0 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat0 = u_xlat10_0 * vs_COLOR0;
    SV_Target0.xyz = u_xlat0.www * u_xlat0.xyz;
    SV_Target0.w = u_xlat0.w;
    return;
}
#endif
"
}
SubProgram "opengl " {
// Stats: 2 math, 2 textures
Keywords { "ETC1_EXTERNAL_ALPHA" "PIXELSNAP_ON" }
"#version 120

#ifdef VERTEX
uniform vec4 _ScreenParams;

uniform vec4 _Color;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 tmpvar_1;
  vec4 tmpvar_2;
  tmpvar_2.w = 1.0;
  tmpvar_2.xyz = gl_Vertex.xyz;
  tmpvar_1 = (gl_ModelViewProjectionMatrix * tmpvar_2);
  vec4 pos_3;
  pos_3.zw = tmpvar_1.zw;
  vec2 tmpvar_4;
  tmpvar_4 = (_ScreenParams.xy * 0.5);
  pos_3.xy = ((floor(
    (((tmpvar_1.xy / tmpvar_1.w) * tmpvar_4) + vec2(0.5, 0.5))
  ) / tmpvar_4) * tmpvar_1.w);
  gl_Position = pos_3;
  xlv_COLOR = (gl_Color * _Color);
  xlv_TEXCOORD0 = gl_MultiTexCoord0.xy;
}


#endif
#ifdef FRAGMENT
uniform sampler2D _MainTex;
uniform sampler2D _AlphaTex;
varying vec4 xlv_COLOR;
varying vec2 xlv_TEXCOORD0;
void main ()
{
  vec4 c_1;
  vec4 color_2;
  color_2.xyz = texture2D (_MainTex, xlv_TEXCOORD0).xyz;
  color_2.w = texture2D (_AlphaTex, xlv_TEXCOORD0).x;
  vec4 tmpvar_3;
  tmpvar_3 = (color_2 * xlv_COLOR);
  c_1.w = tmpvar_3.w;
  c_1.xyz = (tmpvar_3.xyz * tmpvar_3.w);
  gl_FragData[0] = c_1;
}


#endif
"
}
SubProgram "metal " {
// Stats: 10 math
Keywords { "ETC1_EXTERNAL_ALPHA" "PIXELSNAP_ON" }
Bind "vertex" ATTR0
Bind "color" ATTR1
Bind "texcoord" ATTR2
ConstBuffer "$Globals" 88
Matrix 16 [glstate_matrix_mvp]
Vector 0 [_ScreenParams]
VectorHalf 80 [_Color] 4
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  float4 _glesVertex [[attribute(0)]];
  float4 _glesColor [[attribute(1)]];
  float4 _glesMultiTexCoord0 [[attribute(2)]];
};
struct xlatMtlShaderOutput {
  float4 gl_Position [[position]];
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderUniform {
  float4 _ScreenParams;
  float4x4 glstate_matrix_mvp;
  half4 _Color;
};
vertex xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 tmpvar_1;
  float4 tmpvar_2;
  float4 tmpvar_3;
  tmpvar_3.w = 1.0;
  tmpvar_3.xyz = _mtl_i._glesVertex.xyz;
  tmpvar_2 = (_mtl_u.glstate_matrix_mvp * tmpvar_3);
  tmpvar_1 = half4((_mtl_i._glesColor * (float4)_mtl_u._Color));
  float4 pos_4;
  pos_4.zw = tmpvar_2.zw;
  float2 tmpvar_5;
  tmpvar_5 = (_mtl_u._ScreenParams.xy * 0.5);
  pos_4.xy = ((floor(
    (((tmpvar_2.xy / tmpvar_2.w) * tmpvar_5) + float2(0.5, 0.5))
  ) / tmpvar_5) * tmpvar_2.w);
  _mtl_o.gl_Position = pos_4;
  _mtl_o.xlv_COLOR = tmpvar_1;
  _mtl_o.xlv_TEXCOORD0 = _mtl_i._glesMultiTexCoord0.xy;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
Keywords { "ETC1_EXTERNAL_ALPHA" "PIXELSNAP_ON" }
"#ifdef VERTEX
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform 	vec4 _ScreenParams;
uniform 	vec4 hlslcc_mtx4glstate_matrix_mvp[4];
uniform 	vec4 _Color;
in  vec4 in_POSITION0;
in  vec4 in_COLOR0;
in  vec2 in_TEXCOORD0;
out vec4 vs_COLOR0;
out vec2 vs_TEXCOORD0;
vec4 u_xlat0;
vec2 u_xlat1;
void main()
{
    u_xlat0 = in_POSITION0.yyyy * hlslcc_mtx4glstate_matrix_mvp[1];
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[0] * in_POSITION0.xxxx + u_xlat0;
    u_xlat0 = hlslcc_mtx4glstate_matrix_mvp[2] * in_POSITION0.zzzz + u_xlat0;
    u_xlat0 = u_xlat0 + hlslcc_mtx4glstate_matrix_mvp[3];
    u_xlat0.xy = u_xlat0.xy / u_xlat0.ww;
    u_xlat1.xy = _ScreenParams.xy * vec2(0.5, 0.5);
    u_xlat0.xy = u_xlat0.xy * u_xlat1.xy;
    u_xlat0.xy = roundEven(u_xlat0.xy);
    u_xlat0.xy = u_xlat0.xy / u_xlat1.xy;
    gl_Position.xy = u_xlat0.ww * u_xlat0.xy;
    gl_Position.zw = u_xlat0.zw;
    vs_COLOR0 = in_COLOR0 * _Color;
    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
    return;
}
#endif
#ifdef FRAGMENT
#version 150
#extension GL_ARB_explicit_attrib_location : require
#extension GL_ARB_shader_bit_encoding : enable
uniform  sampler2D _MainTex;
uniform  sampler2D _AlphaTex;
in  vec4 vs_COLOR0;
in  vec2 vs_TEXCOORD0;
layout(location = 0) out vec4 SV_Target0;
vec4 u_xlat0;
lowp vec4 u_xlat10_1;
void main()
{
    u_xlat0 = texture(_MainTex, vs_TEXCOORD0.xy);
    u_xlat10_1 = texture(_AlphaTex, vs_TEXCOORD0.xy);
    u_xlat0.w = u_xlat10_1.x;
    u_xlat0 = u_xlat0 * vs_COLOR0;
    SV_Target0.xyz = u_xlat0.www * u_xlat0.xyz;
    SV_Target0.w = u_xlat0.w;
    return;
}
#endif
"
}
}
Program "fp" {
SubProgram "opengl " {
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math, 1 textures
SetTexture 0 [_MainTex] 2D 0
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]]
  ,   texture2d<half> _MainTex [[texture(0)]], sampler _mtlsmp__MainTex [[sampler(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  half4 tmpvar_2;
  tmpvar_2 = (_MainTex.sample(_mtlsmp__MainTex, (float2)(_mtl_i.xlv_TEXCOORD0)) * _mtl_i.xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
"// shader disassembly not supported on glcore"
}
SubProgram "opengl " {
Keywords { "ETC1_EXTERNAL_ALPHA" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math, 2 textures
Keywords { "ETC1_EXTERNAL_ALPHA" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_AlphaTex] 2D 1
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]]
  ,   texture2d<half> _MainTex [[texture(0)]], sampler _mtlsmp__MainTex [[sampler(0)]]
  ,   texture2d<half> _AlphaTex [[texture(1)]], sampler _mtlsmp__AlphaTex [[sampler(1)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  half4 color_2;
  color_2.xyz = _MainTex.sample(_mtlsmp__MainTex, (float2)(_mtl_i.xlv_TEXCOORD0)).xyz;
  color_2.w = _AlphaTex.sample(_mtlsmp__AlphaTex, (float2)(_mtl_i.xlv_TEXCOORD0)).x;
  half4 tmpvar_3;
  tmpvar_3 = (color_2 * _mtl_i.xlv_COLOR);
  c_1.w = tmpvar_3.w;
  c_1.xyz = (tmpvar_3.xyz * tmpvar_3.w);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
Keywords { "ETC1_EXTERNAL_ALPHA" }
"// shader disassembly not supported on glcore"
}
SubProgram "opengl " {
Keywords { "PIXELSNAP_ON" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math, 1 textures
Keywords { "PIXELSNAP_ON" }
SetTexture 0 [_MainTex] 2D 0
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]]
  ,   texture2d<half> _MainTex [[texture(0)]], sampler _mtlsmp__MainTex [[sampler(0)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  half4 tmpvar_2;
  tmpvar_2 = (_MainTex.sample(_mtlsmp__MainTex, (float2)(_mtl_i.xlv_TEXCOORD0)) * _mtl_i.xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
Keywords { "PIXELSNAP_ON" }
"// shader disassembly not supported on glcore"
}
SubProgram "opengl " {
Keywords { "ETC1_EXTERNAL_ALPHA" "PIXELSNAP_ON" }
"// shader disassembly not supported on opengl"
}
SubProgram "metal " {
// Stats: 2 math, 2 textures
Keywords { "ETC1_EXTERNAL_ALPHA" "PIXELSNAP_ON" }
SetTexture 0 [_MainTex] 2D 0
SetTexture 1 [_AlphaTex] 2D 1
"#include <metal_stdlib>
#pragma clang diagnostic ignored "-Wparentheses-equality"
using namespace metal;
struct xlatMtlShaderInput {
  half4 xlv_COLOR;
  float2 xlv_TEXCOORD0;
};
struct xlatMtlShaderOutput {
  half4 _glesFragData_0 [[color(0)]];
};
struct xlatMtlShaderUniform {
};
fragment xlatMtlShaderOutput xlatMtlMain (xlatMtlShaderInput _mtl_i [[stage_in]], constant xlatMtlShaderUniform& _mtl_u [[buffer(0)]]
  ,   texture2d<half> _MainTex [[texture(0)]], sampler _mtlsmp__MainTex [[sampler(0)]]
  ,   texture2d<half> _AlphaTex [[texture(1)]], sampler _mtlsmp__AlphaTex [[sampler(1)]])
{
  xlatMtlShaderOutput _mtl_o;
  half4 c_1;
  half4 color_2;
  color_2.xyz = _MainTex.sample(_mtlsmp__MainTex, (float2)(_mtl_i.xlv_TEXCOORD0)).xyz;
  color_2.w = _AlphaTex.sample(_mtlsmp__AlphaTex, (float2)(_mtl_i.xlv_TEXCOORD0)).x;
  half4 tmpvar_3;
  tmpvar_3 = (color_2 * _mtl_i.xlv_COLOR);
  c_1.w = tmpvar_3.w;
  c_1.xyz = (tmpvar_3.xyz * tmpvar_3.w);
  _mtl_o._glesFragData_0 = c_1;
  return _mtl_o;
}

"
}
SubProgram "glcore " {
Keywords { "ETC1_EXTERNAL_ALPHA" "PIXELSNAP_ON" }
"// shader disassembly not supported on glcore"
}
}
 }
}
}