Shader "Unlit/BackgroundTile"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }


            fixed2 WorldToScreenPos(fixed3 pos) {
                pos = normalize(pos - _WorldSpaceCameraPos) * (_ProjectionParams.y + (_ProjectionParams.z - _ProjectionParams.y)) + _WorldSpaceCameraPos;
                fixed2 uv = 0;
                fixed3 toCam = mul(unity_WorldToCamera, pos);
                fixed camPosZ = toCam.z;
                fixed height = 2 * camPosZ / unity_CameraProjection._m11;
                fixed width = _ScreenParams.x / _ScreenParams.y * height;
                uv.x = (toCam.x + width / 2) / width;
                uv.y = (toCam.y + height / 2) / height;
                return uv;
            }

            fixed4 frag(v2f i) : SV_Target
            {

                float2 uv = i.uv;
                uv -= WorldToScreenPos(float3(0, 0, 0)) * 10 * _MainTex_ST.xy;

                uv.x *= _ScreenParams.x / _ScreenParams.y;

                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}