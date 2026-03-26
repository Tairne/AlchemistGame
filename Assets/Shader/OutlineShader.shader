Shader "Custom/Outline"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,0,1)
        _Thickness ("Thickness", Float) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Cull Off

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #pragma vertex vert
            #pragma fragment frag

            float _Thickness;
            float4 _Color;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            Varyings vert (Attributes v)
            {
                Varyings o;
                float3 pos = v.positionOS.xyz + v.normalOS * _Thickness;
                o.positionHCS = TransformObjectToHClip(float4(pos, 1.0));
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                return _Color;
            }
            ENDHLSL
        }
    }
}
