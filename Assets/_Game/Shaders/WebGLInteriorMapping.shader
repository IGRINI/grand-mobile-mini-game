Shader "Custom/URP/WebGLInteriorMapping"
{
    Properties
    {
        _MainTex("Facade Texture", 2D) = "white" {}
        _InteriorTex("Interior Texture", 2D) = "white" {}
        _Depth("Room Depth", Range(0.1, 2.0)) = 0.5
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(half4, _BaseColor)
            UNITY_INSTANCING_BUFFER_END(Props)

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_InteriorTex);
            SAMPLER(sampler_InteriorTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _InteriorTex_ST;
                half _Depth;
                half _AlphaCutoff;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                half3 normalOS : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
                half ndotl : TEXCOORD2;
                float4 shadowCoord : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                float3 worldViewDir = GetCameraPositionWS() - worldPos;
                half3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
                
                float3 right = normalize(cross(float3(0, 1, 0), worldNormal));
                float3 up = normalize(cross(worldNormal, right));
                float3x3 tangentMatrix = float3x3(right, up, worldNormal);
                
                float3 localViewDir = mul(tangentMatrix, normalize(worldViewDir));
                localViewDir.y = abs(localViewDir.y) + 0.1;
                output.viewDir = localViewDir;
                
                Light mainLight = GetMainLight();
                output.ndotl = saturate(dot(worldNormal, mainLight.direction)) * 0.5 + 0.5;
                output.shadowCoord = GetShadowCoord(posInputs);
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                
                half4 facade = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                clip(facade.a - _AlphaCutoff);
                
                float2 interiorUV = input.uv;
                float2 offset = input.viewDir.xz / input.viewDir.y * _Depth;
                interiorUV += offset;
                
                half4 interior = SAMPLE_TEXTURE2D(_InteriorTex, sampler_InteriorTex, interiorUV);
                
                half mask = step(facade.a, _AlphaCutoff);
                half3 mixed = lerp(facade.rgb, interior.rgb, mask);
                
                Light mainLight = GetMainLight(input.shadowCoord);
                half shadow = mainLight.shadowAttenuation;
                half4 baseColor = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                
                half3 finalColor = mixed * baseColor.rgb * input.ndotl * shadow;
                
                return half4(finalColor, facade.a);
            }
            ENDHLSL
        }
    }
    FallBack Off
} 