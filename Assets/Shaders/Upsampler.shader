Shader "ImageEffect/Upsampler"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        _MainTexSize ("Texture Size", Float) = 256
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 pos : TEXCOORD1;
            };

            float _MainTexSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.pos = v.uv * _MainTexSize * 2;
                return o;
            }

            sampler2D _MainTex;

            static const bool lut[4][9] = {
//                { 0, 0, 0, 0, 0, 1 },
//                { 0, 0, 0, 1, 1, 1 },
//                { 0, 0, 1, 0, 1, 1 },
//                { 0, 1, 1, 1, 1, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 0, 0, 0, 0, 1, 1, 1, 0, 1 },
                { 0, 0, 1, 1, 0, 0, 1, 0, 1 },
                { 0, 1, 0, 1, 0, 1, 1, 0, 1 }
            };

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                int col_idx = min(5, (int)(col.a * 32));

                // compute output pixel
                int x = (int)i.pos.x;
                int y = (int)i.pos.y;
                int pos_idx = (x & 1) | ((y & 1) << 1);

                bool mask = lut[pos_idx][col_idx];
//                bool mask = true;

                return (mask ? col : fixed4(0,0,0,0));
            }
            ENDCG
        }
    }
}
