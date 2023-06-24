using UnityEngine;

public class SpawnerJob : MonoBehaviour
{
    [SerializeField] private int spawnAmount;
    [SerializeField] private SpiderMove[] spiders;

    private void Awake()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            var spiderIndex = Random.Range(0, spiders.Length);
            var rotation = Quaternion.identity;
            Instantiate(spiders[spiderIndex], Vector3.zero, rotation, null);
        }
    }
}