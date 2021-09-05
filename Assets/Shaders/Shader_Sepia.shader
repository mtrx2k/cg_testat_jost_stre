Shader "Project/Shader_Sepia"
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
                float4 grabPosUV : TEXCOORD0; //grab texture coordinates
                float4 position : SV_POSITION;
            };

            //vertex shader
            v2f vert (appdata v)
            {
                v2f OUT;

                //clip space
                OUT.position = UnityObjectToClipPos(v.vertex);
                //texture coordinate
                OUT.grabPosUV = ComputeGrabScreenPos(OUT.position);
                return OUT;
            }

            //define texture for fragement shader
            sampler2D _GrabTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                float4 pixelRGB = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(i.grabPosUV.x, i.grabPosUV.y, i.grabPosUV.z, i.grabPosUV.w)));
                fixed4 pixelSepia = fixed4(0, 0, 0, pixelRGB.w);

                //convert rgb to sepia
                pixelSepia.r = (0.393 * pixelRGB.r) + (0.769 * pixelRGB.g) + (0.189 * pixelRGB.b);
                pixelSepia.g = (0.349 * pixelRGB.r) + (0.686 * pixelRGB.g) + (0.168 * pixelRGB.b);
                pixelSepia.b = (0.272 * pixelRGB.r) + (0.534 * pixelRGB.g) + (0.131 * pixelRGB.b);
                pixelSepia.a = pixelRGB.a;

                return pixelSepia;
      
            }
            ENDCG
        }
    }
}
