// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Pass"
{
	Properties
	{

	}
		SubShader
		{
			Tags { "RenderType" = "Fade" "Queue" = "Overlay+1000"}
			LOD 200


			Pass
			{
			Cull Off
			ZWrite Off
			//Blend SrcAlpha DstAlpha
			//Blend SrcAlpha OneMinusSrcAlpha
			ZTest Always
			Lighting Off

			//Offset 1, 1

			CGPROGRAM
		#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

		struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};
			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

		v2f vert(appdata v)
		{
			v2f o;
			float4x4 scaleMat;
			scaleMat[0][0] = 1.0;
			scaleMat[0][1] = 0.0;
			scaleMat[0][2] = 0.0;
			scaleMat[0][3] = 0.0;
			scaleMat[1][0] = 0.0;
			scaleMat[1][1] = 1.0;
			scaleMat[1][2] = 0.0;
			scaleMat[1][3] = 0.0;
			scaleMat[2][0] = 0.0;
			scaleMat[2][1] = 0.0;
			scaleMat[2][2] = 1.0;
			scaleMat[2][3] = 0.0;
			scaleMat[3][0] = 0.0;
			scaleMat[3][1] = 0.0;
			scaleMat[3][2] = 0.0;
			scaleMat[3][3] = 1.0;
			o.vertex = mul(scaleMat, UnityObjectToClipPos(v.vertex));
			o.uv = o.uv;
			return o;
		}


			fixed4 frag(v2f i) : SV_Target
			{
				return fixed4(0, 0, 0, 1);
			}



		ENDCG


	}
		}
}