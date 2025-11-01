using UnityEngine;
using TMPro; // 1. Import TextMeshPro!

public class MessageManager : MonoBehaviour
{
    // 2. This creates the "Singleton"
    // Any script can access this manager by writing "MessageManager.Instance"
    public static MessageManager Instance { get; private set; }

    // 3. Drag your text object here in the Inspector
    public TMP_Text messageBoxText;

    private void Awake()
    {
        // 4. Set up the Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: keeps it alive between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 5. This is the public function that other scripts will call
    public void ShowMessage(string message)
    {
        if (messageBoxText != null)
        {
            messageBoxText.text = message;
        }
        else
        {
            Debug.LogWarning("No Message!");
        }
    }
}