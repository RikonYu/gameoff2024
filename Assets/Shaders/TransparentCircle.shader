Shader "UI/TransparentCircle"
{
    Properties
    {
        _MainTex("Base Texture", 2D) = "white" { }
        _Color("Color", Color) = (0,0,0,1)
        _CircleRadius("Circle Radius", Float) = 50
        _ScreenResolution("Screen Resolution", Vector) = (1920,1080,0,0)
        _CirclePosition("Circle Position", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
            };

            fixed4 _Color;
            float _CircleRadius;
            float4 _ScreenResolution;
            float4 _CirclePosition;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 获取UV坐标
                float2 uv = i.uv;

                //uv.y = 1.0 - uv.y;

                // 计算屏幕坐标中的圆心位置
                float2 circleUV = _CirclePosition.xy / _ScreenResolution.xy;

                // 计算距离，考虑宽高比
                float2 diff = uv - circleUV;
                diff.x *= _ScreenResolution.x / _ScreenResolution.y;

                float dist = length(diff) * _ScreenResolution.y;

                if (dist < _CircleRadius)
                {
                    discard;
                }

                return _Color;
            }
            ENDCG
        }
    }
}