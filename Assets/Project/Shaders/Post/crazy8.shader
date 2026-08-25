Shader "Custom/PostProcess/crazy8ScreenPass"
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
            Name "crazy8ScreenPass"

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

                float2 delta = float2(0.002, 0.002); // Texel step size
                float2 uv = input.texcoord;

                float3 c00 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-delta.x, -delta.y)).rgb;
                float3 c10 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( 0.0,     -delta.y)).rgb;
                float3 c20 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( delta.x, -delta.y)).rgb;
                float3 c01 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-delta.x,  0.0)).rgb;
                float3 c21 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( delta.x,  0.0)).rgb;
                float3 c02 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2(-delta.x,  delta.y)).rgb;
                float3 c12 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( 0.0,      delta.y)).rgb;
                float3 c22 = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv + float2( delta.x,  delta.y)).rgb;

                float3 gx = c00 + 2.0*c01 + c02 - (c20 + 2.0*c21 + c22);
                float3 gy = c00 + 2.0*c10 + c20 - (c02 + 2.0*c12 + c22);
                float edge = length(gx) + length(gy);

                float4 outColor = float4(float3(edge, edge, edge), 1.0);

                return lerp(originalColor, outColor, _IntensityEffect);
            }

            ENDHLSL
        }
    }
}