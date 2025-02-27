Shader "Custom/BlurShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _Refraction ("Refraction", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _BumpMap;
        float _Refraction;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 c = tex2D (_MainTex, IN.uv_MainTex);
            half3 bump = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = bump;
            o.Emission = c.rgb * _Refraction;
        }
        ENDCG
    }
    FallBack "Diffuse"
}