Shader "Custom/HeightFog"
{
    Properties
    {
        _FogColor ("Fog Color", Color) = (1, 1, 1, 1)
        _FogStart ("Fog Start Height", Float) = 0.0
        _FogEnd ("Fog End Height", Float) = 100.0
        _FogDensity ("Fog Density", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Background" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 worldPos : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float _FogStart;
            float _FogEnd;
            float _FogDensity;
            float4 _FogColor;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.worldPos;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float height = i.worldPos.y;
                float fogFactor = 0.0;

                if (height > _FogStart)
                {
                    float t = (height - _FogStart) / (_FogEnd - _FogStart);
                    fogFactor = 1.0 - exp(-_FogDensity * t * t);
                }

                return lerp(_FogColor, float4(1, 1, 1, 1), fogFactor);
            }
            ENDCG
        }
    }
}
