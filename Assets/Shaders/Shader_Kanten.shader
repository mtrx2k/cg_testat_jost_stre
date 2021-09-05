Shader "Project/Shader_Kanten"
{
    Properties
    {
    }
    SubShader
    {
        // needed to render image into texture and reuse it later

        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        // "Queue"="Transparent": Draw ourselves after all opaque geometry
        // "IgnoreProjector"="True": Don't be affected by any Projectors
        // "RenderType"="Transparent": Declare RenderType as transparent
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

        // Grab the screen behind the object into Default _GrabTexture
        // https://docs.unity3d.com/Manual/SL-GrabPass.html
        GrabPass
        {
        }

        Pass
        {
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
                float2 grabPosUV : TEXCOORD0; //grab texture coordinates
                float4 pos : SV_POSITION;
            };

            // vertex shader
            v2f vert(appdata v)
            {
                v2f o;

                // clip space
                o.pos = UnityObjectToClipPos(v.vertex);

                // texture coordinate
                o.grabPosUV = ComputeGrabScreenPos(o.pos);
                return o;
            }

            //define grab texture
            sampler2D _GrabTexture;

            //size information of textures pixels
            float4 _GrabTexture_TexelSize

            //fragment shader
            fixed4 frag (v2f i) : SV_Target
            {
                
            }
            ENDCG
        }
    }
}
