Shader "Custom/DottedLine"
{
    Properties
    {
        _Space("Space Between Dots", Range(1, 5)) = 1.2
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half2 texcoord : TEXCOORD0;
                half4 vertex : POSITION;
                fixed4 color : COLOR0;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 pos : SV_POSITION;
                fixed4 color : COLOR0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            half _Space;

            fixed4 frag(v2f i) : SV_TARGET
            {
                // 1�̏�]���Ƃ邱�ƂŁA0��1�̊Ԃ��J��Ԃ��悤�ɂ���
                i.uv.x = i.uv.x % 0.35;
                //i.uv.x = (i.uv.x + _Time * 50) % _Space;

                fixed4 col = i.color;

                // �~�̓�����s�����A�O���𓧖��ɂ���
                half s = length(i.uv - half2(1, 2) * 0.25);
                col.a = saturate((0.1 - s) * 100);

                return col;
            }
            ENDCG
        }
    }
}
