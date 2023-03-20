Shader "Raymarching/LevelRaymarching"
{

Properties
{
    [Header(Base)][Space]
    [MainColor] _BaseColor("Color", Color) = (0.5, 0.5, 0.5, 1)
    [HideInInspector][MainTexture] _BaseMap("Albedo", 2D) = "white" {}
    [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.5
    _Smoothness("Smoothness", Range(0.0, 1.0)) = 0.5
    [ToggleOff] _EnvironmentReflections("Environment Reflections", Float) = 1.0

    [Header(ClearCoat (Forward Only))][Space]
    [Toggle] _ClearCoat("Clear Coat", Float) = 0.0
    [HideInInspector] _ClearCoatMap("Clear Coat Map", 2D) = "white" {}
    _ClearCoatMask("Clear Coat Mask", Range(0.0, 1.0)) = 0.0
    _ClearCoatSmoothness("Clear Coat Smoothness", Range(0.0, 1.0)) = 1.0

    [Header(Pass)][Space]
    [Enum(UnityEngine.Rendering.CullMode)] _Cull("Culling", Int) = 2
    [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("Blend Src", Float) = 5 
    [Enum(UnityEngine.Rendering.BlendMode)] _BlendDst("Blend Dst", Float) = 10
    [Toggle][KeyEnum(Off, On)] _ZWrite("ZWrite", Float) = 1

    [Header(Raymarching)][Space]
    _Loop("Loop", Range(1, 100)) = 30
    _MinDistance("Minimum Distance", Range(0.001, 0.1)) = 0.01
    _DistanceMultiplier("Distance Multiplier", Range(0.001, 2.0)) = 1.0

    [PowerSlider(10.0)] _NormalDelta("NormalDelta", Range(0.00001, 0.1)) = 0.0001

// @block Properties
    _Smooth("Smooth", Range(0.5,5)) = 1.2

    // _NumMetaballs("Number of metaballs", Range(1,16)) = 2
// @endblock
}

SubShader
{

Tags 
{ 
    "RenderType" = "Opaque"
    "Queue" = "Geometry"
    "IgnoreProjector" = "True" 
    "RenderPipeline" = "UniversalPipeline" 
    "UniversalMaterialType" = "Lit"
    "DisableBatching" = "True"
}

LOD 300

HLSLINCLUDE

#define WORLD_SPACE 

#define OBJECT_SHAPE_NONE

#define DISTANCE_FUNCTION DistanceFunction
#define POST_EFFECT PostEffect

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/Primitives.hlsl"
#include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/Math.hlsl"
#include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/Structs.hlsl"
#include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/Utils.hlsl"

// @block DistanceFunction
float _Smooth;

uniform float4 _MetaballTranslations[16];
uniform float _MetaballRadii[16];

uniform int _NumMetaballs;

inline float DistanceFunction(float3 pos)
{
    float expDistance;

    for (int i = 0; i < _NumMetaballs; ++i)
    {
        float sphereDist = Sphere(pos + _MetaballTranslations[i].xyz, _MetaballRadii[i]);

        expDistance += exp( - _Smooth * sphereDist);
    }

    return log(expDistance)/_Smooth;

}
// @endblock

#define PostEffectOutput SurfaceData

// @block PostEffect
inline void PostEffect(RaymarchInfo ray, inout PostEffectOutput o)
{
}
// @endblock

ENDHLSL

Pass
{
    Name "ForwardLit"
    Tags { "LightMode" = "UniversalForward" }

    Blend [_BlendSrc] [_BlendDst]
    ZWrite [_ZWrite]
    Cull [_Cull]

    HLSLPROGRAM

    #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
    #pragma shader_feature_local_fragment _SURFACE_TYPE_TRANSPARENT
    #pragma shader_feature_local_fragment _ALPHATEST_ON
    #pragma shader_feature_local_fragment _ _ALPHAPREMULTIPLY_ON _ALPHAMODULATE_ON
    #pragma shader_feature_local_fragment _EMISSION
    #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
    #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
    #pragma shader_feature_local_fragment _CLEARCOAT_ON
    #ifdef _CLEARCOAT_ON
        #define _CLEARCOAT
    #endif

    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
    #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
    #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
    #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
    #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
    #pragma multi_compile_fragment _ _SHADOWS_SOFT
    #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
    #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
    #pragma multi_compile_fragment _ _LIGHT_LAYERS
    #pragma multi_compile_fragment _ _LIGHT_COOKIES
    #pragma multi_compile _ _CLUSTERED_RENDERING

    #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
    #pragma multi_compile _ SHADOWS_SHADOWMASK
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DYNAMICLIGHTMAP_ON
    #pragma multi_compile_fog
    #pragma multi_compile_fragment _ DEBUG_DISPLAY

    #pragma multi_compile_instancing
    #pragma instancing_options renderinglayer
    #pragma multi_compile _ DOTS_INSTANCING_ON

    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    #pragma target 2.0

    #pragma vertex Vert
    #pragma fragment Frag
    #include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/ForwardLit.hlsl"

    ENDHLSL
}

Pass
{
    Name "GBuffer"
    Tags { "LightMode" = "UniversalGBuffer" }

    ZWrite [_ZWrite]
    ZTest LEqual
    Cull [_Cull]

    HLSLPROGRAM
    #pragma exclude_renderers gles gles3 glcore

    #pragma shader_feature_local _NORMALMAP
    #pragma shader_feature_local_fragment _ALPHATEST_ON
    #pragma shader_feature_local_fragment _EMISSION
    #pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

    #pragma shader_feature_local_fragment _SPECULARHIGHLIGHTS_OFF
    #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
    #pragma shader_feature_local_fragment _SPECULAR_SETUP
    #pragma shader_feature_local _RECEIVE_SHADOWS_OFF
    #pragma shader_feature_local_fragment _ENVIRONMENTREFLECTIONS_OFF
    #pragma shader_feature_local_fragment _CLEARCOAT_ON
    #ifdef _CLEARCOAT_ON
        #define _CLEARCOAT
    #endif

    #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
    #pragma multi_compile_fragment _ _REFLECTION_PROBE_BLENDING
    #pragma multi_compile_fragment _ _REFLECTION_PROBE_BOX_PROJECTION
    #pragma multi_compile_fragment _ _SHADOWS_SOFT
    #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
    #pragma multi_compile_fragment _ _LIGHT_LAYERS
    #pragma multi_compile_fragment _ _RENDER_PASS_ENABLED

    #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
    #pragma multi_compile _ SHADOWS_SHADOWMASK
    #pragma multi_compile _ DIRLIGHTMAP_COMBINED
    #pragma multi_compile _ LIGHTMAP_ON
    #pragma multi_compile _ DYNAMICLIGHTMAP_ON
    #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT

    #pragma multi_compile_instancing
    #pragma instancing_options renderinglayer
    #pragma multi_compile _ DOTS_INSTANCING_ON

    #pragma vertex Vert
    #pragma fragment Frag
    #include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/DeferredLit.hlsl"
    ENDHLSL
}

Pass
{
    Name "DepthOnly"
    Tags { "LightMode" = "DepthOnly" }

    ZWrite On
    ColorMask 0
    Cull [_Cull]

    HLSLPROGRAM

    #pragma shader_feature _ALPHATEST_ON
    #pragma multi_compile_instancing

    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    #pragma target 2.0

    #pragma vertex Vert
    #pragma fragment Frag
    #include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/DepthOnly.hlsl"

    ENDHLSL
}

Pass
{
    Name "DepthNormals"
    Tags { "LightMode" = "DepthNormals" }

    ZWrite On
    Cull [_Cull]

    HLSLPROGRAM

    #pragma shader_feature _ALPHATEST_ON
    #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
    #pragma multi_compile_instancing

    #pragma prefer_hlslcc gles
    #pragma exclude_renderers d3d11_9x
    #pragma target 2.0

    #pragma vertex Vert
    #pragma fragment Frag
    #include "Packages/com.hecomi.uraymarching/Runtime/Shaders/Include/UniversalRP/DepthNormals.hlsl"

    ENDHLSL
}

}

FallBack "Hidden/Universal Render Pipeline/FallbackError"
CustomEditor "uShaderTemplate.MaterialEditor"

}