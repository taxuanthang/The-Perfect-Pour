Shader "Custom/Unlit_UI_Sphere_Water"
{
    Properties
    {
        [MainTexture] _BaseMap("Texture", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1, 1, 1, 1)
        
        // Tốc độ cuộn (X: ngang, Y: dọc - dùng cho nước chảy)
        _ScrollSpeed("Scroll Speed (X, Y)", Vector) = (0, 0.5, 0, 0)
        
        _Cutoff("AlphaCutout", Range(0.0, 1.0)) = 0.5

        // UI Mask Properties
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "PreviewType"="Plane" // Giữ Plane để UI hiển thị đúng, nhưng mesh áp vào sẽ là Sphere
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Unlit"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 worldPosition : TEXCOORD1; 
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                float4 _ScrollSpeed;
                float4 _ClipRect;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.worldPosition = input.positionOS; 
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                
                // Tính toán UV cuộn dựa trên thời gian (Time.y là thời gian thực)
                // UV = (uv * tiling) + offset + (tốc độ * thời gian)
                float2 scrollingUV = input.uv * _BaseMap_ST.xy + _BaseMap_ST.zw;
                scrollingUV += _ScrollSpeed.xy * _Time.y;
                
                output.uv = scrollingUV;
                output.color = input.color * _BaseColor;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv) * input.color;

                // Logic RectMask2D cho UI
                #ifdef UNITY_UI_CLIP_RECT
                color.a *= step(_ClipRect.x, input.worldPosition.x) * step(input.worldPosition.x, _ClipRect.z) *
                           step(_ClipRect.y, input.worldPosition.y) * step(input.worldPosition.y, _ClipRect.w);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
            ENDHLSL
        }
    }
}