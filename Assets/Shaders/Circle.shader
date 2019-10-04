Shader "Unlit/Circle"
{
    Properties
    {
        _Color ("Color", Color) = (0, 0, 0, 1)
        _Fill ("Fill", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float3 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            float _Fill;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distSqr = dot(i.uv, i.uv);
                float min = 1 - _Fill;
                float minSqr = min * min;
                fixed4 col = (distSqr < 1 && distSqr > minSqr) ? _Color : fixed4(0, 0, 0, 0);
                return col;
            }
            ENDCG
        }
    }
}
