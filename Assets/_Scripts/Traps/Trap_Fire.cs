using UnityEngine;
using System.Collections;

public class Trap_Fire : MonoBehaviour
{
    [SerializeField] private Trap_FireButton fireButton;
    [SerializeField] private float offDuration;

    private Animator anim;
    private CapsuleCollider2D fireCollider;
    private bool isActive;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        fireCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Start()
    {
        if (fireButton == null)
        {
            Debug.LogWarning("Don't have fire button" + gameObject.name + "!");
        }
        SetFire(true);
    }
    public void SwitchOffFire()
    {
        if (!isActive) { return; }
        StartCoroutine(FireCourtine());
    }
    private IEnumerator FireCourtine()
    {
        SetFire(false);
        yield return new WaitForSeconds(offDuration);
        SetFire(true);
    }
    private void SetFire(bool active)
    {
        anim.SetBool("active", active);
        fireCollider.enabled = active;
        isActive = active;
    }
}
