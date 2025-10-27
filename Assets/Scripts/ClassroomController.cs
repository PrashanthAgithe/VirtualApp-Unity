using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomController : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
