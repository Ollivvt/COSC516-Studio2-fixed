using UnityEngine;

public class LaunchIndicator : MonoBehaviour
{
    void Update()
    {
        if (Camera.main != null)
        {
            transform.forward = Camera.main.transform.forward;
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }
    }
}

