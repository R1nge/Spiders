using UnityEngine;

public class Entrypoint : MonoBehaviour
{
    [SerializeField] private int randomIndex = 12345;
    [SerializeField] private int spidersCount = 8500;

    public Unity.Mathematics.Random Random { get; private set; }

    public int RandomIndex => randomIndex;

    public int SpidersCount => spidersCount;
    public static Entrypoint Instance { get; private set; }

    private void Awake()
    {
        Random = Unity.Mathematics.Random.CreateFromIndex((uint)randomIndex);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}