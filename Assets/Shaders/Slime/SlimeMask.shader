Shader "Unlit/SlimeMask"
{
    Properties
    {
        _Stencil ("Stencil ID", Int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100
        Zwrite Off
        Blend SrcAlpha OneMinusSrcAlpha


        Pass
        {
            Stencil
            {
                Ref [_Stencil]
                Comp always
                Pass replace
            }

            CGPROGRAM
            #pragma vertex vert alpha
            #pragma fragment frag alpha
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return 0;
            }
            ENDCG
        }
    }
}
