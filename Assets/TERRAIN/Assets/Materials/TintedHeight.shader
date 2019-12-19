Shader "TintedHeight" {

	Properties
	{
	  _MainTex("Base (RGB)", 2D) = "white" {}
	  _HeightMinWater("Height Min Of Water", Float) = -1
	  _HeightMaxWater("Height Max Of Water", Float) = 1
	  _ColorMinWater("Tint Color Of Water At Min", Color) = (0,0,0,1)
	  _ColorMaxWater("Tint Color Of Water At Max", Color) = (1,1,1,1)
	  _HeightMinBeach("Height Min Of Beach", Float) = -1
	  _HeightMaxBeach("Height Max Of Beach", Float) = 1
	  _ColorMinBeach("Tint Color Of Beach At Min", Color) = (0,0,0,1)
	  _ColorMaxBeach("Tint Color Of Beach At Max", Color) = (1,1,1,1)
	  _HeightMinPlain("Height Min Of Plain", Float) = -1
	  _HeightMaxPlain("Height Max Of Plain", Float) = 1
	  _ColorMinPlain("Tint Color Of Plain At Min", Color) = (0,0,0,1)
	  _ColorMaxPlain("Tint Color Of Plain At Max", Color) = (1,1,1,1)
	  _HeightMinRocks("Height Min Of Rocks", Float) = -1
	  _HeightMaxRocks("Height Max Of Rocks", Float) = 1
	  _ColorMinRocks("Tint Color Of Rocks At Min", Color) = (0,0,0,1)
	  _ColorMaxRocks("Tint Color Of Rocks At Max", Color) = (1,1,1,1)
	  _HeightMinSnow("Height Min Of Snow", Float) = -1
	  _HeightMaxSnow("Height Max Of Snow", Float) = 1
	  _ColorMinSnow("Tint Color Of Snow At Min", Color) = (0,0,0,1)
	  _ColorMaxSnow("Tint Color Of Snow At Max", Color) = (1,1,1,1)
	}

		SubShader{
			Pass {

				CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				float _HeightMaxWater;
				float _HeightMinWater;
				fixed4 _ColorMinWater;
				fixed4 _ColorMaxWater;
				float _HeightMaxBeach;
				float _HeightMinBeach;
				fixed4 _ColorMinBeach;
				fixed4 _ColorMaxBeach;
				float _HeightMaxPlain;
				float _HeightMinPlain;
				fixed4 _ColorMinPlain;
				fixed4 _ColorMaxPlain;
				float _HeightMaxRocks;
				float _HeightMinRocks;
				fixed4 _ColorMinRocks;
				fixed4 _ColorMaxRocks;
				float _HeightMaxSnow;
				float _HeightMinSnow;
				fixed4 _ColorMinSnow;
				fixed4 _ColorMaxSnow;

				struct Input
			 {
			   float3 worldPos;
			 };

				struct v2f {
					float4 pos : SV_POSITION;
					fixed3 color : COLOR0;
				};

				v2f vert(appdata_base v)
				{
					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					 float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
					 if (_HeightMaxWater > worldPos.y)
					 { 
						float h = (_HeightMaxWater - worldPos.y) / (_HeightMaxWater - _HeightMinWater);
						o.color = lerp(_ColorMaxWater.rgba, _ColorMinWater.rgba, h);
					 }
					 else if (_HeightMaxBeach > worldPos.y)
					 {
						 float h = (_HeightMaxBeach - worldPos.y) / (_HeightMaxBeach - _HeightMinBeach);
						 o.color = lerp(_ColorMaxBeach.rgba, _ColorMinBeach.rgba, h);
					 }
					 else if (_HeightMaxPlain > worldPos.y)
					 {
						 float h = (_HeightMaxPlain - worldPos.y) / (_HeightMaxPlain - _HeightMinPlain);
						 o.color = lerp(_ColorMaxPlain.rgba, _ColorMinPlain.rgba, h);
					 }
					 else if (_HeightMaxRocks > worldPos.y)
					 {
						 float h = (_HeightMaxRocks - worldPos.y) / (_HeightMaxRocks - _HeightMinRocks);
						 o.color = lerp(_ColorMaxRocks.rgba, _ColorMinRocks.rgba, h);
					 }
					 else if (_HeightMaxSnow > worldPos.y)
					 {
						 float h = (_HeightMaxSnow - worldPos.y) / (_HeightMaxSnow - _HeightMinSnow);
						 o.color = lerp(_ColorMaxSnow.rgba, _ColorMinSnow.rgba, h);
					 }
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					return fixed4(i.color, 1);
				}
				ENDCG

			}
	  }
}