Shader "Custom/Water" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_RimColor("Rim Color", Color) = (0,0,0,0)
	}
		SubShader{
				Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
				LOD 100

				GrabPass { "_WaterGrab" }
				Pass
				{
					Cull Off
					Blend One Zero
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag

					#include "UnityCG.cginc"

					struct appdata
					{
						float4 vertex : POSITION;
						float2 uv : TEXCOORD0;
						half3 normal : NORMAL;
					};

					struct v2f
					{
						float2 uv : TEXCOORD0;
						float4 vertex : SV_POSITION;
						half4 projCoord: TEXCOORD1; // proj
						half3 viewNormal : TEXCOORD2;
						half define : TEXCOORD3;
					};

					sampler2D _MainTex;
					float4 _MainTex_ST;
					fixed4 _Color;
					fixed4 _RimColor;
					sampler2D _WaterGrab;

					v2f vert(appdata v)
					{
						v2f o;
						o.vertex = UnityObjectToClipPos(v.vertex);
						o.uv = TRANSFORM_TEX(v.uv, _MainTex);
						o.projCoord = ComputeGrabScreenPos(o.vertex);
						o.viewNormal = COMPUTE_VIEW_NORMAL;

						half cN = 1.0 - abs(dot(normalize(mul(UNITY_MATRIX_MV, v.vertex).xyz), o.viewNormal));
						cN *= cN;
						cN *= cN;
						o.define.x = cN;
						return o;
					}

					fixed4 frag(v2f i) : SV_Target
					{
						half kScale = 1.7777;
						half2 enc;
						enc = i.viewNormal.xy / (i.viewNormal.z + 1);
						enc /= kScale;
						i.viewNormal.xy = enc * 0.5;//*0.5+0.5;

						fixed4 col = tex2D(_MainTex, i.uv);
						col *= _Color;
						i.projCoord.xy -= i.viewNormal.xy;
						fixed3 baseColor = tex2Dproj(_WaterGrab, UNITY_PROJ_COORD(i.projCoord));
						col.rgb = lerp(baseColor, col.rgb, col.a);
						col.rgb += i.define.x * _RimColor;
						return col;
					}
				ENDCG
			}
		}
		FallBack "Diffuse"
}


