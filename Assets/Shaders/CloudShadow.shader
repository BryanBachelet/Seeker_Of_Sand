Shader "CustomRenderTexture/CloudTexture"
{
    Properties
    {
        _Speed ("Speed", Vector) = (0.1, 0, 0.1, 0)
        _MainTex ("InputTex", 2D) = "white" {}
        _MainTex2 ("InputTex", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 10.0 // Échelle du bruit
        _NoiseSpeed ("Noise Speed", Vector) = (0.05, 0.05, 0, 0) // Vitesse du bruit
    }

    SubShader
    {
        Blend One Zero
        Lighting Off

        Pass
        {
            Name "CloudTexture"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4 _Speed;
            sampler2D _MainTex;
            float4 _MainTex_A;
            sampler2D _MainTex2;
            float4 _MainTex2_A;
            float _NoiseScale; // Échelle du bruit
            float4 _NoiseSpeed; // Vitesse du bruit

            // Fonction de hash
            float hash(float n) { return frac(sin(n) * 43758.5453123); }

            // Fonction de bruit de Perlin 2D périodique
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                f = f * f * (3.0 - 2.0 * f);

                float2 u = f * f * (3.0 - 2.0 * f);

                float2 p0 = i + float2(0.0, 0.0);
                float2 p1 = i + float2(1.0, 0.0);
                float2 p2 = i + float2(0.0, 1.0);
                float2 p3 = i + float2(1.0, 1.0);

                float h0 = hash(p0.x + p0.y * 57.0);
                float h1 = hash(p1.x + p1.y * 57.0);
                float h2 = hash(p2.x + p2.y * 57.0);
                float h3 = hash(p3.x + p3.y * 57.0);

                float res = lerp(lerp(h0, h1, u.x), lerp(h2, h3, u.x), u.y);
                return res;
            }

            // Fonction de bruit périodique seamless
            float perlinNoiseSeamless(float2 p)
            {
                float2 q = float2(sin(p.x), cos(p.y));
                float2 r = float2(cos(p.x), sin(p.y));

                return noise(q) * 0.5 + noise(r) * 0.5;
            }

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                // Coordonnées de texture ajustées avec du bruit évolutif
                float2 noiseUV = IN.localTexcoord.xy * _NoiseScale - _Time * _NoiseSpeed.xy; // Inversion de la vitesse pour le bruit
                float noiseValue = perlinNoiseSeamless(noiseUV);
                float2 noiseOffset = noiseValue * float2(0.1, 0.1); // Offset basé sur le bruit

                float2 uv1 = IN.localTexcoord.xy + frac(_Time * _Speed.xy) + noiseOffset;
                float2 uv2 = IN.localTexcoord.xy + frac(_Time * _Speed.zw) + noiseOffset;

                float4 cloud = tex2D(_MainTex, uv1);
                float4 cloud2 = tex2D(_MainTex2, uv2);

                return max(0.1f, cloud * cloud2);
            }
            ENDCG
        }
    }
}
