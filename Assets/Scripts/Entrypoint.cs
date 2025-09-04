using Unity.Mathematics;
using UnityEngine;

public class Entrypoint : MonoBehaviour
{
    [SerializeField] private int randomIndex = 12345;
    [SerializeField] private int spidersCount = 8500;

    public Unity.Mathematics.Random Random { get; private set; }
    public int SpidersCount => spidersCount;
    public static Entrypoint Instance { get; private set; }

    private void Awake()
    {
        Random = new Unity.Mathematics.Random((uint)math.sqrt(UnityEngine.Random.value * (uint.MaxValue / 4)));
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void PickNewRandom()
    {
        var seed = (uint)math.sqrt(UnityEngine.Random.value * (uint.MaxValue / 4));

        if (seed == 0)
        {
            PickNewRandom();
            return;
        }

        Random = new Unity.Mathematics.Random(seed);
    }
}