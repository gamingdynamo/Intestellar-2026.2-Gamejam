Shader "Custom/PostProcess/crazy0ScreenPass"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Source Texture", 2D) = "white" {}  
        _Intensity("Inversion Intensity", Range(0.0, 1.0)) = 1.0
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
            Name "crazy0ScreenPass"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Intensity;
                float _IntensityEffect;
            CBUFFER_END

            half4 Frag(Varyings input) : SV_Target
            {
                
                float4 originalColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                if ( _IntensityEffect == 0.0 ){ return originalColor; }

                float4 outColor = {0.0,0.0,0.0,0.0};
                float4 inColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

                float3 invertedColor = 1.0 - inColor.rgb;
                outColor.rgb = lerp(inColor.rgb, invertedColor.rgb, _Intensity);
                outColor.a = inColor.a;

                return lerp(originalColor, outColor, _IntensityEffect);
            }

            ENDHLSL
        }
    }
}