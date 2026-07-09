using UnityEngine;

public class ParallaxZoneGateTrigger : MonoBehaviour
{
    [SerializeField] private ParallaxZoneTransitionManager zoneManager;
    [SerializeField] private string leftZoneName;
    [SerializeField] private string rightZoneName;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player == null)
            return;

        string targetZone = player.transform.position.x >= transform.position.x
            ? rightZoneName : leftZoneName;

        zoneManager.SwitchToZone(targetZone,player);
    }
}
