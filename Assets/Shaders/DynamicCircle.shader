Shader "CustomRenderTexture/DynamicCircle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Fill ("Fill", Range(0, 1)) = 0.5
     }

     SubShader
     {
        Tags { "RenderType"="Opaque" }

        Lighting Off
        Blend One Zero

        Pass
        {
            Name "Foo"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag

            float4 _Color;
            float _Fill;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float2 uv = IN.localTexcoord.xy * 2 - 1;
                float distSqr = dot(uv, uv);
                float sin2 = _SinTime.w * _SinTime.w;
                float min = 1 - _Fill * sin2;
                float minSqr = min * min;
                fixed4 col = (distSqr < 1 && distSqr > minSqr) ? _Color : fixed4(0, 0, 0, 0);
                return col;
            }
            ENDCG
        }
    }
}
