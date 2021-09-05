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

        // "RenderType"="Transparent": Declare RenderType as transparent
        Tags {"RenderType" = "Transparent"}
        LOD 200

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
            float4 _GrabTexture_TexelSize;

            //sobel operation gt: grabTexture; gPUV: grabPositionUV
            fixed sobelOperation(sampler2D gt, float2 gPUV) {
                float2 pixelDelta = float2(_GrabTexture_TexelSize.x, _GrabTexture_TexelSize.y);

                float4 x = float4(0, 0, 0, 0);
                float4 y = float4(0, 0, 0, 0);

                //grad x
                x += tex2D(gt, (gPUV + float2(-1.0, -1.0) * pixelDelta)) * 1.0;
                x += tex2D(gt, (gPUV + float2(0.0, -1.0) * pixelDelta)) * 0.0;
                x += tex2D(gt, (gPUV + float2(1.0, -1.0) * pixelDelta)) * -1.0;
                x += tex2D(gt, (gPUV + float2(-1.0, 0.0) * pixelDelta)) * 2.0;
                x += tex2D(gt, (gPUV + float2(0.0, 0.0) * pixelDelta)) * 0.0;
                x += tex2D(gt, (gPUV + float2(1.0, 0.0) * pixelDelta)) * -2.0;
                x += tex2D(gt, (gPUV + float2(-1.0, 1.0) * pixelDelta)) * 1.0;
                x += tex2D(gt, (gPUV + float2(0.0, 1.0) * pixelDelta)) * 0.0;
                x += tex2D(gt, (gPUV + float2(1.0, 1.0) * pixelDelta)) * -1.0;

                //grad y
                y += tex2D(gt, (gPUV + float2(-1.0, -1.0) * pixelDelta)) * 1.0;
                y += tex2D(gt, (gPUV + float2(0.0, -1.0) * pixelDelta)) * 2.0;
                y += tex2D(gt, (gPUV + float2(1.0, -1.0) * pixelDelta)) * 1.0;
                y += tex2D(gt, (gPUV + float2(-1.0, 0.0) * pixelDelta)) * 0.0;
                y += tex2D(gt, (gPUV + float2(0.0, 0.0) * pixelDelta)) * 0.0;
                y += tex2D(gt, (gPUV + float2(1.0, 0.0) * pixelDelta)) * 0.0;
                y += tex2D(gt, (gPUV + float2(-1.0, 1.0) * pixelDelta)) * -1.0;
                y += tex2D(gt, (gPUV + float2(0.0, 1.0) * pixelDelta)) * -2.0;
                y += tex2D(gt, (gPUV + float2(1.0, 1.0) * pixelDelta)) * -1.0;

                //relativer betrag
                return sqrt(x * x + y * y);
            }

            //fragment shader: kantenfilter
            float4 frag(v2f_img IN) : COLOR{
            float s = sobelOperation(_GrabTexture, IN.uv);
            return float4(s, s, s, 1);
            }

            ENDCG

        }
    }
}
