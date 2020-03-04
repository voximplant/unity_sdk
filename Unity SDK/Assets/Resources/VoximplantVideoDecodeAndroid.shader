Shader "Hidden/Voximplant/VideoDecodeAndroid"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
    }

    SubShader
    {
         Pass 
         {
            Name "Rotate_FlipV_OESExternal_To_RGBA"

            GLSLPROGRAM

            #extension GL_OES_EGL_image_external : require
            #pragma glsl_es2

            #ifdef VERTEX

            varying vec2 textureCoord;
            uniform float rotation;
            uniform float local;
            uniform float front;
            
            void main()
            {
                gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
                
                textureCoord = vec2(gl_MultiTexCoord0.x, gl_MultiTexCoord0.y);
                if (front == 0.0) {
                    textureCoord = vec2(1.0 - textureCoord.x, textureCoord.y);
                }
                
                textureCoord = vec2(0.5 - textureCoord.x, 0.5 - textureCoord.y);
                float sin_factor = sin(rotation);
                float cos_factor = cos(rotation);
                textureCoord = vec2(textureCoord.x * cos_factor - textureCoord.y * sin_factor + 0.5, textureCoord.x * sin_factor + textureCoord.y * cos_factor + 0.5);
            }

            #endif

            #ifdef FRAGMENT

            vec4 AdjustForColorSpace(vec4 color)
            {
            #ifdef UNITY_COLORSPACE_GAMMA
                return color;
            #else
                vec3 sRGB = color.rgb;
                return vec4(sRGB * (sRGB * (sRGB * 0.305306011 + 0.682171111) + 0.012522878), color.a);
            #endif
            }

            varying vec2 textureCoord;
            uniform samplerExternalOES _MainTex;
            void main()
            {
                gl_FragColor = AdjustForColorSpace(textureExternal(_MainTex, textureCoord));
            }

            #endif

            ENDGLSL
         }
    }

    FallBack Off
}
 