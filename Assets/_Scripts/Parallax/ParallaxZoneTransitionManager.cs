using System.Collections;
using UnityEngine;

public class ParallaxZoneTransitionManager : MonoBehaviour
{
    [System.Serializable]
    public class Zone
    {
        public string zoneName;
        public LevelCamera levelCamera;
        public GameObject parallaxRoot;
    }

    [SerializeField] private Zone[] zones;
    [SerializeField] private string startZonename;
    [SerializeField] private UI_FadeEffect fadeEffect;

    [Header("Timing")]
    [SerializeField] private float fadeOutDuration = 0.25f;
    [SerializeField] private float blackHoldDuration = 0.15f;
    [SerializeField] private float fadeInDuration = 0.25f;

    private Zone currentZone;
    private bool isTransitioning;

    private void Start()
    {
        currentZone = FindZone(startZonename);

        foreach (Zone zone in zones)
        {
            bool isStartZone = zone == currentZone;

            if(zone.parallaxRoot != null)
                zone.parallaxRoot.SetActive(isStartZone);

            if (zone.levelCamera != null)
                zone.levelCamera.EnableCamera(isStartZone);
               
        }

        if (fadeEffect != null)
            fadeEffect.ScreenFade(0, 0);
    }

    public void SwitchToZone(string zoneName,Player player)
    {
        if (isTransitioning)
            return;

        Zone nextZone = FindZone(zoneName);

        if (nextZone == null || nextZone == currentZone)
            return;

        StartCoroutine(SwitchZoneRoutine(nextZone, player));
    }

    private IEnumerator SwitchZoneRoutine(Zone nextZone,Player player)
    {
        isTransitioning = true;
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        PlayerInputHandler input = player.GetComponent<PlayerInputHandler>();
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (movement != null)
            movement.enabled = false;

        if(input != null)
            input.enabled = false;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (fadeEffect != null)
            fadeEffect.ScreenFade(1, fadeOutDuration);

        yield return new WaitForSeconds(fadeOutDuration);

        if (nextZone.parallaxRoot != null)
            nextZone.parallaxRoot.SetActive(true);

        if(nextZone.levelCamera != null)
        {
            nextZone.levelCamera.EnableCamera(true);
            nextZone.levelCamera.SetNewTarger(player.transform);
        }

        if(currentZone != null)
        {
            if (currentZone.parallaxRoot != null)
                currentZone.parallaxRoot.SetActive(false);

            if (currentZone.levelCamera != null)
                currentZone.levelCamera.EnableCamera(false);
        }

        currentZone = nextZone;

        yield return new WaitForSeconds(blackHoldDuration);

        if(fadeEffect != null)
            fadeEffect.ScreenFade(0, fadeInDuration);

        yield return new WaitForSeconds(fadeInDuration);

        if (movement != null)
            movement.enabled = true;

        if(input != null)
            input.enabled = true;

        isTransitioning = false;
    }

    private Zone FindZone(string zoneName)
    {
        foreach (Zone zone in zones)
        {
            if(zone.zoneName == zoneName) 
                return zone;
        }
        return null;
    }
}
