
// *************Sa RTM*********
Shader "RTM/Test/FrontBack" {

  Properties {
    _MainTex ("Map Base", 2D) = "white" {}
    _BackTex ("Map test", 2D) = "white" {}
    _DecalTex ("Map test", 2D) = "white" {}
// 		_FrontColor ("Front Color", Color) = (1, 1, 1, 1)
//    _BackColor ("Back Color", Color) = (1, 1, 1, 1)
  }

SubShader {
Tags { "RenderType"="Opaque" }
    LOD 400
    
    //*******************************************
    Cull back//正面   
    CGPROGRAM

    #pragma surface surf BlinnPhong 
    sampler2D _MainTex;
    
    struct Input {
        float2 uv_MainTex;
    };
     
    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = tex.rgb;
    }
    ENDCG
 
    //*******************************************
    Cull front
    CGPROGRAM
    #pragma surface surf BlinnPhong
     
    sampler2D _BackTex;
     
    struct Input {
         float2 uv_BackTex;
    };
     
    void surf (Input IN, inout SurfaceOutput o) {
        fixed4 tex = tex2D(_BackTex, IN.uv_BackTex);
        o.Albedo = tex.rgb;
        }

        
    ENDCG
  }
  
  
  FallBack "Specular"
}
  
  
  
  