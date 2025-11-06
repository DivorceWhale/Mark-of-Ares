using UnityEngine;

public class GhostSpawner : MonoBehaviour
{
    [Header("Ghost Settings")]
    public GameObject ghostPrefab;  // Assign your ghost prefab here
    public int ghostCount = 3;      // How many ghosts to spawn
    public float spawnDelay = 2f;   // Time between spawns

    private BoxCollider area;

    void Start()
    {
        area = GetComponent<BoxCollider>();
        StartCoroutine(SpawnGhosts());
    }

    System.Collections.IEnumerator SpawnGhosts()
    {
        for (int i = 0; i < ghostCount; i++)
        {
            Vector3 spawnPos = GetRandomPointInArea();
            Instantiate(ghostPrefab, spawnPos, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    Vector3 GetRandomPointInArea()
    {
        Vector3 center = area.center + transform.position;
        Vector3 size = area.size;

        float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
        float y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);
        float z = Random.Range(center.z - size.z / 2, center.z + size.z / 2);

        return new Vector3(x, y, z);
    }
}
