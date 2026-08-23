Shader "Custom/PostProcess/FullScreenHLSL"
{
    Properties
    {
        // URP automatically binds the screen color texture to _BlitTexture
        [HideInInspector] _BlitTexture("Source Texture", 2D) = "white" {}
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
            Name "FullScreenPass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            // Include URP full-screen utilities
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Fragment shader running for every pixel on the screen
            half4 Frag(Varyings input) : SV_Target
            {
                // Sample the screen texture at current pixel UV coordinates
                float4 color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);

                // Example effect: Invert colors
                color.rgb = 1.0 - color.rgb;

                return color;
            }
            ENDHLSL
        }
    }
}