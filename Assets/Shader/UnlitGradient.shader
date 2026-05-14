Shader "Custom/URPRadialGradient"
{
    Properties
    {
        _CenterColor ("Center Color", Color) = (1, 0.9, 0.2, 1) // Chỉnh sẵn màu vàng nhạt cho bạn
        _EdgeColor ("Edge Color", Color) = (0.2, 0.7, 1, 1)     // Chỉnh sẵn màu xanh dương
        _Radius ("Gradient Spread", Range(0.1, 2.0)) = 0.8      // Thanh trượt chỉnh độ rộng của vùng màu vàng
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" "Queue"="Geometry" }
        LOD 100
        ZWrite On 

        Pass
        {
            Name "Unlit"
            Tags { "LightMode"="UniversalForward" } 

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
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _CenterColor;
                half4 _EdgeColor;
                float _Radius;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // Xác định điểm trung tâm của tấm Quad (tọa độ 0.5, 0.5)
                float2 center = float2(0.5, 0.5);
                
                // Tính khoảng cách từ các điểm xung quanh đến tâm
                float dist = distance(input.uv, center);
                
                // Tính toán độ lan tỏa dựa trên thông số Radius
                float blend = saturate(dist / _Radius);
                
                // Pha trộn giữa màu tâm và màu viền
                return lerp(_CenterColor, _EdgeColor, blend);
            }
            ENDHLSL
        }
    }
}