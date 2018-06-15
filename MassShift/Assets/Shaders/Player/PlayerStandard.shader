Shader "Custom/PlayerStandard" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	_RampTex("Ramp", 2D) = "white" {}
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf PlayerStandard
#pragma target 3.0

	sampler2D _MainTex;
	sampler2D _RampTex;

	struct Input {
		float2 uv_MainTex;
	};

	fixed4 _Color;

	fixed4 LightingPlayerStandard(SurfaceOutput s, fixed3 lightdir, half3 viewdir, fixed atten)
	{
		// ライティングトゥーン
		half d = dot(s.Normal, lightdir) * 0.5 + 0.5;
		fixed3 ramp = tex2D(_RampTex, fixed2(d, 0.5)).rgb;

		// 反射光
		half NdotL = max(0, dot(s.Normal, lightdir));
		float3 R = normalize(-lightdir + 2.0 * s.Normal * NdotL);
		float3 spec =(pow(max(0, dot(R, viewdir)), 10.0) / 8);

		// 最終結果
		fixed4 c;
		c.rgb = s.Albedo * _LightColor0.rgb * ramp * NdotL + spec;
		c.a = 0;
		return c;
	}


	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}
