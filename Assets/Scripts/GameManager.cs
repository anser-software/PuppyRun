using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    
    public static GameManager instance { get; private set; }

    public float characterCountMultiplierForScore;

    public System.Action OnWin, OnLose;

    public GameState gameState;

    private void Awake()
    {
        instance = this;
    }

    public void Win()
    {
        gameState = GameState.Finished;

        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

        OnWin?.Invoke();
    }

    public void Lose()
    {
        gameState = GameState.Finished;

        OnLose?.Invoke();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        var level = SceneManager.GetActiveScene().buildIndex + 1;

        if (level >= SceneManager.sceneCountInBuildSettings)
            level = 0;

        SceneManager.LoadScene(level);
    }

}

public enum GameState
{
    Playing,
    Finished
}