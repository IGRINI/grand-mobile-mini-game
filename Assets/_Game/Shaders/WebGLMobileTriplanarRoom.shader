Shader "Custom/URP/WebGLMobileTriplanarRoom"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _MainTex("Facade Texture", 2D) = "white" {}
        _InteriorCube("Interior Cubemap", Cube) = "" {}
        _Tile("Tiling", Vector) = (1,1,0,0)
        _AlphaCutoff("Alpha cutoff", Range(0,1)) = 0.5
        _InteriorScale("Interior Scale (H,V)", Vector) = (1,1,0,0)
        _InteriorInvert("Interior Invert (X,Y,Z)", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200

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
            TEXTURECUBE(_InteriorCube);
            SAMPLER(sampler_InteriorCube);

            float4 _Tile;
            float4 _InteriorScale;
            float4 _InteriorInvert;
            float _AlphaCutoff;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                half3 normalOS      : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS    : SV_POSITION;
                float3 worldPos      : TEXCOORD0;
                half3 worldNormal    : TEXCOORD1;
                float3 blendWeight   : TEXCOORD2;
                half ndotl           : TEXCOORD3;
                float4 shadowCoord   : TEXCOORD4;
                float3 objectPos     : TEXCOORD5;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionCS = posInputs.positionCS;

                float3 worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.worldPos = worldPos;

                float3 worldNormal = TransformObjectToWorldNormal(input.normalOS);
                output.worldNormal = worldNormal;

                float3 n = abs(normalize(worldNormal));
                float sum = n.x + n.y + n.z + 1e-6;
                output.blendWeight = n / sum;

                Light mainLight = GetMainLight();
                output.ndotl = saturate(dot(worldNormal, mainLight.direction)) * 0.5 + 0.5;
                output.shadowCoord = GetShadowCoord(posInputs);
                output.objectPos = TransformObjectToWorld(float3(0,0,0));

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);

                float2 uvX = input.worldPos.yz * _Tile.xy;
                float2 uvY = input.worldPos.xz * _Tile.xy;
                float2 uvZ = input.worldPos.xy * _Tile.xy;

                half4 colX = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvX);
                half4 colY = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvY);
                half4 colZ = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvZ);
                half4 facade = colX * input.blendWeight.x + colY * input.blendWeight.y + colZ * input.blendWeight.z;

                float3 viewDir = normalize(input.worldPos - input.objectPos);
                float3 reflDir = reflect(viewDir, input.worldNormal);
                reflDir.y = -reflDir.y;
                float2 scale = _InteriorScale.xy;
                float2 sampleXZ = lerp(viewDir.xz, reflDir.xz, scale.x);
                float sampleY = lerp(viewDir.y, reflDir.y, scale.y);
                float3 sampleDir = normalize(float3(sampleXZ.x, sampleY, sampleXZ.y));
                sampleDir *= float3(1 - 2*_InteriorInvert.x, 1 - 2*_InteriorInvert.y, 1 - 2*_InteriorInvert.z);
                half4 interior = SAMPLE_TEXTURECUBE(_InteriorCube, sampler_InteriorCube, sampleDir);
                half mask = step(facade.a, _AlphaCutoff);
                half3 mixed = lerp(facade.rgb, interior.rgb, mask);

                Light mainLight = GetMainLight(input.shadowCoord);
                half shadow = mainLight.shadowAttenuation;
                half3 lit = mixed * UNITY_ACCESS_INSTANCED_PROP(Props,_BaseColor).rgb * input.ndotl * shadow;

                return half4(lit, facade.a);
            }

            ENDHLSL
        }
    }
    FallBack Off
} 