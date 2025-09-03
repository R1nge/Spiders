using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject[] spiders;

    private void Start()
    {
        for (int i = 0; i < Entrypoint.Instance.SpidersCount; i++)
        {
            var spiderIndex = Random.Range(0, spiders.Length);
            var rotation = Quaternion.identity;
            Instantiate(spiders[spiderIndex], Vector3.zero, rotation, null);
        }
    }
}