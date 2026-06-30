Shader "Custom/LegoBrickTriplanar"
{
    Properties
    {
        _Color          ("Color", Color) = (1, 1, 1, 1)
        _NormalMap      ("Normal Map", 2D) = "bump" {}
        _NormalScale    ("Normal Scale", Range(0, 3)) = 1.0
        _Smoothness     ("Smoothness", Range(0, 1)) = 0.4
        _Metallic       ("Metallic", Range(0, 1)) = 0.0
        _BlendSharpness ("Blend Sharpness", Range(1, 16)) = 4.0
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue"          = "Geometry"
        }
        LOD 300

        // ── Forward Lit ───────────────────────────────────────────────────────
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DYNAMICLIGHTMAP_ON
            #pragma multi_compile_fog

            // We always need world-space position in the fragment
            #define REQUIRES_WORLD_SPACE_POS_INTERPOLATOR

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _NormalMap_ST;
                float  _NormalScale;
                float  _Smoothness;
                float  _Metallic;
                float  _BlendSharpness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS        : POSITION;
                float3 normalOS          : NORMAL;
                float4 tangentOS         : TANGENT;
                float2 staticLightmapUV  : TEXCOORD1;
                float2 dynamicLightmapUV : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                half   fogFactor    : TEXCOORD2;
                DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 3);
            #ifdef DYNAMICLIGHTMAP_ON
                float2 dynamicLightmapUV : TEXCOORD7;
            #endif
            #ifdef USE_APV_PROBE_OCCLUSION
                float4 probeOcclusion : TEXCOORD8;
            #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ── Triplanar helpers ─────────────────────────────────────────────

            float3 TriplanarWeights(float3 n, float sharpness)
            {
                float3 w = pow(abs(n), sharpness);
                return w / (w.x + w.y + w.z + 1e-5);
            }

            // Returns world-space surface normal blended from triplanar normal-map samples.
            float3 TriplanarNormalWS(float3 posWS, float3 normalWS, float2 tiling, float scale, float sharpness)
            {
                float3 w = TriplanarWeights(normalWS, sharpness);

                // UVs: project world position onto each axis plane, scaled by texture Tiling
                float2 uvX = posWS.zy * tiling;
                float2 uvY = posWS.xz * tiling;
                float2 uvZ = posWS.xy * tiling;

                // Mirror UVs so each face reads the texture the right way around
                // Ternary instead of if — avoids non-uniform control flow before texture sampling (Metal/WebGL)
                uvX.x *= (normalWS.x < 0)  ? -1.0 : 1.0;
                uvY.x *= (normalWS.y < 0)  ? -1.0 : 1.0;
                uvZ.x *= (normalWS.z >= 0) ? -1.0 : 1.0;

                float3 tnX = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvX), scale);
                float3 tnY = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvY), scale);
                float3 tnZ = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uvZ), scale);

                // Whiteout blending: add detail normal to geometric normal per axis
                // X projection (yz plane) — tangent=Z, bitangent=Y, normal=X
                float3 nX = normalize(float3(sign(normalWS.x) * tnX.z + normalWS.x, tnX.y, tnX.x));
                // Y projection (xz plane) — tangent=X, bitangent=Z, normal=Y
                float3 nY = normalize(float3(tnY.x, sign(normalWS.y) * tnY.z + normalWS.y, tnY.y));
                // Z projection (xy plane) — tangent=X, bitangent=Y, normal=Z
                float3 nZ = normalize(float3(tnZ.x, tnZ.y, sign(normalWS.z) * tnZ.z + normalWS.z));

                return normalize(nX * w.x + nY * w.y + nZ * w.z);
            }

            // ── Vertex ────────────────────────────────────────────────────────

            Varyings vert(Attributes input)
            {
                UNITY_SETUP_INSTANCE_ID(input);
                Varyings output = (Varyings)0;
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs posInputs    = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs   normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;
                output.normalWS   = normalInputs.normalWS;
                output.fogFactor  = ComputeFogFactor(posInputs.positionCS.z);

                OUTPUT_LIGHTMAP_UV(input.staticLightmapUV, unity_LightmapST, output.staticLightmapUV);
            #ifdef DYNAMICLIGHTMAP_ON
                output.dynamicLightmapUV = input.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
            #endif
                OUTPUT_SH4(posInputs.positionWS, output.normalWS.xyz,
                    GetWorldSpaceNormalizeViewDir(posInputs.positionWS),
                    output.vertexSH, output.probeOcclusion);

                return output;
            }

            // ── Fragment ──────────────────────────────────────────────────────

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float3 geomNormalWS  = normalize(input.normalWS);
                float3 finalNormalWS = TriplanarNormalWS(
                    input.positionWS, geomNormalWS,
                    _NormalMap_ST.xy, _NormalScale, _BlendSharpness);

                SurfaceData surface = (SurfaceData)0;
                surface.albedo     = _Color.rgb;
                surface.alpha      = _Color.a;
                surface.metallic   = _Metallic;
                surface.smoothness = _Smoothness;
                surface.normalTS   = float3(0, 0, 1);
                surface.occlusion  = 1.0;

                InputData inputData = (InputData)0;
                inputData.positionWS              = input.positionWS;
                inputData.positionCS              = input.positionCS;
                inputData.normalWS                = finalNormalWS;
                inputData.viewDirectionWS         = GetWorldSpaceNormalizeViewDir(input.positionWS);
            #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE) || defined(_MAIN_LIGHT_SHADOWS_SCREEN)
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
            #else
                inputData.shadowCoord = float4(0, 0, 0, 0);
            #endif
                inputData.fogCoord                = InitializeInputDataFog(float4(input.positionWS, 1.0), input.fogFactor);
                inputData.vertexLighting          = half3(0, 0, 0);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);

            #if defined(DYNAMICLIGHTMAP_ON)
                inputData.bakedGI    = SAMPLE_GI(input.staticLightmapUV, input.dynamicLightmapUV, input.vertexSH, finalNormalWS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
            #elif !defined(LIGHTMAP_ON) && (defined(PROBE_VOLUMES_L1) || defined(PROBE_VOLUMES_L2))
                inputData.bakedGI = SAMPLE_GI(input.vertexSH,
                    GetAbsolutePositionWS(input.positionWS),
                    finalNormalWS,
                    inputData.viewDirectionWS,
                    input.positionCS.xy,
                    input.probeOcclusion,
                    inputData.shadowMask);
            #else
                inputData.bakedGI    = SAMPLE_GI(input.staticLightmapUV, input.vertexSH, finalNormalWS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(input.staticLightmapUV);
            #endif

                half4 color = UniversalFragmentPBR(inputData, surface);
                color.rgb = MixFog(color.rgb, inputData.fogCoord);

                return color;
            }
            ENDHLSL
        }

        // ── Shadow Caster ─────────────────────────────────────────────────────
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex   ShadowPassVertex
            #pragma fragment ShadowPassFragment
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            // Minimal CBUFFER so the SRP batcher is happy (no alpha cutout, so nothing used)
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _NormalMap_ST;
                float  _NormalScale;
                float  _Smoothness;
                float  _Metallic;
                float  _BlendSharpness;
            CBUFFER_END

            #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
            ENDHLSL
        }

        // ── Depth Only ────────────────────────────────────────────────────────
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }

            ZWrite On
            ColorMask R
            Cull Back

            HLSLPROGRAM
            #pragma vertex   DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _NormalMap_ST;
                float  _NormalScale;
                float  _Smoothness;
                float  _Metallic;
                float  _BlendSharpness;
            CBUFFER_END

            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        // ── Depth Normals ─────────────────────────────────────────────────────
        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormals" }

            ZWrite On
            Cull Back

            HLSLPROGRAM
            #pragma vertex   DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _NormalMap_ST;
                float  _NormalScale;
                float  _Smoothness;
                float  _Metallic;
                float  _BlendSharpness;
            CBUFFER_END

            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
