
Shader "Sprites/BbSprites/ColorKey_UL"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Main texture", 2D) = "white" {}
		_ColorKeyRange ("Key range", Float) = 0
	}
	SubShader
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True"}
		Cull Off
		CGPROGRAM
		#pragma surface surf Lambert addshadow
		#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
			float4 color: COLOR;
		};
		fixed4 _Color;
		sampler2D _MainTex;
		float _ColorKeyRange;

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed3 key = tex2D(_MainTex, float2(0,.9999f)).rgb;
			o.Albedo = (tex2D (_MainTex, IN.uv_MainTex)).rgb;
			if(distance(o.Albedo, key) <= _ColorKeyRange)
			{
				discard;
			}
			o.Albedo *= IN.color.rgb * _Color.rgb;
		}
		ENDCG

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }
			Fog {Mode Off}
			ZWrite On ZTest Less Cull Off
			Offset [_ShadowBias], [_ShadowBiasSlope]
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcollector
			#pragma multi_compile SHADOWS_NATIVE SHADOWS_CUBE
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 texcoord : TEXCOORD0;
			};
			struct v2f
			{ 
				V2F_SHADOW_CASTER;
				float2  uv : TEXCOORD1;
			};
			sampler2D _MainTex;
			float _ColorKeyRange;
			uniform float4 _MainTex_ST;

			v2f vert( appdata v )
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			float4 frag( v2f i ) : COLOR
			{
				fixed3 key = tex2D(_MainTex, float2(0,.9999f)).rgb;
				fixed3 curColor = tex2D( _MainTex, i.uv ).rgb;
				if(distance(curColor, key) <= _ColorKeyRange)
				{
					discard;
				}
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}

		Pass
		{
			Name "ShadowCollector"
			Tags { "LightMode" = "ShadowCollector" }
			Fog {Mode Off}
			ZWrite On ZTest LEqual
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#define SHADOW_COLLECTOR_PASS
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 texcoord : TEXCOORD0;
			};
			struct v2f
			{
				V2F_SHADOW_COLLECTOR;
				float2  uv : TEXCOORD5;
			};
			sampler2D _MainTex;
			float _ColorKeyRange;
			uniform float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				TRANSFER_SHADOW_COLLECTOR(o)
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed3 key = tex2D(_MainTex, float2(0,.9999f)).rgb;
				fixed3 curColor = tex2D( _MainTex, i.uv ).rgb;
				if(distance(curColor, key) <= _ColorKeyRange)
				{
					discard;
				}

				SHADOW_COLLECTOR_FRAGMENT(i)
			}
			ENDCG
		}

	}
	FallBack Off
	//CustomEditor "Editor_Shader_ColorKey"
}
