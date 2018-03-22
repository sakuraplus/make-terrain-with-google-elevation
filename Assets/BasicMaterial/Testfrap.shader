// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


// *************Sa RTM*********
Shader "RTM/Test/frap" {
  Properties {
       
        _MapTexUp ("Map Up", 2D) = "white" {}
				_MainTex("Map Down", 2D) = "white" {}
	
				_heightMax("pos Max",Range(0,100)) = 0.8 	
				_heightMin("pos Min",Float) = 0.2 
    }
    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform sampler2D _MainTex;
						uniform sampler2D _MapTexUp;
						float _heightMax;
            fixed4 frag(v2f_img i) : SV_Target {
            		fixed4 f;
            		if(i.pos.y>_heightMax){
            			f=tex2D(_MainTex, i.uv);
            		}else{
            			f=tex2D(_MapTexUp, i.uv);
            		}
                return f;
            }
            ENDCG
        }
    }
}
//  SubShader {
//        Pass {
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//
//            #include "UnityCG.cginc"
//
//            struct vertexInput {
//                float4 vertex : POSITION;
//                float4 texcoord0 : TEXCOORD0;
//            };
//
//            struct fragmentInput{
//                float4 position : SV_POSITION;
//                float4 texcoord0 : TEXCOORD0;
//            };
//
//            fragmentInput vert(vertexInput i){
//                fragmentInput o;
//                o.position = UnityObjectToClipPos (i.vertex);
//                o.texcoord0 = i.texcoord0;
//                return o;
//            }
//
//            fixed4 frag(fragmentInput i) : SV_Target {
//                fixed4 color;
//                if ( fmod(i.texcoord0.x*8.0,2.0) < 1.0 ){
//                    if ( fmod(i.texcoord0.y*8.0,2.0) < 1.0 )
//                    {
//                        color = fixed4(1.0,1.0,1.0,1.0);
//                    } else {
//                        color = fixed4(0.0,0.0,0.0,1.0);
//                    }
//                } else {
//                    if ( fmod(i.texcoord0.y*8.0,2.0) > 1.0 )
//                    {
//                        color = fixed4(1.0,1.0,1.0,1.0);
//                    } else {
//                        color = fixed4(0.0,0.0,0.0,1.0);}
//                    }
//                return color;
//            }
//            ENDCG
//        }
//    }
//}