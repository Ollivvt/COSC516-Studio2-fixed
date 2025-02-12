using UnityEngine;

public class Gutter : MonoBehaviour
{
    private void OnTriggerEnter(Collider triggeredBody)
    {
        // **Ensure only the ball enters the gutter**
        if (!triggeredBody.CompareTag("Ball")) return;

        Rigidbody ballRigidBody = triggeredBody.GetComponent<Rigidbody>();
        BallController ballController = triggeredBody.GetComponent<BallController>();

        if (ballRigidBody != null && ballController != null)
        {
            ballController.SetInGutter(true); // Mark ball as in the gutter
            Debug.Log("Ball entered gutter. Forcing full centering.");

            // **Force ball immediately to the exact center of the gutter**
            Vector3 targetPosition = new Vector3(
                transform.position.x, 
                transform.position.y + 0.1f, // Slight lift to prevent collider sticking
                triggeredBody.transform.position.z
            );

            triggeredBody.transform.position = targetPosition;

            // **Eliminate lateral movement and ensure forward motion**
            Vector3 velocity = ballRigidBody.linearVelocity;
            velocity.x = 0f; // No sideways movement
            velocity.y = 0f; // Prevent bounce
            velocity.z = Mathf.Max(velocity.z, 5f); // Ensure steady forward motion
            ballRigidBody.linearVelocity = velocity;

            // **Apply a smooth forward force to prevent stopping**
            ballRigidBody.AddForce(transform.forward * 5f, ForceMode.VelocityChange);
        }
    }

    private void OnTriggerStay(Collider triggeredBody)
    {
        // **Ensure only the ball stays in the gutter**
        if (!triggeredBody.CompareTag("Ball")) return;

        Rigidbody ballRigidBody = triggeredBody.GetComponent<Rigidbody>();

        if (ballRigidBody != null)
        {
            // **Continuously correct ball position to remain centered**
            Vector3 targetPosition = new Vector3(
                transform.position.x, 
                transform.position.y + 0.1f, // Maintain proper height to prevent collision glitches
                triggeredBody.transform.position.z
            );

            triggeredBody.transform.position = Vector3.Lerp(triggeredBody.transform.position, targetPosition, Time.deltaTime * 10f);

            // **Maintain strong forward force to keep motion smooth**
            ballRigidBody.linearVelocity = new Vector3(0f, 0f, Mathf.Max(ballRigidBody.linearVelocity.z, 5f));
            ballRigidBody.AddForce(transform.forward * 5f, ForceMode.Acceleration);
        }
    }

    private void OnTriggerExit(Collider triggeredBody)
    {
        // **Ensure only the ball can exit the gutter**
        if (!triggeredBody.CompareTag("Ball")) return;

        BallController ballController = triggeredBody.GetComponent<BallController>();
        if (ballController != null)
        {
            ballController.SetInGutter(false); // Reset state when leaving gutter
        }
    }
}
