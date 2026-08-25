Shader "Custom/PostProcess/crazy3ScreenPass"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Source Texture", 2D) = "white" {}
        _Intensity("Intensity", Range(0.0, 25.0)) = 1.0
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
            Name "crazy3ScreenPass"

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

                float2 uv = input.texcoord;
                float2 dir = uv - 0.5;
                float dist = length(dir);
                float offset = dist * 0.03 * _Intensity; // Intensity

                float r = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + dir * offset).r;
                float g = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv).g;
                float b = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv - dir * offset).b;

                float4 outColor = float4(r, g, b, 1.0);

                return lerp(originalColor, outColor, _IntensityEffect);
            }

            ENDHLSL
        }
    }
}