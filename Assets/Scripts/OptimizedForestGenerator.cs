using UnityEngine;
using System.Collections.Generic;

public class OptimizedForestGenerator : MonoBehaviour
{
    [Header("Configuración del Bosque")]
    public int treeCount = 100; // Reducido para mejor rendimiento
    public float forestSize = 100f;
    public int seed = 12345;
    
    [Header("Optimización")]
    public bool useLOD = true; // Level of Detail
    public bool generateAtStart = true;
    
    private List<GameObject> trees = new List<GameObject>();
    private System.Random rng;
    
    void Start()
    {
        if (generateAtStart)
        {
            GenerateForest();
        }
    }
    
    public void GenerateForest()
    {
        ClearForest();
        rng = new System.Random(seed);
        
        // Generar árboles con distancia mínima para evitar solapamiento
        float minDistance = 3f;
        int maxAttempts = treeCount * 5;
        int attempts = 0;
        int treesGenerated = 0;
        
        while (treesGenerated < treeCount && attempts < maxAttempts)
        {
            Vector3 position = new Vector3(
                (float)(rng.NextDouble() * forestSize - forestSize/2),
                0f,
                (float)(rng.NextDouble() * forestSize - forestSize/2)
            );
            
            // Verificar distancia mínima
            bool tooClose = false;
            foreach (GameObject tree in trees)
            {
                if (Vector3.Distance(tree.transform.position, position) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }
            
            if (!tooClose)
            {
                CreateTree(position);
                treesGenerated++;
            }
            
            attempts++;
        }
        
        Debug.Log($"Bosque generado: {treesGenerated} árboles en {attempts} intentos");
    }
    
    void CreateTree(Vector3 position)
    {
        // Crear árbol simple con dos cilindros (tronco y copa)
        GameObject tree = new GameObject("Tree");
        tree.transform.position = position;
        tree.transform.SetParent(transform);
        
        // Tronco
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(tree.transform);
        trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
        trunk.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);
        trunk.GetComponent<Renderer>().material.color = new Color(0.4f, 0.2f, 0.1f);
        
        // Copa
        GameObject crown = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        crown.transform.SetParent(tree.transform);
        crown.transform.localPosition = new Vector3(0, 3.5f, 0);
        crown.transform.localScale = new Vector3(1.5f, 2f, 1.5f);
        crown.GetComponent<Renderer>().material.color = new Color(0.2f, 0.8f, 0.2f);
        
        // Desactivar colisiones para mejor rendimiento
        trunk.GetComponent<Collider>().enabled = false;
        crown.GetComponent<Collider>().enabled = false;
        
        trees.Add(tree);
    }
    
    public void ClearForest()
    {
        foreach (GameObject tree in trees)
        {
            if (tree != null)
                Destroy(tree);
        }
        trees.Clear();
    }
    
    // Método para obtener árboles cercanos (para IA)
    public List<GameObject> GetNearbyTrees(Vector3 position, float radius)
    {
        List<GameObject> nearbyTrees = new List<GameObject>();
        
        foreach (GameObject tree in trees)
        {
            if (tree != null && Vector3.Distance(tree.transform.position, position) <= radius)
            {
                nearbyTrees.Add(tree);
            }
        }
        
        return nearbyTrees;
    }
}