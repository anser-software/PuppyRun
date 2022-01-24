using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

    [SerializeField]
    private GameObject start, win, lose;

    [SerializeField]
    private Text characterCounter, multiplier, levelCounter;

    private void Start()
    {
        GameManager.instance.OnWin += OnWin;

        GameManager.instance.OnLose += OnLose;

        levelCounter.text = string.Format(levelCounter.text, PlayerPrefs.GetInt("Level", 1).ToString());
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            start.SetActive(false);
        }
    }

    private void OnWin()
    {
        characterCounter.text = CrowdManager.instance.characters.Count.ToString();

        multiplier.text = string.Format(multiplier.text, Mathf.CeilToInt(CrowdManager.instance.characters.Count * GameManager.instance.characterCountMultiplierForScore).ToString());

        win.SetActive(true);
    }

    private void OnLose()
    {
        lose.SetActive(true);
    }

    public void Restart()
    {
        GameManager.instance.Restart();
    }

    public void NextLevel()
    {
        GameManager.instance.NextLevel();
    }

}
