Shader "Unlit/Voxel"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct VoxelData
			{
				int id;
				bool active;
				bool isHit;
				float3 position;
				float3 rotation;
				float3 scale;
			};


			int _IdOffset;
			float3 _Scale;

		
			StructuredBuffer<VoxelData> _VoxelData;



			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
			};

			inline int getId(float2 uv1)
			{
				return (int)(uv1.x + 0.5) + _IdOffset;
			}

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float2 uv1 : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float3 rotate(float3 p, float3 rotation)
			{
				float3 a = normalize(rotation);
				float angle = length(rotation);
				if (abs(angle) < 0.001) return p;
				float s = sin(angle);
				float c = cos(angle);
				float r = 1.0 - c;
				float3x3 m = float3x3(
					a.x * a.x * r + c,
					a.y * a.x * r + a.z * s,
					a.z * a.x * r - a.y * s,
					a.x * a.y * r - a.z * s,
					a.y * a.y * r + c,
					a.z * a.y * r + a.x * s,
					a.x * a.z * r + a.y * s,
					a.y * a.z * r - a.x * s,
					a.z * a.z * r + c
					);
				return mul(m, p);
			}

			v2f vert (appdata v)
			{

				VoxelData p = _VoxelData[getId(v.uv1)];
				v.vertex.xyz *= p.scale;
				v.vertex.xyz = rotate(v.vertex.xyz, p.rotation);
				v.vertex.xyz += p.position;
				//v.normal = rotate(v.normal, p.rotation);

				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
