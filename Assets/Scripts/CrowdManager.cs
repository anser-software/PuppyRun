using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CrowdManager : MonoBehaviour
{
    
    public static CrowdManager instance { get; private set; }

    public List<CharacterController> characters { get; private set; } = new List<CharacterController>();

    public float maxDistanceToFurthest;

    [SerializeField]
    private GameObject characterPrefab;

    public Vector3 averagePos { get; private set; }
    public Vector3 furthestPos { get; private set; }


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        characters = FindObjectsOfType<CharacterController>().ToList();

        SetAveragePosition();

        SetFurthestPosition();
    }

    private void Update()
    {
        SetAveragePosition();

        SetFurthestPosition();
    }

    private void SetFurthestPosition()
    {
        int furthestIndex = 0;

        float furthestZ = Mathf.NegativeInfinity;

        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].transform.position.z > furthestZ)
            {
                furthestZ = characters[i].transform.position.z;
                furthestIndex = i;
            }
        }

        furthestPos = characters[furthestIndex].transform.position;
    }

    private void SetAveragePosition()
    {
        var avgPos = Vector3.zero;

        foreach (var c in characters)
        {
            avgPos += c.transform.position;
        }

        averagePos = avgPos / characters.Count;
    }

    public void AddCharacter()
    {
        var character = Instantiate(characterPrefab, averagePos, Quaternion.identity);

        characters.Add(character.GetComponent<CharacterController>());
    }

    public void RemoveRandomCharacter()
    {
        int randomIndex = Random.Range(0, characters.Count);

        Destroy(characters[randomIndex]);

        characters.RemoveAt(randomIndex);
    }

    public void RemoveCharacter(CharacterController character) 
    {
        characters.Remove(character);

        Destroy(character.gameObject);
    }

}
