Shader "Pop/AudioVisualization"
{
    Properties
    {
        _Width ("Width [线条宽度]", Range(1, 10)) = 2
        [HDR]
        _UpColor ("Up Color [上部颜色]", Color) = (1.0, 0.0, 0.0)
        [HDR]
        _DownColor ("Down Color [下部颜色]", Color) = (1.0, 1.0, 1.0)
        _Speed ("Speed [速度]", Range(0.1, 2.0)) = 0.25
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Cull Off ZWrite Off ZTest less

        Pass
        {
            Tags {"LightMode"="ForwardBase"}
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            uniform float _Points[64];
            float _Width;
            float3 _UpColor;
            float3 _DownColor;
            float _Speed;

            //直角坐标转极坐标
            float2 PolarCoordinates(float2 uv, float2 center, float radiusScale, float angleScale)
            {
                //uv偏移center
                uv -= center;
                //长度
                fixed radius = length(uv) * 2 * radiusScale;
                //atan2输出-pi~pi,映射至0~1
                fixed angle = (atan2(uv.y,uv.x) + UNITY_PI) / (UNITY_PI * 2) * angleScale;
                return float2(angle, radius);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //波浪线
                //float x = abs(fmod(i.uv.x * 64 + 16, 32) - 16);
                //float x = abs((i.uv.x - 0.5) * 2 * 16);
                //float startIndex = floor(x);
                //float endIndex = floor(clamp(startIndex + 1, 0, 15));
                //float halfwidth = fwidth(i.uv.y) * _Width;
                //float startValue = clamp(_Points[startIndex], -halfwidth, 1.0);
                //float endValue = clamp(_Points[endIndex], -halfwidth, 1.0);
                //float value = lerp(startValue, endValue, frac(x)) + 0.5;
                //float l = smoothstep(value - halfwidth, value, i.uv.y) - smoothstep(value, value + halfwidth, i.uv.y);
                //float y = clamp((i.uv.y - 0.5) * 2, 0, 1);
                //float3 color = lerp(_DownColor, _UpColor, y) * i.color.rgb;
                //return float4(color.rgb, l * i.color.a);
                //i.uv.x = frac(i.uv.x);
                
                //圆形
                float2 uv = PolarCoordinates(i.uv, float2(0.5, 0.5), 1.0, 1.0);
                uv.x += _Time.x * _Speed;

                float x = abs(fmod(uv.x * 64 + 16, 32) - 16);
                float startIndex = floor(x);
                float endIndex = floor(clamp(startIndex + 1, 0, 15));
                float halfwidth = fwidth(uv.y) * _Width;
                float startValue = clamp(_Points[startIndex], -halfwidth, 1.0);
                float endValue = clamp(_Points[endIndex], -halfwidth, 1.0);
                float value = lerp(startValue, endValue, frac(x)) + 0.5;
                float up = smoothstep(value, value + halfwidth * 0.5, uv.y) - smoothstep(value + halfwidth * 0.5, value + halfwidth, uv.y);
                //value = 1.0 - value;
                //float down = smoothstep(value - halfwidth, value - halfwidth * 0.5, uv.y) - smoothstep(value - halfwidth * 0.5, value, uv.y);
                float y = clamp((uv.y - 0.5) * 2, 0, 1);
                float3 color = lerp(_DownColor, _UpColor, y) * i.color.rgb;
                return float4(color.rgb, /*(up + down)*/ up * i.color.a);
            }
            ENDCG
        }
    }
}
