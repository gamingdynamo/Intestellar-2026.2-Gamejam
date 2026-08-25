Shader "Custom/PostProcess/crazy10ScreenPass"
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
            Name "crazy10ScreenPass"

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

                float2 uv = input.texcoord;
                float block = floor(uv.y * 20.0);
                float noise = frac(sin(block * 12.9898 + floor(_Time.y * 10.0)) * 43758.5453);

                if (noise > 0.7)
                {
                    uv.x += (noise - 0.7) * 0.3;
                }

                float4 outColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                return lerp(originalColor, outColor, _IntensityEffect);
            }

            ENDHLSL
        }
    }
}