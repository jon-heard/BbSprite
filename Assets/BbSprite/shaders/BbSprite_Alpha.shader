
Shader "Sprites/BbSprites/Alpha"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
		Cull Off
		CGPROGRAM
		#pragma surface surf Lambert addshadow alphatest:_Cutoff

		struct Input
		{
			float2 uv_MainTex;
			float4 color: COLOR;
		};
		fixed4 _Color;
		sampler2D _MainTex;

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb * IN.color.rgb;
			o.Alpha = c.a;
		}

		ENDCG
	}

	FallBack "Transparent/Cutout/Diffuse"
	//CustomEditor "Editor_Shader_Alpha"
}
