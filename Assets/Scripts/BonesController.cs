using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonesController : MonoBehaviour
{
    
    public static BonesController instance { get; private set; }

    public Transform targetBone { get {
            if (currentBones.Count > 0)
                return currentBones[0];
            else return null;
        } }

    public List<Transform> currentBones { get; private set; } = new List<Transform>();

    public GameObject bonePrefab;

    public Vector3 boneOffset;

    public int maxBoneCount;

    public float distanceBetweenBones, minDrawingDistance;

    private bool isDrawing = false;

    private void Awake()
    {
        instance = this;

        Finish.instance.OnFinished += () =>
        {
            finished = true;
            ClearBones();
        };
    }

    private void Update()
    {
        if (finished)
            return;


        if(Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }
        if(Input.GetMouseButton(0))
        {
            if (currentBones.Count < 1)
                StartDrawing();

            Draw();
        }
        if(Input.GetMouseButtonUp(0))
        {
            StopDrawing();
        }

        if(currentBones.Count > 0)
        {
            if(currentBones[0].position.z < CrowdManager.instance.furthestCharacter.position.z)
            {
                Destroy(currentBones[0].gameObject);

                currentBones.RemoveAt(0);
            } 
        }
    }

    private void StartDrawing()
    {
        var screenPos = Input.mousePosition;

        var worldPos = InputManager.Instance.GetWorldPos(screenPos);

        if (!InputManager.Instance.validWorldPos || worldPos.z <= CrowdManager.instance.furthestCharacter.position.z + minDrawingDistance)
        {
            return;
        }

        isDrawing = true;

        ClearBones();

        var bone = Instantiate(bonePrefab, worldPos + boneOffset, Quaternion.identity);

        //bone.transform.forward = (currentBones[currentBones.Count - 1].position - bone.transform.position).normalized;

        currentBones.Add(bone.transform);
    }

    private bool finished;

    public void ClearBones()
    {
        foreach (var b in currentBones)
        {
            Destroy(b.gameObject);
        }

        currentBones.Clear();
    }

    private void Draw()
    {
        if (!isDrawing || currentBones.Count < 1)
            return;

        var screenPos = Input.mousePosition;

        var worldPos = InputManager.Instance.GetWorldPos(screenPos);

        if (!InputManager.Instance.validWorldPos || worldPos.z <= CrowdManager.instance.furthestCharacter.position.z)
        {
            StopDrawing();
            return;
        }

        var displacement = worldPos + boneOffset - currentBones[currentBones.Count - 1].position;

        if (displacement.z < 0F)
            return;

        var sqrLength = displacement.sqrMagnitude;

        if (sqrLength >= distanceBetweenBones * distanceBetweenBones)
        {
            var position = currentBones[currentBones.Count - 1].position + displacement.normalized * distanceBetweenBones;

            var bone = Instantiate(bonePrefab, position, bonePrefab.transform.rotation);

            bone.transform.forward = (currentBones[currentBones.Count - 1].position - bone.transform.position).normalized;

            currentBones.Add(bone.transform);

        }

        if (currentBones.Count == maxBoneCount)
            StopDrawing();
    }

    private void StopDrawing()
    {
        isDrawing = false;
    }

    public void EatFirstBone()
    {
        Destroy(currentBones[0].gameObject);

        currentBones.RemoveAt(0);
    }

}
