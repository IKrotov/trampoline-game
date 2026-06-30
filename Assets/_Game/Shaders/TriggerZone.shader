Shader "Custom/TriggerZone"
{
    Properties
    {
        _Color          ("Color", Color) = (0, 1, 1, 1)
        _Intensity      ("Intensity", Range(0, 3)) = 1.0
        _FresnelPower   ("Fresnel Power", Range(0.5, 8)) = 2.0
        _FresnelMin     ("Fresnel Min", Range(0, 1)) = 0.05
        _ScanFrequency  ("Scanline Frequency", Range(1, 50)) = 8.0
        _ScanSpeed      ("Scanline Speed", Range(-10, 10)) = 2.0
        _ScanWidth      ("Scanline Width", Range(0, 0.99)) = 0.15
        _ScanBrightness ("Scanline Brightness", Range(0, 3)) = 1.0
        _PulseSpeed     ("Pulse Speed", Range(0, 5)) = 1.5
        _PulseAmount    ("Pulse Amount", Range(0, 1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Transparent"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off                // видно и снаружи и изнутри цилиндра

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float  _Intensity;
                float  _FresnelPower;
                float  _FresnelMin;
                float  _ScanFrequency;
                float  _ScanSpeed;
                float  _ScanWidth;
                float  _ScanBrightness;
                float  _PulseSpeed;
                float  _PulseAmount;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float  fogFactor  : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output = (Varyings)0;
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs posInputs  = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs   normInputs = GetVertexNormalInputs(input.normalOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS   = normInputs.normalWS;
                output.fogFactor  = ComputeFogFactor(posInputs.positionCS.z);

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 normalWS  = normalize(input.normalWS);
                float3 viewDir   = GetWorldSpaceNormalizeViewDir(input.positionWS);

                // Fresnel — края ярче, середина прозрачнее
                float NdotV   = saturate(dot(normalWS, viewDir));
                float fresnel = pow(1.0 - NdotV, _FresnelPower);
                fresnel       = max(fresnel, _FresnelMin);

                // Горизонтальные полосы по мировой Y — скользят вверх
                float scanUV = input.positionWS.y * _ScanFrequency - _Time.y * _ScanSpeed;
                float scan   = frac(scanUV);
                scan         = step(1.0 - _ScanWidth, scan); // тонкая светлая линия

                // Пульсация всего эффекта
                float pulse = 1.0 - _PulseAmount + _PulseAmount * (0.5 + 0.5 * sin(_Time.y * _PulseSpeed * TWO_PI));

                // Итог
                float  alpha   = fresnel * pulse * _Color.a;
                float3 emissive = _Color.rgb * (fresnel + scan * _ScanBrightness) * pulse * _Intensity;

                half4 color = half4(emissive, alpha);
                color.rgb = MixFog(color.rgb, input.fogFactor);

                return color;
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Unlit"
}
