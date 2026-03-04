Shader "CustomRenderTexture/CloudShadows"
{
    Properties
    {
        _Speed ("Speed", Vector) = (0.1, 0, 0.1, 0)
        _MainTex("Text_A", 2D) = "white" {}
        _MainTex2("Text_B", 2D) = "white" {}
     }

     SubShader
     {
        Blend One Zero
        Lighting Off

        Pass
        {
            Name "CloudShadows"

            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            float4      _Speed;
            sampler2D   _MainTex;
            float4      _MainText_A;
            sampler2D   _MainTex2;
            float4      _MainText2_A;

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                float4 cloud = tex2D(_MainTex, IN.localTexcoord.xy + frac(_Time * _Speed.xy));
                float4 cloud2 = tex2D(_MainTex2, IN.localTexcoord.xy + frac(_Time * _Speed.zw));
                return max(.1f, cloud * cloud2);
            }
            ENDCG
        }
    }
}
