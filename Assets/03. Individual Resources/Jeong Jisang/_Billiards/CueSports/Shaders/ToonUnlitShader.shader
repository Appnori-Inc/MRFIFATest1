Shader "Custom/CelShader" 
{
	Properties 
    {
        _MainTex("Main Texture",2D) = "white" {}
        _OutlineBold ("Outline", Range(0,1)) = 0.1
        _Band_Tex("Band LUT",2D) = "white" {}
	}

	SubShader 
    {
		Tags 
        {
			"RenderType" = "Opaque"
		}

        Cull front
        Pass 
        {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata 
            {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f 
            {
				float4 vertex : SV_POSITION;
			};

            float _OutlineBold;


			v2f vert(appdata v) 
            {
				v2f o;

                float3 normalizedNormal = normalize(v.normal);
                float3 outlinePosition = v.vertex + normalizedNormal*(_OutlineBold * 0.1f);

				o.vertex = UnityObjectToClipPos(outlinePosition);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target 
            {
				return 0.0f;
			}
			ENDCG
		}
    
        Cull Back

        CGPROGRAM
        #pragma surface surf _BandedLighting

        struct Input 
        {
            float2 uv_MainTex;
            float2 uv_Band_Tex;
        };

        sampler2D _MainTex;
        sampler2D _Band_Tex;

        float _OutlineBold;


        void surf(Input IN, inout SurfaceOutput o) 
        {
            float4 mainTex = tex2D(_MainTex,IN.uv_MainTex);
            o.Albedo = mainTex.rgb;
            o.Alpha = 1.0f;
        }

        float4 Lighting_BandedLighting(SurfaceOutput s,float3 lightDir, float3 viewDir,float atten)
        {
            float3 fBandedDiffuse;
            float fNDotL = dot(s.Normal, lightDir) * 0.5f + 0.5f;  

            float fBandNum = 3.0f;
            fBandedDiffuse = ceil(fNDotL * fBandNum) / fBandNum; 
            
            float4 fFinalColor;
            fFinalColor.rgb = (s.Albedo) * fBandedDiffuse * _LightColor0.rgb * atten;
            fFinalColor.a = s.Alpha;

            return fFinalColor;
        }

        ENDCG
    
	}
	FallBack "Standard"
}