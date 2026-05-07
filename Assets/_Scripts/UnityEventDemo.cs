using UnityEngine;
using UnityEngine.Events;


public class UnityEventDemo : MonoBehaviour
{
    public UnityEvent OnDamaged;

    public void TakeDamage()
    {
        OnDamaged.Invoke();
    }
}
