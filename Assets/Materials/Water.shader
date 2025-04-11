Shader "Custom/LowPolyWaterV2" {
    Properties {
        _Color ("Water Color", Color) = (0.1, 0.3, 0.8, 0.8)
        _WaveSpeed ("Wave Speed", Range(0,2)) = 0.8
        _WaveHeight ("Wave Height", Range(0,1)) = 0.2
        _WaveFrequency ("Wave Frequency", Range(0,5)) = 2.5
        _NoiseStrength ("Noise Strength", Range(0,0.5)) = 0.1
    }

    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert addshadow
        #pragma target 3.0

        #include "UnityCG.cginc"

        struct Input {
            float3 worldPos;
        };

        fixed4 _Color;
        float _WaveSpeed;
        float _WaveHeight;
        float _WaveFrequency;
        float _NoiseStrength;

        // Gerstner Wave Function
        float3 GerstnerWave(
            float4 waveParams,
            float3 position,
            inout float3 tangent,
            inout float3 binormal
        ) {
            float steepness = waveParams.z;
            float wavelength = waveParams.w;
            float speed = _WaveSpeed * 2;
            
            float k = 2 * UNITY_PI / wavelength;
            float c = sqrt(9.8 / k);
            float2 d = normalize(waveParams.xy);
            float f = k * (dot(d, position.xz) - c * _Time.y * speed);
            float a = steepness / k;

            tangent += float3(
                -d.x * d.x * (steepness * sin(f)),
                d.x * (steepness * cos(f)),
                -d.x * d.y * (steepness * sin(f))
            );
            
            binormal += float3(
                -d.x * d.y * (steepness * sin(f)),
                d.y * (steepness * cos(f)),
                -d.y * d.y * (steepness * sin(f))
            );
            
            return float3(
                d.x * (a * cos(f)),
                a * sin(f),
                d.y * (a * cos(f))
            );
        }

        void vert(inout appdata_full v) {
            // Base wave directions
            float4 wave1 = float4(1,0,0.2,5); // X-direction wave
            float4 wave2 = float4(0,1,0.2,4); // Y-direction wave
            float4 wave3 = float4(0.7,0.7,0.15,3.5); // Diagonal wave
            
            float3 tangent = float3(1,0,0);
            float3 binormal = float3(0,0,1);
            
            // Combine 3 Gerstner waves
            float3 offsets = GerstnerWave(wave1, v.vertex, tangent, binormal);
            offsets += GerstnerWave(wave2, v.vertex, tangent, binormal);
            offsets += GerstnerWave(wave3, v.vertex, tangent, binormal);

            // Add fractal noise
            float noise = sin(_Time.y + v.vertex.x*2) * cos(_Time.y*0.8 + v.vertex.z*2);
            offsets.y += noise * _NoiseStrength;

            // Apply final displacement
            v.vertex.xyz += offsets * _WaveHeight;
            
            // Recalculate normals
            float3 normal = normalize(cross(binormal, tangent));
            v.normal = normal;
        }

        void surf (Input IN, inout SurfaceOutputStandard o) {
            o.Albedo = _Color.rgb;
            o.Alpha = _Color.a;
            o.Metallic = 0.1;
            o.Smoothness = 0.7;
        }
        ENDCG
    }
    FallBack "Diffuse"
}