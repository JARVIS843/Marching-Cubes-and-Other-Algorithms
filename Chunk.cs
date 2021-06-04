using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{

    MeshFilter meshFilter;
    public GameObject chunkobject;
    MeshRenderer meshRenderer;

    Vector3Int chunkPosition;

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();


    float[ , , ] TerrainMap;

    int width {get {return GameData.ChunkWidth; }}

    int height {get {return GameData.ChunkHeight; }}

    float isoline {get {return GameData.isoline; }}

    // Start is called before the first frame update

    MeshCollider meshCollider;
    public Chunk(Vector3Int _position)
    {
        
        chunkobject = new GameObject();
        chunkPosition = _position;
        chunkobject.transform.position = _position;
        chunkobject.name = string.Format("Chunk {0} , {1}", _position.x, _position.z);
        meshFilter = chunkobject.AddComponent<MeshFilter>();
        TerrainMap = new float[width+1, height+1,width+1];
        meshCollider = chunkobject.AddComponent<MeshCollider>();
        meshRenderer = chunkobject.AddComponent<MeshRenderer>();
        meshRenderer.material = Resources.Load<Material>("Materials/TerrainMaterial");

        chunkobject.transform.tag = "Terrain";
        GenerateTerrainMap();
        CreateTerrainData();
        BuildingMesh();
    }

    void GenerateTerrainMap()
    {
        for(int x = 0; x < width+1; x++)
        {
                for(int z =0; z <width+1; z++)
                {
                 for(int y = 0; y<height+1;y++)
                 {
                     float localHeight;
                    //float localHeight = 4.03f*Mathf.PerlinNoise(x*0.25f,z*0.25f)+ 
                    //1.96f*Mathf.PerlinNoise(x*0.5f,z*0.5f)+
                    //1.01f*Mathf.PerlinNoise(x,z);
                    localHeight = GameData.GetTerrainHeight(x + chunkPosition.x ,z + chunkPosition.z);

                    TerrainMap[x,y,z] = localHeight - y;
                }
            }
        }
    }


   public void PlaceTerrain(Vector3 pos)
    {
        Vector3Int rounded = new Vector3Int(Mathf.CeilToInt(pos.x),Mathf.CeilToInt(pos.y),Mathf.CeilToInt(pos.z));
        TerrainMap[rounded.x,rounded.y,rounded.z] = 0f;
        CreateTerrainData();
        BuildingMesh();
    }
   public  void RemoveTerrain(Vector3 pos)
    {
        Vector3Int rounded = new Vector3Int(Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.y),Mathf.FloorToInt(pos.z));
        TerrainMap[rounded.x,rounded.y,rounded.z] = 1f;
        CreateTerrainData();
        BuildingMesh();
    }



    //return the value of the Map at a specific point
    float SampleTerrain(Vector3Int vert)
    {   
        return TerrainMap[vert.x,vert.y,vert.z];
    }

    void CreateTerrainData()
    {
        ClearMesh();
        for(int x = 0 ;x < width;x++){
            for(int y = 0; y<height; y++)
            {
                for(int z = 0 ; z<width; z++)
                {
                    Vector3Int point = new Vector3Int(x,y,z);
                    MarchCube(point);
                }
            }
        }
    }
    void BuildingMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshCollider.sharedMesh = mesh;
        meshFilter.mesh = mesh;
    }

    void ClearMesh()
    {
        vertices.Clear();
        triangles.Clear();
    }

    void MarchCube(Vector3Int position)
    {
        //Loop through every corner of a cube

        float[] cube = new float[8];
        for(int n = 0; n <8; n++)
        {
            cube[n] = SampleTerrain(position + GameData.CornerTable[n]);
        }

        int CubeIndex = EvaluateCubeIndex(cube);

        if(CubeIndex == 0||CubeIndex == 255)
            return;

        for(int i = 0; GameData.triangulation[CubeIndex,i] !=-1; i++)
        {
            int edgeindex = GameData.triangulation[CubeIndex,i];

            Vector3Int a = position + GameData.CornerTable[GameData.EdgeIndexes[edgeindex,0]];
            Vector3Int b = position + GameData.CornerTable[GameData.EdgeIndexes[edgeindex,1]];


            Vector3 vert;

                float v1 = SampleTerrain(a);
                float v2 = SampleTerrain(b);
                vert = Interpolation(v1,v2,a,b);
            
  

                triangles.Add(CheckVertices(vert));
            
            
        }

    }

    int CheckVertices(Vector3 point)
    {
        for(int i = 0 ; i<vertices.Count;i++)
        {
            if(point == vertices[i])
                return i;
        }

        vertices.Add(point);
        return vertices.Count -1;
    }

    Vector3 Interpolation(float v1,float v2,Vector3 p1,Vector3 p2)
    {
        float fraction = (isoline - v1)/(v2 - v1);
        return p1 + fraction *(p2-p1);
    }

    int EvaluateCubeIndex(float[] cube)
    {
        int CubeIndex = 0;
        if(cube[0]<isoline) CubeIndex |=1;
        if(cube[1]<isoline) CubeIndex |=2;
        if(cube[2]<isoline) CubeIndex |=4;
        if(cube[3]<isoline) CubeIndex |=8;
        if(cube[4]<isoline) CubeIndex |=16;
        if(cube[5]<isoline) CubeIndex |=32;
        if(cube[6]<isoline) CubeIndex |=64;
        if(cube[7]<isoline) CubeIndex |=128;
        return CubeIndex;
    }

    

}
