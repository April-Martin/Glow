Shader "Custom/AxisTester" {
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_ScreenWidth("Screen Width", Float) = 960
		_ScreenHeight("Screen Height", Float) = 540
		_LightTex("Light Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
		//Blend Off

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma shader_feature ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _LightTex;
			sampler2D _AlphaTex;
			float _ScreenHeight;
			float _ScreenWidth;

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				color.a = tex2D (_AlphaTex, uv).r;
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{		
					
			float4 screenCoords = IN.vertex;
				float4 uvCoords;

				uvCoords[0] = (screenCoords[0]/_ScreenWidth);
				uvCoords[1] = (screenCoords[1]/_ScreenHeight);
				uvCoords[2] = 0;
				uvCoords[3] = 1;


				fixed4 test;
				test[0] = 1;
				test[1] = 0;
				test[2] = 0;
				test[3] = 1;
				// CORRECT FOR DIFFERING DIRECT3D / OPENGL CONVENTIONS
				if (_ProjectionParams.x >= 0)
				//	uvCoords[1] = 1 - uvCoords[1];
				{
					return test;
				}
				else
				{
					test[0] = 0;
					test[1] = 1;
					return test;
				}

				fixed4 mainTex = tex2D(_MainTex, IN.texcoord);
				fixed4 lightTex = tex2D (_LightTex, uvCoords);
				fixed4 combined = mainTex * lightTex * mainTex.a;


				return combined;



			}
		ENDCG
		}
	}

}
