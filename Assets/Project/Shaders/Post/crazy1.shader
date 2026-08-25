Shader "Custom/PostProcess/crazy1ScreenPass"
{
    Properties
    {
        [HideInInspector] _BlitTexture("Source Texture", 2D) = "white" {}  
        _Bands("Bands", Range(0.1, 1000.0)) = 1.0
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
            Name "crazy1ScreenPass"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Bands;
                float _IntensityEffect;
            CBUFFER_END

            half4 Frag(Varyings input) : SV_Target
            {
                float4 originalColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                if ( _IntensityEffect == 0.0 ){ return originalColor; }

                float2 texcoord = input.texcoord;

                float4 outColor = float4(0.0, 0.0, 0.0, 0.0);

                if ( fmod(texcoord.y * _Bands, 2.0) > 1.0)
                {
                    texcoord.x = sin( (1.0 - texcoord.x) * 3.1415 );
                }
                else
                {
                    
                }

                float4 inColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, texcoord);
                outColor = inColor;

                return lerp(originalColor, outColor, _IntensityEffect);
            }

            ENDHLSL
        }
    }
}