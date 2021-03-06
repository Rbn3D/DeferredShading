﻿Shader "DeferredShading/RadialBlur" {

Properties {
    _BaseColor ("BaseColor", Vector) = (0.3, 0.3, 0.3, 10.0)
    _GlowColor ("GlowColor", Vector) = (0.0, 0.0, 0.0, 0.0)
}
SubShader {
    Tags { "RenderType"="Opaque" "Queue"="Geometry" }

CGINCLUDE
#include "Compat.cginc"

sampler2D frame_buffer;
float4 radialblur_params; // x: size, y: 1.0/size, z: opacity, w: pow
float4 stretch_params;
float3 base_position;
float4 color_bias;

struct ia_out
{
    float4 vertex : POSITION;
    float4 normal : NORMAL;
};

struct vs_out
{
    float4 vertex : SV_POSITION;
    float4 spos : TEXCOORD0;
    float4 center : TEXCOORD1;
    float4 params : TEXCOORD2;
};

struct ps_out
{
    float4 color : COLOR0;
};

vs_out vert(ia_out v)
{
    float3 n = normalize(mul(_Object2World, float4(v.normal.xyz,0.0)).xyz);
    float4 wpos = mul(_Object2World, float4(v.vertex.xyz, 1.0));

    float4 expand = 0.0;
    if(stretch_params.w!=0.0) {
        float d = max(dot(stretch_params.xyz, n.xyz), 0.0);
        expand.xyz = stretch_params.xyz * stretch_params.w * d;
    }

    vs_out o;
    o.vertex = o.spos = mul(UNITY_MATRIX_VP, wpos+expand);
    o.center = mul(UNITY_MATRIX_VP, float4(base_position.xyz, 1.0));
    o.params = 0.0;


    float3 plane = normalize(_WorldSpaceCameraPos - wpos);
    float3 pos_rel = wpos - base_position;
    float dist = dot(pos_rel, plane);
    float3 pos_proj = wpos - dist*plane;
    o.params.x = length(pos_proj-base_position);
    return o;
}

ps_out frag(vs_out i)
{
    float2 coord = screen_to_texcoord(i.spos);
    float2 center = screen_to_texcoord(i.center);
#if UNITY_UV_STARTS_AT_TOP
    coord.y = 1.0 - coord.y;
    center.y = 1.0 - center.y;
#endif
    const int iter = 32;
    float2 dir = normalize(coord-center);
    float step = length(coord-center)*radialblur_params.z / iter;

    float4 ref_color = tex2D(frame_buffer, coord);
    float4 color = 0.0;
    float blend_rate = 0.0;
    ps_out o;
    for(int k=0; k<iter; ++k) {
        float r = 1.0 - (1.0/iter*k);
        blend_rate += r;
        color.rgb += tex2D(frame_buffer, coord - dir*(step*k)).rgb * r;
    }
    color.rgb /= blend_rate;
    float attenuation = i.params.x*radialblur_params.y;
    o.color.rgb = color;
    o.color.a = 1.0-attenuation;

    //// debug
    //if(attenuation>=1.0) {
    //    o.color.rgb = float3(1.0, 0.0, 0.0);
    //}
    o.color.rgb *= 1.0 + color_bias.rgb*radialblur_params.z;
    //o.color.a = 1.0;
    //o.color.r = 0.5;

    return o;
}

ENDCG

    Pass {
        Cull Front
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #pragma target 3.0
        #ifdef SHADER_API_OPENGL 
            #pragma glsl
        #endif
        ENDCG
    }
}
Fallback Off
}
