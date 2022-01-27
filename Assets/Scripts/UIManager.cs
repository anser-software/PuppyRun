using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{

    [SerializeField]
    private GameObject start, win, lose, tutorialFood;

    [SerializeField]
    private bool showTutorialFood;

    [SerializeField]
    private float characterZForTutorialFood;

    [SerializeField]
    private Text characterCounter, multiplier, levelCounter;

    [SerializeField]
    private Animation hand;

    [SerializeField, Range(0F, 1F)]
    private float handAnimSpeed;

    private void Start()
    {
        GameManager.instance.OnWin += OnWin;

        GameManager.instance.OnLose += OnLose;

        levelCounter.text = string.Format(levelCounter.text, PlayerPrefs.GetInt("Level", 1).ToString());

        hand["UIHand"].speed = handAnimSpeed;
    }

    private void ShowTutorialFood()
    {
        tutorialFood.SetActive(true);

        Time.timeScale = 0F;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            start.SetActive(false);
            tutorialFood.SetActive(false);
            Time.timeScale = 1F;
        }

        if(showTutorialFood)
        {
            if(CrowdManager.instance.furthestCharacter.position.z > characterZForTutorialFood)
            {
                ShowTutorialFood();

                showTutorialFood = false;
            }
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
