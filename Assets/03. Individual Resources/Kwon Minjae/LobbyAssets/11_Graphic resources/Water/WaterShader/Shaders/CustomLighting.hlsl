#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

//#ifndef SHADERGRAPH_PREVIEW
//	#include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
//	#if (SHADERPASS != SHADERPASS_FORWARD)
//		#undef REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR
//	#endif
//#endif

struct CustomLightingData
{
	float3 lightDir;
	float lightIntensity;

	float3 positionWS;
	float3 normalWS;
	float3 viewDirectionWS;
	//float4 shadowCoord;

	float3 albedo;
	float smoothness;
	float ambientOcclusion;

	//float3 bakedGI;

	float fogFactor;
};

float GetSmoothnessPower(float rawSmoothness)
{
	return exp2(12 * rawSmoothness + 1);
}

#ifndef SHADERGRAPH_PREVIEW
float3 CustomGlobalIllumination(CustomLightingData d)
{
	//float3 indirectDiffuse = d.albedo * d.bakedGI * d.ambientOcclusion;
	float3 indirectDiffuse = d.albedo * d.ambientOcclusion;
	
	float3 reflectVector = reflect(-d.viewDirectionWS, d.normalWS);

	//float fresnel = Pow4(1 - saturate(dot(d.viewDirectionWS, d.normalWS)));

	float3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, RoughnessToPerceptualRoughness(1 - d.smoothness), d.ambientOcclusion);// * fresnel;

	return indirectDiffuse * indirectSpecular;
}

float3 CustomLightHandling(CustomLightingData d)//, Light light)
{
	//float3 radiance = 1;//light.color;
	//float3 radiance = light.color * (light.distanceAttenuation * light.shadowAttenuation);

	//float diffuse = saturate(dot(d.normalWS, light.direction)) + 1;
	//float specularDot = saturate(dot(d.normalWS, normalize(light.direction + d.viewDirectionWS)));
	float diffuse = saturate(dot(d.normalWS, d.lightDir)) * 0.2 + 0.8;
	float specularDot = saturate(dot(d.normalWS, normalize(d.lightDir + d.viewDirectionWS)));
	float specular = pow(specularDot, GetSmoothnessPower(d.smoothness)) * diffuse * 0.5;
	//float specular = pow(specularDot, GetSmoothnessPower(d.smoothness))* diffuse * 50;

	float3 color =  d.lightIntensity * d.albedo * diffuse + clamp(specular, 0, 0.5);// + specular;// (diffuse + specular);
	//float3 color =  clamp(d.lightIntensity * d.albedo * diffuse * (specular + 1), 0, 0.3);// + specular;// (diffuse + specular);
	return color;
}
#endif

float3 CalulateCustomLighting(CustomLightingData d)
{
	#ifdef SHADERGRAPH_PREVIEW
		float3 lightDir = float3(0.5,0.5,0);
		float intensity = saturate(dot(d.normalWS, lightDir)) + pow(saturate(dot(d.normalWS, normalize(d.viewDirectionWS + lightDir))), GetSmoothnessPower(d.smoothness));
		return d.albedo * intensity;
	#else
		Light mainLight = GetMainLight();
		//Light mainLight = GetMainLight(d.shadowCoord, d.positionWS, 1);

		//MixRealtimeAndBakedGI(mainLight, d.normalWS, d.bakedGI);
		//float3 color = 0;
		float3 color = CustomGlobalIllumination(d);

		color += CustomLightHandling(d);//, mainLight);

		//#ifdef _ADDITIONAL_LIGHTS
		//	uint numAdditionalLights = GetAdditionalLightsCount();
		//	for	(uint lightI = 0; lightI < numAdditionalLights; lightI++)
		//	{
		//		Light light = GetAdditionalLight(lightI, d.positionWS, 1);
		//		color += CustomLightHandling(d, light);
		//	}
		//#endif

		color = MixFog(color, d.fogFactor);

		return color;
	#endif
}

void CalulateCustomLighting_half(float3 LightDir, float LightIntensity, float3 Position, float3 Normal, float3 ViewDirection,
	float3 Albedo, float Smoothness, float AmbientOcclusion,// float2 LightmapUV,
	out float3 Color)
{
	CustomLightingData d;

	d.lightDir = LightDir;
	d.lightIntensity = LightIntensity;
	d.positionWS = Position;
	d.normalWS = Normal;
	d.viewDirectionWS = ViewDirection;
	d.albedo = Albedo;
	d.smoothness = Smoothness;
	d.ambientOcclusion = AmbientOcclusion;

	#ifdef SHADERGRAPH_PREVIEW

		//d.shadowCoord = 0;
		d.fogFactor = 0;
	#else
		float4 positionCS = TransformWorldToHClip(Position);

		//#if SHADOWS_SCREEN
		//	d.shadowCoord = ComputeScreenPos(positionCS);
		//#else
		//	d.shadowCoord = TransformWorldToShadowCoord(Position);
		//#endif

		//float3 lightmapUV;
		//OUTPUT_LIGHTMAP_UV(LightmapUV, unit_LightmapST, lightmapUV);
		//float3 vertexSH;
		//OUTPUT_SH(Normal, vertexSH);
		//d.bakedGI = SAMPLE_GI(lightmapUV, vertexSH, Normal);

		d.fogFactor = ComputeFogFactor(positionCS.z);
	#endif

	Color = CalulateCustomLighting(d);
}

void CalulateCustomLighting_float(float3 LightDir, float LightIntensity, float3 Position, float3 Normal, float3 ViewDirection,
	float3 Albedo, float Smoothness, float AmbientOcclusion,// float2 LightmapUV,
	out float3 Color)
{
	CustomLightingData d;

	d.lightDir = LightDir;
	d.lightIntensity = LightIntensity;
	d.positionWS = Position;
	d.normalWS = Normal;
	d.viewDirectionWS = ViewDirection;
	d.albedo = Albedo;
	d.smoothness = Smoothness;
	d.ambientOcclusion = AmbientOcclusion;

	#ifdef SHADERGRAPH_PREVIEW

		//d.shadowCoord = 0;
		d.fogFactor = 0;
	#else
		float4 positionCS = TransformWorldToHClip(Position);

		//#if SHADOWS_SCREEN
		//	d.shadowCoord = ComputeScreenPos(positionCS);
		//#else
		//	d.shadowCoord = TransformWorldToShadowCoord(Position);
		//#endif

		//float3 lightmapUV;
		//OUTPUT_LIGHTMAP_UV(LightmapUV, unit_LightmapST, lightmapUV);
		//float3 vertexSH;
		//OUTPUT_SH(Normal, vertexSH);
		//d.bakedGI = SAMPLE_GI(lightmapUV, vertexSH, Normal);

		d.fogFactor = ComputeFogFactor(positionCS.z);
	#endif

	Color = CalulateCustomLighting(d);
}
#endif