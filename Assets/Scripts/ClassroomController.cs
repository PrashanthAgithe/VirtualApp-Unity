using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomController : MonoBehaviour
{
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void GoToPRPSA()
    {
        SceneManager.LoadScene("PRPSAScene");
    }
        public void ExitApp()
    {
        // Quits app (works only in build)
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
