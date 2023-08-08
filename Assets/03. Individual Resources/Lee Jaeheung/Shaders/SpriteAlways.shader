// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UI/Default"
  {  
         Properties
         {
                 [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
         }
         SubShader
         {
                 Tags 
                 { 
                         "Queue"="Geometry" 
                         "RenderType"="Opaque"
                         "ForceNoShadowCasting" = "True"
                         "IgnoreProjector"="True"
                 }                
                 
                 Pass
                 {
                         Blend SrcAlpha OneMinusSrcAlpha
                         Cull Back
 
                                 // I tried to edit these with no results
                         ZWrite Off
                         ZTest Off
 
 
 
                         CGPROGRAM
                         #pragma vertex vert
                         #pragma fragment frag
 
                         #pragma target 2.0
                         #pragma glsl_no_auto_normalization
 
                         #pragma only_renderers gles gles3 metal d3d11 glcore
 
 
 
                         sampler2D _MainTex;
 
 
 
                         struct VertexInput
                         {
                             fixed4 position:POSITION;
                             fixed2 uv:TEXCOORD0;
                             fixed4 tintColor:COLOR;
                         };
 
                         struct VertexOutput
                         {
                             fixed4 position : SV_POSITION;
                             fixed2 uv:TEXCOORD0;
                             fixed4 tintColor:COLOR;  
                         };
 
                         VertexOutput vert (VertexInput input)
                         {
                                 VertexOutput output;
                                 output.position = UnityObjectToClipPos (input.position);
                                 output.uv = input.uv;
                                 output.tintColor = input.tintColor;
 
                                 return output;
                         }
 
                         fixed4 frag (VertexOutput input) : COLOR
                         {
                                 fixed4 fragmentColor = tex2D (_MainTex, input.uv) * input.tintColor;
 
                                 return fragmentColor;
                         }
                         ENDCG
                 }
         }   
  } 