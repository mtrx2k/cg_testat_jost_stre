Shader "Project/Shader_Grav"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"
        struct appdata
        {
            float4 vertex : POSITION;
            float4 uv : TEXCOORD0;
        };
        struct v2f
        {
            float4 pos : SV_POSITION;
            float4 posUV : TEXCOORD0;
        };

        fixed4 _Color;
        sampler2D _MainTex;

        fixed4 frag(v2f i) : SV_Target 
        {
            
        }

        ENDCG
    }
    FallBack "Diffuse"
}
