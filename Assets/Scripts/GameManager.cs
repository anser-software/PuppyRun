using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HomaGames.HomaBelly;
public class GameManager : MonoBehaviour
{
    
    public static GameManager instance { get; private set; }

    public float characterCountMultiplierForScore;

    public System.Action OnWin, OnLose;

    public GameState gameState;

    private void Awake()
    {
        instance = this;

        PlayerPrefs.SetInt("Scene", SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        DefaultAnalytics.LevelStarted(PlayerPrefs.GetInt("Scene") + 1);
    }

    public void Win()
    {
        gameState = GameState.Finished;
        DefaultAnalytics.LevelCompleted();

        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

        OnWin?.Invoke();
    }

    public void Lose()
    {
        gameState = GameState.Finished;
        DefaultAnalytics.LevelFailed();

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
            level = 1;

        SceneManager.LoadScene(level);
    }

}

public enum GameState
{
    Playing,
    Finished
}