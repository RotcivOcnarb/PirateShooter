Shader "Unlit/IslandShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _IslandTexture("Island Texture", 2D) = "white" {}
        _SandTexture("Sand Texture", 2D) = "white" {}
        _IslandThreshold ("Alpha Threshold", Range(0, 1)) = 0.3
        _SandThreshold ("Alpha Threshold", Range(0, 1)) = 0.6
    }
    SubShader
    {
        Tags { "RenderType"="Transparent"  "Queue" = "Transparent"}
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _IslandTexture;
            sampler2D _SandTexture;

            float _IslandThreshold;
            float _SandThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
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

            fixed4 frag (v2f i) : SV_Target
            {
                float alpha = tex2D(_MainTex, i.uv).r;
                
                float2 uvOffset = WorldToScreenPos(float3(0, 0, 0)) * 10 * _MainTex_ST.xy;
                
                fixed4 island = tex2D(_IslandTexture, i.uv - uvOffset);
                fixed4 sand = tex2D(_SandTexture, i.uv - uvOffset);

                fixed4 col = fixed4(0, 0, 0, 0);

                if (alpha > _SandThreshold) {
                    col = island; 
                }
                else if (alpha > _IslandThreshold) {
                    col = sand;
                }
                 

                return col;
            }
            ENDCG
        }
    }
}
