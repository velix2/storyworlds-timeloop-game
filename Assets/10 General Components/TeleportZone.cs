using System;
using System.Collections;
using FadeToBlack;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportZone : MonoBehaviour
{
     [Serializable]
    public class Side
    {
        public Transform teleportPosition;
        public CollisionReporter reporter;
    }

    public Side A;
    public Side B;
    public GameObject player;

    private void Awake()
    {
        A.reporter.TriggerEnter.AddListener(teleportToB);
        B.reporter.TriggerEnter.AddListener(teleportToA);
    }

    private void teleportToA(Collider c)
    {
        if (!enabled) return;
        StartCoroutine(teleportTo(A.teleportPosition.position));
    }

    private void teleportToB(Collider c)
    {
        if (!enabled) return;
        StartCoroutine(teleportTo(B.teleportPosition.position));

    }

    private IEnumerator teleportTo(Vector3 newPos)
    {
        FadeToBlackPanel.Instance.FadeToBlack();
        yield return new WaitForSeconds(FadeToBlackPanel.Instance.FadeOutSeconds);
        
        Vector3 oldPos = player.transform.position;
        
        player.SetActive(false);
        player.transform.position = newPos;
        player.SetActive(true);

        CameraManager.mainVirtual.OnTargetObjectWarped(player.transform, newPos - oldPos);

    }
}
