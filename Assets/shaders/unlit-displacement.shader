Shader "Unlit/displacement"
{
	Properties
	{
		/* The material's base texture. */
		_MainTex ("Texture", 2D) = "white" {}

		/* Displacement mask: white moves top-right, black moves bottom-left. */
		_DisplacementTex ("Displacement", 2D) = "white" {}

		/* How much Texture is displaced by. */
		_DisplacementFactor ("Factor", Float) = 0.5
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			/**
			 *  A sampler2D may have a few accompanying objects:
			 *
			 *  - <var>_ST: the texture's tiling (.xy) and offset (.zw)
			 *      - <var>_ST.x: X tiling value
			 *      - <var>_ST.y: Y tiling value
			 *      - <var>_ST.z: X offset value
			 *      - <var>_ST.w: Y offset value
			 *  - <var>_TexelSize: the texture's normalized size (.xy) and real size (.zw)
			 *      - <var>_TexelSize.x: 1.0 / width
			 *      - <var>_TexelSize.y: 1.0 / height
			 *      - <var>_TexelSize.z: width
			 *      - <var>_TexelSize.w: height
			 */

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DisplacementTex;
			float4 _DisplacementTex_ST;
			float _DisplacementFactor;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				/* Assume displacement is a BW texture and extract the gray value. */
				float2 displacement_position = TRANSFORM_TEX(i.uv, _DisplacementTex);
				float gray = tex2D(_DisplacementTex, displacement_position).x;

				/* Calculate the displacement factor:
				 *   - 1.0 (white): Source the pixel from the a upper-right position
				 *   - 0.5 (gray): Source the pixel from its original position
				 *   - 0.0 (black): Source the pixel from the a lower-left position
				 */
				float factor = _DisplacementFactor * (gray - 0.5) * 2.0;
				float2 displaced_offset = float2(factor, factor);

				/* Get the actual pixel from the texture,
				 * displacing it by the factor in _DisplacementTex.
				 */
				_MainTex_ST.zw += displaced_offset;
				float2 main_position = TRANSFORM_TEX(i.uv, _MainTex);
				fixed4 color = tex2D(_MainTex, main_position);
				return color;
			}
			ENDCG
		}
	}
}
