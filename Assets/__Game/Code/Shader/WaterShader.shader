Shader "Custom/URP/WaterMetaball2D"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" {}
        _Color ("Water Color", Color) = (0.2, 0.6, 1, 1)

        _Radius ("Particle Radius", Range(0.1, 1)) = 0.45
        _Softness ("Edge Softness", Range(0.01, 0.5)) = 0.15

        _DensityThreshold ("Density Threshold", Range(0, 5)) = 1.2
        _DensitySoft ("Density Softness", Range(0.01, 2)) = 0.5

        _SampleOffset ("Sample Offset", Range(0.001, 0.05)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float _Radius;
            float _Softness;
            float _DensityThreshold;
            float _DensitySoft;
            float _SampleOffset;

            Varyings vert (Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float RadialAlpha(float2 uv)
            {
                float2 centered = uv - 0.5;
                float dist = length(centered);
                return 1.0 - smoothstep(_Radius, _Radius + _Softness, dist);
            }

            float SampleDensity(float2 uv)
            {
                float a = tex2D(_MainTex, uv).a;
                return a * RadialAlpha(uv);
            }

            half4 frag (Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float o = _SampleOffset;

                // Fake SPH density (5 samples)
                float density = 0;
                density += SampleDensity(uv);
                density += SampleDensity(uv + float2( o,  0));
                density += SampleDensity(uv + float2(-o,  0));
                density += SampleDensity(uv + float2( 0,  o));
                density += SampleDensity(uv + float2( 0, -o));

                // Extract surface
                float alpha = smoothstep(
                    _DensityThreshold,
                    _DensityThreshold + _DensitySoft,
                    density
                );

                return half4(_Color.rgb, alpha * _Color.a);
            }
            ENDHLSL
        }
    }
}
