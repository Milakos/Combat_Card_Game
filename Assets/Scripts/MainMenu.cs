using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
    using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private int nextScene;

    public void NewGame()
    {
        SceneManager.LoadSceneAsync(nextScene);
    }
    public void QuitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
