using System.Collections;
using UnityEngine;

public class GraveGhostActivator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem graveParticles;
    [SerializeField] private Enemy_Ghost ghostPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Activation")]
    [SerializeField] private float ghostDelay = 0.35f;

    private BoxCollider2D triggerCollider;
    private bool activated;

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated || collision.GetComponent<Player>() == null)
            return;

        activated = true;
        triggerCollider.enabled = false;

        if(graveParticles != null)
        {
            graveParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        StartCoroutine(ActivateGhost());
    }

    private IEnumerator ActivateGhost()
    {
        if(ghostDelay > 0)
            yield return new WaitForSeconds(ghostDelay);

        if (ghostPrefab != null)
        {
            Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
            Instantiate(ghostPrefab, position, Quaternion.identity);
        }
    }
}
