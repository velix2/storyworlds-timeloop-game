Shader "UI/OutlineToggle"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)

        [Toggle] _enabled ("Outline Enabled", Float) = 1
        _Color ("Outline Color", Color) = (0,0,0,1)
        _width ("Outline Width", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            float4 _Tint;
            float4 _Color;
            float _width;
            float _enabled;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mainCol = tex2D(_MainTex, i.uv) * _Tint;

                // Wenn Outline aus → normales Sprite
                if (_enabled < 0.5)
                    return mainCol;

                // Nur Outline zeichnen, wo Sprite transparent ist
                if (mainCol.a <= 0.001)
                {
                    float2 offset = _MainTex_TexelSize.xy * _width;

                    float alpha =
                        tex2D(_MainTex, i.uv + float2( offset.x, 0)).a +
                        tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a +
                        tex2D(_MainTex, i.uv + float2(0,  offset.y)).a +
                        tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;

                    if (alpha > 0)
                        return _Color;
                }

                return mainCol;
            }
            ENDCG
        }
    }
}
