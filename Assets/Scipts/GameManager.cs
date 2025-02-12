using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float score = 0;
    
    [SerializeField] private BallController ball; // Ball reference
    [SerializeField] private GameObject pinCollection; // Prefab reference for new pin sets
    [SerializeField] private Transform pinAnchor; // Reference for pin spawn location
    [SerializeField] private InputManager inputManager; // Input Manager reference
    [SerializeField] private TextMeshProUGUI scoreText; // UI Score Display

    private FallTrigger[] fallTriggers;
    private GameObject pinObjects; // Tracks the active pins

    private void Start()
{
    if (pinCollection == null)
    {
        Debug.LogError("[GameManager] WARNING: pinCollection is NULL on Start! Check Inspector.");
    }
    else
    {
        Debug.Log("[GameManager] pinCollection is correctly assigned.");
        DontDestroyOnLoad(pinCollection); // Ensures it persists across resets
    }

    inputManager.OnResetPressed.AddListener(HandleReset);
    FindExistingPins();
}


    private void HandleReset()
    {
        Debug.Log("[GameManager] Reset button pressed.");
        ball.ResetBall();
        StartCoroutine(ResetPins());
    }

    private void FindExistingPins()
    {
        pinObjects = GameObject.FindWithTag("PinCollection");

        if (pinObjects == null)
        {
            Debug.Log("[GameManager] No existing pins found. Creating new pins.");
            pinObjects = Instantiate(pinCollection, pinAnchor.position, Quaternion.identity);
            pinObjects.tag = "PinCollection";
        }
        else
        {
            Debug.Log("[GameManager] Existing pins found. Using them.");
        }

        AttachScoreTracking();
    }

    private IEnumerator ResetPins()
{
    Debug.Log("[GameManager] Resetting Pins...");

    // 1. **CLEAR SCORE EVENT LISTENERS FIRST**
    if (fallTriggers != null)
    {
        foreach (FallTrigger pin in fallTriggers)
        {
            pin.OnPinFall.RemoveAllListeners();
        }
    }

    // 2. **DESTROY ONLY THE CLONED PINS, NEVER THE PREFAB**
    if (pinObjects != null)
    {
        Debug.Log("[GameManager] Destroying old pin instances...");
        Destroy(pinObjects);
        pinObjects = null;
    }

    // **WAIT UNTIL NEXT FRAME TO ENSURE OBJECTS ARE DESTROYED**
    yield return new WaitForEndOfFrame();

    // 3. **MAKE SURE THE PREFAB STILL EXISTS BEFORE SPAWNING NEW PINS**
    if (pinCollection == null)
    {
        Debug.LogError("[GameManager] CRITICAL ERROR: pinCollection is NULL! It should NOT be destroyed.");
        yield break;
    }

    // 4. **INSTANTIATE A NEW SET OF PINS SAFELY**
    Debug.Log("[GameManager] Spawning new pins...");
    pinObjects = Instantiate(pinCollection, pinAnchor.position, Quaternion.identity);
    pinObjects.tag = "PinCollection";

    // 5. **WAIT TO ENSURE THE NEW PINS ARE CREATED**
    yield return new WaitForEndOfFrame();

    // 6. **ATTACH SCORE TRACKING TO NEW PINS**
    AttachScoreTracking();
    Debug.Log("[GameManager] Pin reset complete.");
}

    private void AttachScoreTracking()
    {
        if (pinObjects == null)
        {
            Debug.LogWarning("[GameManager] No pins found when trying to attach score tracking.");
            return;
        }

        fallTriggers = pinObjects.GetComponentsInChildren<FallTrigger>();
        foreach (FallTrigger pin in fallTriggers)
        {
            pin.OnPinFall.RemoveAllListeners(); // Prevent duplicate listeners
            pin.OnPinFall.AddListener(IncrementScore);
        }
    }

    private void IncrementScore()
    {
        score++;
        scoreText.text = $"Score: {score}";
    }
}
