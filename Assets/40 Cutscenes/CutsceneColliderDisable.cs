using UnityEngine;

public class CutsceneColliderDisable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Collider[] colliders;
    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true);
    }
    private void OnCutsceneStart()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void OnCutsceneEnd()
    {
        foreach (Collider collider in colliders)
        {
            collider.enabled = true;
        }
    }
    
    private void OnEnable()
    {
        CutsceneManager.CutsceneStarted += OnCutsceneStart;
        CutsceneManager.CutsceneEnded += OnCutsceneEnd;
    }

    private void OnDisable()
    {
        CutsceneManager.CutsceneStarted -= OnCutsceneStart;
        CutsceneManager.CutsceneEnded -= OnCutsceneEnd;
    }
}
