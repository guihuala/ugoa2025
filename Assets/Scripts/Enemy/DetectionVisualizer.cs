using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class DetectionVisualizer : MonoBehaviour
{
    [Header("侦查范围参数")]
    public float detectionRadius = 5f;    // 侦查范围半径
    public float detectionAngle = 90f;   // 侦查范围角度
    public Material visualizerMaterial;  // 半透明材质

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        // 设置材质
        if (visualizerMaterial != null)
            meshRenderer.material = visualizerMaterial;

        // 创建侦查范围
        CreateDetectionMesh();
    }

    void CreateDetectionMesh()
    {
        Mesh mesh = new Mesh();

        // 顶点数量
        int segments = 50; // 分段数，越高越精细
        int vertexCount = segments + 2;

        // 初始化顶点和三角形
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[segments * 3];

        // 中心点
        vertices[0] = Vector3.zero;

        // 生成扇形的顶点
        float angleStep = detectionAngle / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = -detectionAngle / 2 + angleStep * i; // 从 -angle/2 开始
            float radian = Mathf.Deg2Rad * angle;

            vertices[i + 1] = new Vector3(
                Mathf.Sin(radian) * detectionRadius,
                0,
                Mathf.Cos(radian) * detectionRadius
            );
        }

        // 生成三角形
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;          // 中心点
            triangles[i * 3 + 1] = i + 1; // 当前顶点
            triangles[i * 3 + 2] = i + 2; // 下一个顶点
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
    }
}
