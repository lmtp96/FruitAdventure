using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxZoneGateTrigger : MonoBehaviour
{
    private enum SecondZoneDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    [SerializeField] private ParallaxZoneTransitionManager zoneManager;
    [SerializeField] private string firstZoneName;
    [SerializeField] private string secondZoneName;
    [SerializeField] private SecondZoneDirection secondZoneDirection;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player == null || zoneManager == null)
            return;

        Vector3 playerPosition = player.transform.position;
        Vector3 gatePosition = transform.position;

        bool isInSecondZone = secondZoneDirection switch
        {
            SecondZoneDirection.Right => playerPosition.x >= gatePosition.x,
            SecondZoneDirection.Left => playerPosition.x <= gatePosition.x,
            SecondZoneDirection.Up => playerPosition.y >= gatePosition.y,
            SecondZoneDirection.Down => playerPosition.y <= gatePosition.y,
            _ => false
        };

        string targetZone = isInSecondZone ? secondZoneName : firstZoneName;
        zoneManager.SwitchToZone(targetZone, player);
    }
}
