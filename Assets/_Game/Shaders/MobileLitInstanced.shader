Shader "Custom/URP/MobileLitInstancedURP_WithShadows"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS

            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "UnityInstancing.cginc"

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _BaseColor)
            UNITY_INSTANCING_BUFFER_END(Props)

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float4 shadowCoords : TEXCOORD1;
                half3 lightAmt      : TEXCOORD2;
                UNITY_VERTEX_OUTPUT_INSTANCE_ID
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);

                OUT.positionCS = TransformObjectToHClip(IN.positionOS);

                float3 normalWS = normalize( TransformObjectToWorldNormal(IN.normalOS) );
                Light main = GetMainLight();
                OUT.lightAmt = LightingLambert(main.color, main.direction, normalWS);

                OUT.uv = IN.uv;

                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS);
                OUT.shadowCoords = GetShadowCoord(posInputs);

                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);

                float4 baseCol = UNITY_ACCESS_INSTANCED_PROP(Props, _BaseColor);
                half4 texCol  = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

                half3 col = baseCol.rgb * texCol.rgb * IN.lightAmt;
                half shadow = MainLightRealtimeShadow(IN.shadowCoords);
                col *= shadow;

                return half4(col, baseCol.a * texCol.a);
            }
            ENDHLSL
        }
    }
    FallBack Off
}
