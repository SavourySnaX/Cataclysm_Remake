// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/BackgroundDither"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
		_BackgroundColor ("BG Replace Colour", Color) = (1,1,1,1)
		_RepBGMin ("BG Min Colour", Color) = (1,1,1,1)
		_RepBGMax ("BG Max Colour", Color) = (1,1,1,1)
		_WorldBounds ("World Bounds", Vector) = (1,1,1,1)
		_GoalColor ("Goal Replace Colour", Color) = (1,1,1,1)
		_RepGoalMin ("Goal Min Colour", Color) = (1,1,1,1)
		_RepGoalMax ("Goal Max Colour", Color) = (1,1,1,1)
	}

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

#include "UnityCG.cginc"

#ifdef UNITY_INSTANCING_ENABLED

    UNITY_INSTANCING_CBUFFER_START(PerDrawSprite)
        // SpriteRenderer.Color while Non-Batched/Instanced.
        fixed4 unity_SpriteRendererColorArray[UNITY_INSTANCED_ARRAY_SIZE];
        // this could be smaller but that's how bit each entry is regardless of type
        float4 unity_SpriteFlipArray[UNITY_INSTANCED_ARRAY_SIZE];
    UNITY_INSTANCING_CBUFFER_END

    #define _RendererColor unity_SpriteRendererColorArray[unity_InstanceID]
    #define _Flip unity_SpriteFlipArray[unity_InstanceID]

#endif // instancing

CBUFFER_START(UnityPerDrawSprite)
#ifndef UNITY_INSTANCING_ENABLED
    fixed4 _RendererColor;
    float4 _Flip;
#endif
    float _EnableExternalAlpha;
CBUFFER_END

// Material Color.
fixed4 _Color;
fixed4 _BackgroundColor;
fixed4 _RepBGMin;
fixed4 _RepBGMax;
fixed4 _GoalColor;
fixed4 _RepGoalMin;
fixed4 _RepGoalMax;
float4 _WorldBounds;

struct appdata_t
{
    float4 vertex   : POSITION;
    float4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex   : SV_POSITION;
    fixed4 color    : COLOR;
    float2 texcoord : TEXCOORD0;
	float2 world : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};

v2f SpriteVert(appdata_t IN)
{
    v2f OUT;

    UNITY_SETUP_INSTANCE_ID (IN);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

#ifdef UNITY_INSTANCING_ENABLED
    IN.vertex.xy *= _Flip.xy;
#endif

    OUT.vertex = UnityObjectToClipPos(IN.vertex);
    OUT.texcoord = IN.texcoord;
    OUT.color = IN.color * _Color * _RendererColor;
	OUT.world = mul(unity_ObjectToWorld, IN.vertex);

    #ifdef PIXELSNAP_ON
    OUT.vertex = UnityPixelSnap (OUT.vertex);
    #endif

    return OUT;
}

sampler2D _MainTex;
sampler2D _AlphaTex;

fixed4 DitherHeight(float2 pos)
{
	float height = _WorldBounds.w - _WorldBounds.y;

	float pixelDither = frac(sin( dot(trunc(pos.xyx*12) ,float3(12.9898,78.233,45.5432) )) * 43758.5453);

	return lerp(_RepBGMin,_RepBGMax,((pos.y-_WorldBounds.y )/height)+ pixelDither/4 );
}

fixed4 DitherDist(float2 pos)
{
	float width = _WorldBounds.z - _WorldBounds.x;
	
	float pixelDither = frac(sin( dot(trunc(pos.xyx*12) ,float3(12.9898,78.233,45.5432) )) * 43758.5453);

	return lerp(_RepGoalMin,_RepGoalMax,abs((pos.x-0.5)/(width/2)+pixelDither/4));
}


fixed4 SampleSpriteTexture (v2f frag)
{
    fixed4 color = tex2D (_MainTex, frag.texcoord);

	color.xyz = all(color.xyz==_BackgroundColor.xyz)? DitherHeight(frag.world).xyz 
		: all(color.xyz==_GoalColor.xyz)? DitherDist(frag.world).xyz : color.xyz;

    return color;
}

fixed4 SpriteFrag(v2f IN) : SV_Target
{
    fixed4 c = SampleSpriteTexture (IN) * IN.color;
    c.rgb *= c.a;
    return c;
}
        ENDCG
        }
    }
}
