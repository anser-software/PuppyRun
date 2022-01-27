using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class CrowdManager : MonoBehaviour
{
    
    public static CrowdManager instance { get; private set; }

    public List<CharacterController> characters { get; private set; } = new List<CharacterController>();
    public Vector3 averagePos { get; private set; }
    public Transform furthestCharacter { get; private set; }
    public Transform nearestCharacter { get; private set; }
    public bool immuneToRemove { get; private set; }

    public float speed { get; private set; }

    public float maxDistanceToFurthest;

    [SerializeField]
    private int maxCharactersOnFinish;

    [SerializeField]
    private GameObject characterPrefab, removeCharacterFX, speedUpFX;

    [SerializeField]
    private bool modifyDefaultSpeed;

    [SerializeField]
    private float defaultSpeed, immunityDuration;

    private float targetSpeed;

    private bool finish;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        targetSpeed = speed = defaultSpeed;

        characters = FindObjectsOfType<CharacterController>().ToList();

        SetAveragePosition();

        SetFurthestAndNearestPositions();
    }

    private void Update()
    {
        if (characters.Count == 0 && GameManager.instance.gameState == GameState.Playing)
        {
            Debug.Log("LOSE");
            GameManager.instance.Lose();
            return;
        }

        SetAveragePosition();

        SetFurthestAndNearestPositions();
    }
    public void MultiplySpeed(float value, float changeDuration, float totalDuration)
    {
        var seq = DOTween.Sequence();

        seq.AppendInterval(totalDuration);

        targetSpeed = modifyDefaultSpeed ? defaultSpeed * value : targetSpeed * value;

        seq.Join(DOTween.To(() => speed, x => speed = x, targetSpeed, changeDuration));

        DOTween.Kill("RESET");

        seq.Append(DOTween.To(() => speed, x => speed = x, defaultSpeed, changeDuration).SetId("RESET").OnComplete(() => targetSpeed = defaultSpeed));


        if (speedUpFX)
        {
            speedUpFX.SetActive(true);

            DOTween.Sequence().SetDelay(totalDuration).OnComplete(() => speedUpFX.SetActive(false));
        }
    }

    public void AddSpeed(float value, float changeDuration, float totalDuration)
    {
        var seq = DOTween.Sequence();

        seq.AppendInterval(totalDuration);

        targetSpeed = modifyDefaultSpeed ? defaultSpeed + value : targetSpeed + value;

        seq.Join(DOTween.To(() => speed, x => speed = x, targetSpeed, changeDuration));

        DOTween.Kill("RESET");

        seq.Append(DOTween.To(() => speed, x => speed = x, defaultSpeed, changeDuration).SetId("RESET").OnComplete(() => targetSpeed = defaultSpeed));

        if (speedUpFX)
        {
            speedUpFX.SetActive(true);

            DOTween.Sequence().SetDelay(totalDuration).OnComplete(() => speedUpFX.SetActive(false));
        }
    }

    private void SetFurthestAndNearestPositions()
    {
        int furthestIndex = 0;

        float furthestZ = Mathf.NegativeInfinity;

        int nearestIndex = 0;

        float nearestZ = Mathf.Infinity;

        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].transform.position.z > furthestZ)
            {
                furthestZ = characters[i].transform.position.z;
                furthestIndex = i;
            } else if(characters[i].transform.position.z < nearestZ)
            {
                nearestZ = characters[i].transform.position.z;
                nearestIndex = i;
            }
        }

        furthestCharacter = characters[furthestIndex].transform;
        nearestCharacter = characters[nearestIndex].transform;
    }

    private void SetAveragePosition()
    {
        var avgPos = Vector3.zero;

        foreach (var c in characters)
        {
            if(c)
                avgPos += c.transform.position;
        }

        averagePos = avgPos / characters.Count;
    }

    public void AddCharacter()
    {
        var offset = Random.insideUnitCircle;

        var character = Instantiate(characterPrefab, averagePos + new Vector3(offset.x, 0F, offset.y), Quaternion.identity);

        characters.Add(character.GetComponent<CharacterController>());

        immuneToRemove = true;

        DOTween.Sequence().SetDelay(immunityDuration).OnComplete(() => immuneToRemove = false);
    }

    public void RemoveRandomCharacter()
    {
        int randomIndex = Random.Range(0, characters.Count);

        Destroy(characters[randomIndex].gameObject);

        characters.RemoveAt(randomIndex);

        DOTween.Sequence().SetDelay(immunityDuration).OnComplete(() => immuneToRemove = false);
    }

    public void RemoveCharacter(CharacterController character) 
    {
        characters.Remove(character);

        Destroy(character.gameObject);
    }

    public void FreezeCharacter(CharacterController character, Vector3 targetPos)
    {
        character.Sit();

        characters.Remove(character);

        character.Catch(targetPos);
    }

    public void RemoveLast()
    {
        int nearestIndex = 0;

        float nearestZ = Mathf.Infinity;

        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].transform.position.z < nearestZ)
            {
                nearestZ = characters[i].transform.position.z;
                nearestIndex = i;
            }
        }

        var charToRemove = characters[nearestIndex];

        characters.RemoveAt(nearestIndex);

        if(removeCharacterFX)
        {
            Instantiate(removeCharacterFX, charToRemove.transform.position, Quaternion.identity);
        }

        Destroy(charToRemove.gameObject);
    }

    public void ActivateFinish()
    {
        if (finish)
            return;

        finish = true;

        var targetPositionsOrdered = Finish.instance.GetTargetPositions(characters.Count).OrderBy(p => p.z).ToArray();

        var charactersOrdered = characters.OrderByDescending(c => c.transform.position.z).ToArray();

        var charactersToSpawn = Mathf.Min(maxCharactersOnFinish, charactersOrdered.Length);

        for (int i = 0; i < charactersOrdered.Length; i++)
        {
            if (i < charactersToSpawn)
            {
                charactersOrdered[i].transform.DOMove(targetPositionsOrdered[i], Vector3.Distance(charactersOrdered[i].transform.position, targetPositionsOrdered[i]) / speed).SetEase(Ease.Linear);
                charactersOrdered[i].SetFinish(targetPositionsOrdered[i]);
            } else
            {
                var character = charactersOrdered[i];
                charactersOrdered[i].transform.DOScale(0F, 1F).OnComplete(() =>
                {
                    Destroy(character.gameObject);
                    //characters.Remove(character);
                });
            }
        }
    }

}
