using UnityEngine;
using UnityEngine.Events;

public class FallTrigger : MonoBehaviour
{
    public UnityEvent OnPinFall = new();
    private bool isPinFallen = false;

    private void OnTriggerEnter(Collider triggeredObject)
    {
        // Ensure it only triggers when hitting the ground
        if (triggeredObject.CompareTag("Ground") && !isPinFallen)
        {
            isPinFallen = true;
            OnPinFall?.Invoke();
            Debug.Log($"{gameObject.name} has fallen.");
        }
    }
}
