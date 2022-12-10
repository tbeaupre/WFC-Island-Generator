Shader "Custom/IslandShader"
{
    Properties
    {
        _GrassColor ("Grass Color", Color) = (0.527, 0.594, 0.414, 1)
        _CliffColor("Cliff Color", Color) = (0.602, 0.617, 0.648, 1)
        _OceanColor("Ocean Color", Color) = (0.133, 0.363, 0.477, 1)
        _Glossiness ("Smoothness", Range(0,1)) = 0.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _GrassColor;
        fixed4 _CliffColor;
        fixed4 _OceanColor;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            if (IN.worldPos.y < 0.065f)
            {
                o.Albedo = _OceanColor.rgb;
            }
            else
            {
                if (IN.worldNormal.y > 0.9f)
                    o.Albedo = _GrassColor.rgb;
                else
                    o.Albedo = _CliffColor.rgb;
            }
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = 1.0f;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
