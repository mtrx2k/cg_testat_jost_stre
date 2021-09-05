using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticiesScript : MonoBehaviour
{
    //Wird benötigt, damit Chunk nicht doppelt geladen wird
    private bool Created;

    //Anzahl der Verticies pro Kante
    public int Divisions;

    //Größe X/Y
    public float SizeX;
    public float SizeY;

    //Verticies
    Vector3[] Verts;
    //Anzahl der Verticies
    int vertCount;

    // Start is called before the first frame update
    void Start()
    {
        if (!Created)
            Create();
    }

    //Erstellt eine Karte anhand des DS-Algorithmus
    public void Create()
    {
        vertCount = (Divisions + 1) * (Divisions + 1);
        Verts = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] tris = new int[Divisions * Divisions * 6];

        float halfSizeX = SizeX * 0.5f;
        float halfSizeY = SizeY * 0.5f;
        float divisionSizeX = SizeX / Divisions;
        float divisionSizeY = SizeY / Divisions;

        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int triOffs = 0;

        //Erstellt Verticies und Triangles über gesamtes Square
        for (int i = 0; i <= Divisions; i++)
        {
            for (int j = 0; j <= Divisions; j++)
            {
                Verts[i * (Divisions + 1) + j] = new Vector3(-halfSizeX + j * divisionSizeX, 0.0f, halfSizeY - i * divisionSizeY);
                uvs[i * (Divisions + 1) + j] = new Vector2((float)i / Divisions, (float)j / Divisions);


                if (i < Divisions && j < Divisions)
                {
                    int topL = i * (Divisions + 1) + j;
                    int botL = (i + 1) * (Divisions + 1) + j;


                    tris[triOffs] = topL;
                    tris[triOffs + 1] = topL + 1;
                    tris[triOffs + 2] = botL + 1;


                    tris[triOffs + 3] = topL;
                    tris[triOffs + 4] = botL + 1;
                    tris[triOffs + 5] = botL;

                    triOffs += 6;
                }
            }
            Created = true;
        }

        //Setzt Verticies, UVS und Triangles
        mesh.vertices = Verts;
        mesh.uv = uvs;
        mesh.triangles = tris;

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshCollider>().enabled = true;

        //Neu berechnen
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

}
