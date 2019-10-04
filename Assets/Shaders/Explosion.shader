Shader "ImageEffect/Explosion"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _PositionX ("Position X", Float) = 0
        _PositionY ("Position Y", Float) = 0
        _Radius ("Radius", Float) = 0
        _InnerRadius ("Inner Radius", Float) = 0
        _Color ("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _PositionX, _PositionY;
            float _Radius;
            float _InnerRadius;
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 position = float2(_PositionX, _PositionY);
                float2 delta = i.uv - position;
                float distSqr = dot(delta, delta);
                float maxSqr = _Radius * _Radius;
                float minSqr = _InnerRadius * _InnerRadius;
                if (distSqr > minSqr && distSqr < maxSqr) col = _Color;
                return col;
            }
            ENDCG
        }
    }
}
