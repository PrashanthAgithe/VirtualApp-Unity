using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void LoadClassroom()
    {
        // Loads the classroom (SampleScene)
        SceneManager.LoadScene("SampleScene");
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
