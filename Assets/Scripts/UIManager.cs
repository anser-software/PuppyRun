using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

    [SerializeField]
    private GameObject win, lose;

    [SerializeField]
    private Text characterCount, multiplier;

    private void Start()
    {
        GameManager.instance.OnWin += OnWin;

        GameManager.instance.OnLose += OnLose;
    }

    private void OnWin()
    {
        characterCount.text = CrowdManager.instance.characters.Count.ToString();

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
