using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[ExecuteInEditMode]
public class PaperArtGenerator : MonoBehaviour
{
    public string assetSavePath_Quad = "Assets/PaperObjs/MeshAndMat/Quads";
    public string assetSavePath_Cube = "Assets/PaperObjs/MeshAndMat/Cubes";
    public Material defaultMaterial; // 共享的材质

    [Header("Quad Generator")]
    public Texture2D planeTexture; // 自定义贴图数组
    public float scaleOffset_Plane;

    [Header("Box Generator")]
    public Texture2D cubeTexture;
    public float scaleOffset_Cube;
    public Vector3 cubeScale;

    [ContextMenu("Init Paper GameObject")]
    public void InitPlane()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        vertices[0] = new Vector3(0, 0);
        vertices[1] = new Vector3(0, planeTexture.height/ scaleOffset_Plane);
        vertices[2] = new Vector3(planeTexture.width/ scaleOffset_Plane, planeTexture.height/ scaleOffset_Plane);
        vertices[3] = new Vector3(planeTexture.width/ scaleOffset_Plane, 0);

        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(0, 1);
        uv[2] = new Vector2(1, 1);
        uv[3] = new Vector2(1, 0);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        string gameObjectName = planeTexture.name;
        GameObject newPlane = new GameObject(gameObjectName, typeof(MeshFilter), typeof(MeshRenderer));
        newPlane.GetComponent<MeshFilter>().mesh = mesh;
        Material material = InitNewMaterial(planeTexture);
        newPlane.GetComponent<MeshRenderer>().material = material;

        string meshName = planeTexture.name + ".asset";
        string materialName = planeTexture.name + ".mat";
        // 保存Mesh到资源库
        AssetDatabase.CreateAsset(mesh, $"{assetSavePath_Quad}/{meshName}");
        AssetDatabase.SaveAssets();
        // 保存Material到资源库
        AssetDatabase.CreateAsset(material, $"{assetSavePath_Quad}/{materialName}");
        AssetDatabase.SaveAssets();
    }

    public Material InitNewMaterial(Texture2D tempTexture)
    {
        Material materialInstance = new Material(defaultMaterial);
        materialInstance.SetTexture("_BaseMap", tempTexture);
        return materialInstance;
    }

    [ContextMenu("Init Cube GameObject")]
    public void InitCube()
    {
        float cx = cubeScale.x / scaleOffset_Cube;
        float cy = cubeScale.y / scaleOffset_Cube;
        float cz = cubeScale.z / scaleOffset_Cube;
        Mesh cubeMesh = new Mesh();
        Vector3[] vertices = GenerateVertices();
        Vector2[] uv = GenerateUV();
        int[] triangles = GenerateTriangles();
        Vector3[] normals = GenerateNormals(); 

        cubeMesh.vertices = vertices;
        cubeMesh.uv = uv;
        cubeMesh.triangles = triangles;
        cubeMesh.normals = normals;
        cubeMesh.RecalculateBounds();

        string gameObjectName = cubeTexture.name;
        GameObject newCube = new GameObject(gameObjectName, typeof(MeshFilter), typeof(MeshRenderer));
        newCube.GetComponent<MeshFilter>().mesh = cubeMesh;
        Material material = InitNewMaterial(cubeTexture);
        newCube.GetComponent<MeshRenderer>().material = material;

        string meshName = cubeTexture.name + ".asset";
        string materialName = cubeTexture.name + ".mat";
        // 保存Mesh到资源库
        AssetDatabase.CreateAsset(cubeMesh, $"{assetSavePath_Cube}/{meshName}");
        AssetDatabase.SaveAssets();
        // 保存Material到资源库
        AssetDatabase.CreateAsset(material, $"{assetSavePath_Cube}/{materialName}");
        AssetDatabase.SaveAssets();

        Vector3[] GenerateVertices()
        {
            Vector3[] vertices = new Vector3[24];
            var xyz = new Dictionary<int, float> { { 0, cx }, { 1, cy }, { 2, cz } };
            for (int i = 0, v = 0; i < 3; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    float[] tempV3 = new float[3];
                    int[] vectorIndex = null;
                    switch (i)
                    {
                        case 0: vectorIndex = new int[2] { 1, 2 }; break;
                        case 1: vectorIndex = new int[2] { 0, 2 }; break;
                        case 2: vectorIndex = new int[2] { 0, 1 }; break;
                    }
                    vertices[v] = ReturnVertex(true, true);
                    vertices[v + 1] = (i != 1) ? (j == 0) ? ReturnVertex(false, true) : ReturnVertex(true, false) : (j == 0) ? ReturnVertex(true, false): ReturnVertex(false, true);
                    vertices[v + 2] = ReturnVertex(false, false);
                    vertices[v + 3] = (i != 1) ? (j == 0) ? ReturnVertex(true, false) : ReturnVertex(false, true) : (j == 0) ? ReturnVertex(false, true) : ReturnVertex(true, false);
                    v += 4;
                    Vector3 ReturnVertex(bool pnFir, bool pnSec)
                    {
                        tempV3[vectorIndex[0]] = (pnFir) ? xyz[vectorIndex[0]] : -xyz[vectorIndex[0]];
                        tempV3[vectorIndex[1]] = (pnSec) ? xyz[vectorIndex[1]] : -xyz[vectorIndex[1]];
                        tempV3[i] = (j == 0) ? xyz[i] : -xyz[i];
                        return new Vector3(tempV3[0], tempV3[1], tempV3[2]);
                    }
                }
            }

            //foreach (var item in vertices) Debug.Log(item);

            return vertices;
        }

        Vector2[] GenerateUV()
        {
            float horiLength = cubeScale.z * 2 + cubeScale.x * 2;
            float verLength = cubeScale.z * 2 + cubeScale.y;
            float horiUnit = 1 / horiLength;
            float verUnit = 1 / verLength;

            return new Vector2[]
            {
                //Right
                new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
                new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
                //Left
                new Vector2(0,(cubeScale.z+cubeScale.y)*verUnit),
                new Vector2(cubeScale.z*horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
                new Vector2(cubeScale.z* horiUnit,cubeScale.z*verUnit),
                new Vector2(0,cubeScale.z*verUnit),
                //Top
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,1),
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)* verUnit),
                new Vector2(cubeScale.z * horiUnit,(cubeScale.z+cubeScale.y) * verUnit),
                new Vector2(cubeScale.z* horiUnit,1),
                //Bottom
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,0),
                new Vector2(cubeScale.z * horiUnit,0),
                new Vector2(cubeScale.z* horiUnit,cubeScale.z*verUnit),
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
                //Back
                new Vector2(1,(cubeScale.z+cubeScale.y)*verUnit),
                new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
                new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
                new Vector2(1,cubeScale.z*verUnit),
                //Front
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
                new Vector2((cubeScale.z+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
                new Vector2(cubeScale.z* horiUnit,cubeScale.z*verUnit),
                new Vector2(cubeScale.z* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
            };
        }

        int[] GenerateTriangles()
        {
            int[] tempTriVertex = new int[36];
            for (int i = 0, j = 0; i < 36; i+=6)
            {
                tempTriVertex[i] = j;
                tempTriVertex[i + 1] = j + 1;
                tempTriVertex[i + 2] = j + 2;
                tempTriVertex[i + 3] = j ;
                tempTriVertex[i + 4] = j + 2;
                tempTriVertex[i + 5] = j + 3;
                j += 4;
            }
            return tempTriVertex;
        }

        Vector3[] GenerateNormals()
        {
            Vector3[] tempNormal = new Vector3[24];
            GetNormalDirection(0, Vector3.right);
            GetNormalDirection(4, Vector3.left);
            GetNormalDirection(8, Vector3.up);
            GetNormalDirection(12, Vector3.down);
            GetNormalDirection(16, Vector3.forward);
            GetNormalDirection(20, Vector3.back);

            void GetNormalDirection(int starterIndex, Vector3 td)
            {
                tempNormal[starterIndex] = td;
                tempNormal[starterIndex + 1] = td;
                tempNormal[starterIndex + 2] = td;
                tempNormal[starterIndex + 3] = td;
            }
            return tempNormal;
        }
    }


    //[ContextMenu("Init Cube GameObject")]
    //public void InitCube()
    //{
    //    Mesh cubeMesh = new Mesh();
    //    Vector3[] vertices = GenerateVertices();
    //    Vector2[] uv = GenerateUV();
    //    int[] triangles = GenerateTriangles();

    //    cubeMesh.vertices = vertices;
    //    cubeMesh.uv = uv;
    //    cubeMesh.triangles = triangles;
    //    cubeMesh.RecalculateNormals();
    //    cubeMesh.RecalculateBounds();
    //    //cubeMesh.nor

    //    string gameObjectName = cubeTexture.name;
    //    GameObject newCube = new GameObject(gameObjectName, typeof(MeshFilter), typeof(MeshRenderer));
    //    newCube.GetComponent<MeshFilter>().mesh = cubeMesh;
    //    newCube.GetComponent<MeshRenderer>().material = InitNewMaterial(cubeTexture);

    //    Vector3[] GenerateVertices()
    //    {
    //        return new Vector3[]
    //        {
    //            //Bottom
    //            new Vector3(-1,0,1),   //0
    //            new Vector3(1,0,1),    //1
    //            new Vector3(1,0,-1),   //2
    //            new Vector3(-1,0,-1),  //3
    //            //Top                
    //            new Vector3(-1,2,1),   //4
    //            new Vector3(1,2,1),    //5
    //            new Vector3(1,2,-1),   //6
    //            new Vector3(-1,2,-1),  //7
    //            //Left               
    //            new Vector3(-1,0,1),   //8
    //            new Vector3(-1,0,-1),  //9
    //            new Vector3(-1,2,-1),  //10
    //            new Vector3(-1,2,1),   //11
    //            //Right               
    //            new Vector3(1,0,1),    //12
    //            new Vector3(1,0,-1),   //13
    //            new Vector3(1,2,-1),   //14
    //            new Vector3(1,2,1),    //15
    //            //Front               
    //            new Vector3(1,0,-1),   //16
    //            new Vector3(-1,0,-1),  //17
    //            new Vector3(-1,2,-1),  //18
    //            new Vector3(1,2,-1),   //19 
    //            //Back              
    //            new Vector3(1,0,1),    //20
    //            new Vector3(-1,0,1),   //21
    //            new Vector3(-1,2,1),   //22
    //            new Vector3(1,2,1),    //23
    //        };
    //    }

    //    Vector2[] GenerateUV()
    //    {
    //        float horiLength = cubeScale.z * 2 + cubeScale.x * 2;
    //        float verLength = cubeScale.z * 2 + cubeScale.y;
    //        float horiUnit = 1 / horiLength;
    //        float verUnit = 1 / verLength;

    //        return new Vector2[]
    //        {
    //        //Bottom
    //        new Vector2(cubeScale.z * horiUnit,(cubeScale.z+cubeScale.y) * verUnit),
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)* verUnit),
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,1),
    //        new Vector2(cubeScale.z* horiUnit,1),
    //        //Top                
    //        new Vector2(cubeScale.z * horiUnit,0),
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,0),
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
    //        new Vector2(cubeScale.z* horiUnit,cubeScale.z*verUnit),
    //        //Left               
    //        new Vector2(0,(cubeScale.z+cubeScale.y)*verUnit),
    //        new Vector2(cubeScale.z*horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
    //        new Vector2(cubeScale.z* horiUnit,cubeScale.z*verUnit),
    //        new Vector2(cubeScale.z*horiUnit,0),
    //        //Right               
    //        new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
    //        new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
    //        //Front               
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
    //        new Vector2(cubeScale.z* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
    //        new Vector2(cubeScale.z* horiUnit,cubeScale.z*verUnit),
    //        new Vector2((cubeScale.z+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
    //        //Back              
    //           new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,(cubeScale.z+cubeScale.y)*verUnit),
    //      new Vector2(1,(cubeScale.z+cubeScale.y)*verUnit),
    //        new Vector2(1,cubeScale.z*verUnit),
    //         new Vector2((cubeScale.z*2+cubeScale.x)* horiUnit,cubeScale.z*verUnit),
    //        };
    //    }

    //    int[] GenerateTriangles()
    //    {
    //        return new int[]
    //        {
    //            //Bottom/Top
    //            0,1,2,
    //            0,2,3,
    //            4,5,6,
    //            4,6,7,
    //            //Left/Right
    //            8,9,10,
    //            8,10,11,
    //            12,13,14,
    //            12,14,15,
    //            //Bottom/Top
    //            16,17,18,
    //            16,18,19,
    //            20,21,22,
    //            20,22,23,
    //        };
    //    }
    //}

}
