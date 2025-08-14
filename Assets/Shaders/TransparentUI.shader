Shader "Unlit/TransparentUI"
 {
    Properties{
        _Color("Color",Color) = (1,1,1,1)
        _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
        _Cutoff("Alpha cutoff", Range(0,1)) = 0.5
    }
        SubShader{
            Tags {
             "Queue" = "AlphaTest"
            "IgnoreProjector" = "True" 
            "RenderType" = "Transparent" 
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"}
            LOD 100

            Lighting Off

            Pass {
                CGPROGRAM
                    #pragma vertex vert
                    #pragma fragment frag
                    #pragma target 2.0
                    #pragma multi_compile_fog

                    #include "UnityCG.cginc"

                    struct appdata_t {
                        float4 vertex : POSITION;
                        float2 texcoord : TEXCOORD0;
                        fixed4 color : COLOR; 
                        UNITY_VERTEX_INPUT_INSTANCE_ID
                    };

                    struct v2f {
                        float4 vertex : SV_POSITION;
                        float2 texcoord : TEXCOORD0;
                        fixed4 color : COLOR;
                        UNITY_FOG_COORDS(1)
                        UNITY_VERTEX_OUTPUT_STEREO
                    };

                    sampler2D _MainTex;
                    float4 _MainTex_ST;
                    float4 _Color ;
                    fixed _Cutoff;

                    v2f vert(appdata_t v)
                    {
                        v2f o;
                        UNITY_SETUP_INSTANCE_ID(v);
                        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                        o.vertex = UnityObjectToClipPos(v.vertex);
                        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                        o.color = v.color;
                        UNITY_TRANSFER_FOG(o,o.vertex);
                        return o;
                    }

                    fixed4 frag(v2f i) : SV_Target
                    {
                        fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                        clip(col.a - _Cutoff);
                        UNITY_APPLY_FOG(i.fogCoord, col);
                        return col;
                    }
                ENDCG
            }
        }

}