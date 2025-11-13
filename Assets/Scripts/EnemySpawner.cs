using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject monstersRoot;
    public GameObject monsterPrefab;
    public Transform[] spawnPoints;

    public void Spawn()
    {
        if (monstersRoot != null)
        {
            monstersRoot.SetActive(true);
            return;
        }
        if (monsterPrefab != null && spawnPoints != null)
        {
            foreach (var t in spawnPoints)
                Instantiate(monsterPrefab, t.position, t.rotation);
        }
    }
}
