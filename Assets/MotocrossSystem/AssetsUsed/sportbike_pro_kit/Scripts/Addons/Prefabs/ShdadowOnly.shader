Shader "Custom/ShadowOnly"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Lighting Off
        ZWrite On
        ColorMask 0
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
        }
    }
    Fallback Off
}