using UnityEngine;

public class Trap_FireButton : MonoBehaviour
{
    private Animator myAnimator;
    private Trap_Fire trapFire;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        trapFire = GetComponentInParent<Trap_Fire>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (player != null)
        {
            myAnimator.SetTrigger("activate");
            trapFire.SwitchOffFire();
        }
    }
}
