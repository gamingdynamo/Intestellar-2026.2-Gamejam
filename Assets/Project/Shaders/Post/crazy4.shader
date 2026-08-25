Shader "Custom/PostProcess/crazy4ScreenPass"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Source Texture", 2D) = "white" {}
        _IntensityEffect("Effect Intensity", Range(0.0, 1.0)) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
        }

        LOD 100
        ZWrite Off 
        Cull Off 
        ZTest Always

        Pass
        {
            Name "crazy4ScreenPass"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _IntensityEffect;
            CBUFFER_END

            half4 Frag(Varyings input) : SV_Target
            {
                float4 originalColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                if ( _IntensityEffect == 0.0 ){ return originalColor; }

                float pixelSize = 0.01; // Adjust grid size
                float2 uv = floor(input.texcoord / pixelSize) * pixelSize;
                float4 outColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                return lerp(originalColor, outColor, _IntensityEffect);
            }

            ENDHLSL
        }
    }
}