Shader "Voxel/VoxelDraw"
{

SubShader 
{

Tags { "RenderType" = "Opaque" }

CGINCLUDE
#pragma enable_d3d11_debug_symbols
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

#ifdef SHADER_API_D3D11
StructuredBuffer<VoxelData> _VoxelData;
#endif
int _IdOffset;

struct appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv1 : TEXCOORD1;
};

struct v2f
{
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
    float2 uv1 : TEXCOORD1;
};


struct gbuffer_out
{
    float4 diffuse  : SV_Target0; // rgb: diffuse,  a: occlusion
    float4 specular : SV_Target1; // rgb: specular, a: smoothness
    float4 normal   : SV_Target2; // rgb: normal,   a: unused
    float4 emission : SV_Target3; // rgb: emission, a: unused
    float  depth    : SV_Depth;
};

inline int getId(float2 uv1)
{
    //return (int)(uv1.x + 0.5) + _IdOffset;
	return (int)(uv1.x + 0.5);
}

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

v2f vert(appdata v)
{
#ifdef SHADER_API_D3D11
    VoxelData p = _VoxelData[getId(v.uv1)];
    v.vertex.xyz *= p.scale;
    v.vertex.xyz = rotate(v.vertex.xyz, p.rotation);
    v.vertex.xyz += p.position;
	
    v.normal = rotate(v.normal, p.rotation);
#endif

	v.vertex.xyz= float3(0,200,0);
    v2f o;
    o.uv1 = v.uv1;
    o.position = mul(UNITY_MATRIX_VP, v.vertex);
    o.normal = v.normal;
    return o;
}

gbuffer_out frag(v2f i) : SV_Target
{
    VoxelData p;
#ifdef SHADER_API_D3D11
    p = _VoxelData[getId(i.uv1)];
#endif
    //float3 v = p.velocity;

    gbuffer_out o;
    //o.diffuse = float4(v.y * 0.5, (abs(v.x) + abs(v.z)) * 0.1, -v.y * 0.5, 0);
	o.diffuse = float4(1,1,1,1);
    o.normal = float4(i.normal, 1);
    o.emission = o.diffuse * 0.1;
    o.specular = 0;
    o.depth = i.position;

    return o;
}


ENDCG

Pass
{
    Tags { "LightMode" = "Deferred" }
    ZWrite On

    CGPROGRAM
    #pragma target 3.0
    #pragma vertex vert 
    #pragma fragment frag 
    ENDCG
}

} 

FallBack "Diffuse"

}