using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField]
    private int mainMenuScene;
    public TMP_Text scoreText;
    public TMP_Text demonsKilled;

    private void Awake() 
    {
        scoreText.text = "Score: "+ GameController.instance.playerScore.ToString();
        demonsKilled.text = "Demons Killed: "+ GameController.instance.playerKills.ToString();      
    }
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenuScene);
    }
}
