using UnityEngine;

public class ParallaxZoneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private ParallaxZoneTransitionManager zoneManager;
    [SerializeField] private string targetZoneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player == null)
            return;

        zoneManager.SwitchToZone(targetZoneName,player);
    }
}
