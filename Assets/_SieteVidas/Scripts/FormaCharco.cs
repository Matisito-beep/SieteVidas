using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class FormaCharco : MonoBehaviour
{
    public int semilla = 1;
    [Range(12, 128)] public int puntosBorde = 64;
    
    [Range(0f, 0.6f)] public float irregularidad = 0.3f;
  
    [Range(1f, 6f)] public float frecuencia = 2.5f;

    private Mesh malla;

    void OnEnable()
    {
        malla = new Mesh { name = "FormaCharco", hideFlags = HideFlags.DontSave };
        Construir();
        GetComponent<MeshFilter>().sharedMesh = malla;
    }

    void OnDisable()
    {
        if (malla != null) DestroyImmediate(malla);
    }

    void OnValidate()
    {
        
        if (malla != null) Construir();
    }

    private void Construir()
    {
        System.Random azar = new System.Random(semilla);
        float desfaseX = (float)azar.NextDouble() * 100f;
        float desfaseY = (float)azar.NextDouble() * 100f;

        Vector3[] vertices = new Vector3[puntosBorde + 1];
        Vector2[] uvs = new Vector2[puntosBorde + 1];
        int[] triangulos = new int[puntosBorde * 3];

        // Punto central
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);

        // Puntos del borde
        for (int i = 0; i < puntosBorde; i++)
        {
            float angulo = (float)i / puntosBorde * Mathf.PI * 2f;
            float coseno = Mathf.Cos(angulo);
            float seno = Mathf.Sin(angulo);

            float ruido = Mathf.PerlinNoise(coseno * frecuencia + desfaseX, seno * frecuencia + desfaseY);
            float radio = 0.5f * (1f + (ruido - 0.5f) * 2f * irregularidad);

            vertices[i + 1] = new Vector3(coseno * radio, 0f, seno * radio);
            uvs[i + 1] = new Vector2(0.5f + coseno * radio, 0.5f + seno * radio);
        }

        // Triangulos en abanico desde el centro, mirando hacia arriba
        for (int i = 0; i < puntosBorde; i++)
        {
            int actual = i + 1;
            int siguiente = (i + 1) % puntosBorde + 1;
            triangulos[i * 3] = 0;
            triangulos[i * 3 + 1] = siguiente;
            triangulos[i * 3 + 2] = actual;
        }

        malla.Clear();
        malla.vertices = vertices;
        malla.uv = uvs;
        malla.triangles = triangulos;
        malla.RecalculateNormals();
        malla.RecalculateBounds();
    }
}