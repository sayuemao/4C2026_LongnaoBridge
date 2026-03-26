Shader "Custom/HighlightPile"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1, 0.9, 0.2, 1) // 金黄色
        _BlinkSpeed ("Blink Speed", Range(0.1, 10)) = 2.0
        _EdgeWidth ("Edge Width", Range(0.001, 0.02)) = 0.005
        _EdgeThreshold ("Edge Threshold", Range(0.1, 0.9)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

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
            float4 _GlowColor;
            float _BlinkSpeed;
            float _EdgeWidth;
            float _EdgeThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 采样当前像素
                float4 color = tex2D(_MainTex, i.uv);
                
                // 采样相邻像素来检测边缘
                float4 right = tex2D(_MainTex, i.uv + float2(_EdgeWidth, 0));
                float4 left = tex2D(_MainTex, i.uv - float2(_EdgeWidth, 0));
                float4 up = tex2D(_MainTex, i.uv + float2(0, _EdgeWidth));
                float4 down = tex2D(_MainTex, i.uv - float2(0, _EdgeWidth));
                
                // 计算alpha通道的差异
                float alphaDiff = 0.0;
                alphaDiff += abs(color.a - right.a);
                alphaDiff += abs(color.a - left.a);
                alphaDiff += abs(color.a - up.a);
                alphaDiff += abs(color.a - down.a);
                
                // 检测边缘
                float isEdge = step(_EdgeThreshold, alphaDiff);
                
                // 计算闪烁效果
                float blink = sin(_Time.y * _BlinkSpeed) * 0.5 + 0.5;
                
                // 混合边缘发光效果
                float4 finalColor = color;
                finalColor.rgb = lerp(color.rgb, _GlowColor.rgb, isEdge * blink);
                finalColor.a = color.a;
                
                return finalColor;
            }
            ENDCG
        }
    }
}