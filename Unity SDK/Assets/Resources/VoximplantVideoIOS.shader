Shader "Hidden/Voximplant/VideoRotateIOS"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    }
    SubShader
    {
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert
   
        sampler2D _MainTex;
        
        struct Input {
            float2 uv_MainTex;
        };

        uniform float local;
        uniform float front;
        void vert(inout appdata_full v) {
            if (front == 0.0) {
                v.texcoord.x = 1.0 - v.texcoord.x;
            }
        }
 
        void surf(Input IN, inout SurfaceOutput o) {  
            half4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack Off
}
