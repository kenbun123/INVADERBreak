using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using mattatz.VoxelSystem;

[System.Serializable]
public struct GPUBallData
{
    public Vector3 position;
    public float radius;
    public bool isHit;
}

[RequireComponent(typeof(MeshFilter))]
public class VoxelTest : MonoBehaviour {
    public struct VoxelData
    {
        public int id;
        public bool active;
        public bool isHit;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

    };



    const int MAX_VERTEX_NUM = 65534;

    [Header("ボクセル密度")]
    public int VoxelDensity;

    public MeshFilter m_CubeMesh;   //キューブメッシュデータ

    public Mesh m_Mesh;                    //メッシュデータ

    public Vector3 m_BoxScale = new Vector3(1.0f,1.0f,1.0f);

    [SerializeField]
    List<Vector3> m_positions;      //ボクセル座標

    [SerializeField]
    ComputeShader m_computeShader;

    public ComputeShader m_ComputeShade2;

    ComputeBuffer m_computeBuffer;
    public ComputeBuffer m_BallDataCBuffer;
    int m_hitKernel;
    int m_UpdateKernel;

    [SerializeField]
    Shader m_shader;

    int m_boxNumPerMesh;
    int m_meshNum;

    [SerializeField]
    List<Material> materials_ = new List<Material>();

    public GameObject ball;
    public List<GPUBallData> m_ballList = new List<GPUBallData>();
    Mesh m_aferMesh;    //合成後のメッシュ
    public int bytesize = 0;
    // Use this for initialization
    void Start()
    {
        bytesize = Marshal.SizeOf(typeof(VoxelData));
        //m_mesh = GetComponent<MeshFilter>().mesh;

        m_positions = GetMeshVoxelData(m_Mesh, VoxelDensity);


        if (m_positions == null)
        {
            Debug.Log("メッシュファイルがない");
            return;
        }
        m_boxNumPerMesh = MAX_VERTEX_NUM / m_CubeMesh.mesh.vertexCount;
        m_meshNum = (int)Mathf.Ceil((float)m_positions.Count / m_boxNumPerMesh);
        m_aferMesh = CreateCombinedMesh(m_CubeMesh.mesh, m_boxNumPerMesh);

        for (int i = 0; i < m_meshNum; ++i)
        {
            var material = new Material(m_shader);
            material.SetInt("_IdOffset", m_boxNumPerMesh * i);
            materials_.Add(material);
        }

        m_computeBuffer = new ComputeBuffer(m_positions.Count, Marshal.SizeOf(typeof(VoxelData)), ComputeBufferType.Default);

        //座標をセット
        VoxelData[] voxeldata = new VoxelData[m_positions.Count];
        for (int i = 0; i < m_positions.Count; i++)
        {

            voxeldata[i].position = m_positions[i];
            voxeldata[i].rotation = new Vector3(0,0,0);
        }
        m_computeBuffer.SetData(voxeldata);

        var m_initKernel = m_computeShader.FindKernel("Init");
        m_computeShader.SetBuffer(m_initKernel, "_VoxelData", m_computeBuffer);
        m_computeShader.SetVector("_Scale",m_BoxScale);
        m_computeShader.Dispatch(m_initKernel, m_positions.Count / 8, 1, 1);
        m_computeBuffer.GetData(voxeldata);
        //当たり判定用バッファ
        m_BallDataCBuffer = new ComputeBuffer(8, Marshal.SizeOf(typeof(GPUBallData)), ComputeBufferType.Default);
       

        //m_hitKernel = m_computeShader.FindKernel("Hit");
    }

    void OnApplicationQuit()
    {
        m_computeBuffer.Release();
    }

    private void Update()
    {



        //メッシュの描画
        for (int i = 0; i < m_meshNum; ++i)
        {
            var material = materials_[i];
            material.SetInt("_IdOffset", m_boxNumPerMesh * i);
            material.SetBuffer("_VoxelData", m_computeBuffer);
            Graphics.DrawMesh(m_aferMesh, transform.position, transform.rotation, material, 0);
        }

    }


    /// <summary>
    /// 生成したいメッシュ分の数を一つのメッシュとして合成する
    /// </summary>
    /// <param name="_mesh">生成したいメッシュ</param>
    /// <param name="num">数</param>
    /// <returns></returns>
    Mesh CreateCombinedMesh(Mesh _mesh, int num)
    {
        Assert.IsTrue(_mesh.vertexCount * num <= MAX_VERTEX_NUM);

        var meshIndices = _mesh.GetIndices(0);
        var indexNum = meshIndices.Length;

        var vertices = new List<Vector3>();
        var indices = new int[num * indexNum];
        var normals = new List<Vector3>();
        var tangents = new List<Vector4>();
        var uv0 = new List<Vector2>();
        var uv1 = new List<Vector2>();

        
        for (int id = 0; id < num; ++id)
        {
            vertices.AddRange(_mesh.vertices);
            normals.AddRange(_mesh.normals);
            tangents.AddRange(_mesh.tangents);
            uv0.AddRange(_mesh.uv);

            // 各メッシュのインデックスは（1 つのモデルの頂点数 * ID）分ずらす
            for (int n = 0; n < indexNum; ++n)
            {
                indices[id * indexNum + n] = id * _mesh.vertexCount + meshIndices[n];
            }

            // 2 番目の UV に ID を格納しておく
            for (int n = 0; n < _mesh.uv.Length; ++n)
            {
                uv1.Add(new Vector2(id, id));
            }
        }

        var combinedMesh = new Mesh();
        combinedMesh.SetVertices(vertices);
        combinedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
        combinedMesh.SetNormals(normals);
        combinedMesh.RecalculateNormals();
        combinedMesh.SetTangents(tangents);
        combinedMesh.SetUVs(0, uv0);
        combinedMesh.SetUVs(1, uv1);
        combinedMesh.RecalculateBounds();
        combinedMesh.bounds.SetMinMax(Vector3.one * -100f, Vector3.one * 100f);

        return combinedMesh;
    }

    /// <summary>
    /// ボクセル座標
    /// </summary>
    /// <param name="_mesh">メッシュデータ</param>
    /// <param name="count">密度</param>
    /// <returns></returns>
    List<Vector3> GetMeshVoxelData(Mesh _mesh,int count)
    {
        List<Vector3> result = new List<Vector3>();
        List<Voxel> voxels;
        voxels = Voxelizer.Voxelize(_mesh, count);

        foreach (var voxel in voxels)
        {
            result.Add(voxel.position);
        }

        return result;
    }

}
