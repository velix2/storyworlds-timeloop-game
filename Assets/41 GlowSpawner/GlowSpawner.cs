using UnityEngine;

public class GlowSpawner : MonoBehaviour
{
    public static GlowSpawner Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Found more than one GlowSpawner in the scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] ParticleSystem glowEffect;
    
    public void SpawnGlowOnPlayer()
    {
        GameObject glowPoint = GameObject.FindWithTag("GlowPoint");
        if(glowPoint == null)
        {
            Debug.LogError("There is no Object with GlowPoint-Tag on the scene");
            return;
        }

        Instantiate(glowEffect, glowPoint.transform);

    }
}
