using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public int WorldSizeInChunks = 10;

    public GameObject LoadingScreen;

    Dictionary<Vector3Int , Chunk> chunks = new Dictionary<Vector3Int, Chunk>();
    void Start()
    {
        Generate();
    }

    void Generate()
    {   
        LoadingScreen.SetActive(true);

        for( int x = 0; x < WorldSizeInChunks; x++ )
        {
            for( int z = 0; z < WorldSizeInChunks; z++)
            {
                Vector3Int chunkPos = new Vector3Int(x * GameData.ChunkWidth , 0, z * GameData.ChunkWidth);
                chunks.Add(chunkPos, new Chunk(chunkPos));
                chunks[chunkPos].chunkobject.transform.SetParent(transform);
            }
        }

        LoadingScreen.SetActive(false);
        Debug.Log(string.Format("{0} * {0} World Generated", WorldSizeInChunks* GameData.ChunkWidth));

    }

}
