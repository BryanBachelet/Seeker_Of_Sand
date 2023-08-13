// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Hidden/MicroSplat/RenderHeight" 
{
  Properties {

  }

  SubShader 
  {
    Tags { "RenderType"="Opaque" }
    Pass 
    {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
      struct v2f 
      {
         float4 pos : SV_POSITION;
         float3 worldPos : TEXCOORD0;
      };

      struct appdata
      {
         float4 vertex   : POSITION;  // The vertex position in model space.
      };

      v2f vert( appdata v ) 
      {
         v2f o;
         UNITY_SETUP_INSTANCE_ID(v);
         o.pos = UnityObjectToClipPos(v.vertex);
         o.worldPos = mul (unity_ObjectToWorld, v.vertex);
         return o;
      }  

      float4 frag(v2f i) : SV_Target 
      {
         float h = i.worldPos.y;
         return float4(h, h, h, 1);
      }

         ENDCG
    }
  
  }
}
