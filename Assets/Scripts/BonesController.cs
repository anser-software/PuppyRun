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

    public float distanceBetweenBones;

    private bool isDrawing = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
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
    }

    private void StartDrawing()
    {
        if (!InputManager.Instance.validWorldPos || InputManager.Instance.mouseWorldPosition.z <= CrowdManager.instance.furthestCharacter.position.z)
        {
            return;
        }

        isDrawing = true;

        foreach (var b in currentBones)
        {
            Destroy(b.gameObject);
        }

        currentBones.Clear();

        var bone = Instantiate(bonePrefab, InputManager.Instance.mouseWorldPosition + boneOffset, bonePrefab.transform.rotation * Quaternion.AngleAxis(90F, Vector3.forward));

        currentBones.Add(bone.transform);
    }

    private void Draw()
    {
        if (!isDrawing || currentBones.Count < 1)
            return;

        if (!InputManager.Instance.validWorldPos || InputManager.Instance.mouseWorldPosition.z <= CrowdManager.instance.furthestCharacter.position.z)
        {
            StopDrawing();
            return;
        }

        var displacement = InputManager.Instance.mouseWorldPosition + boneOffset - currentBones[currentBones.Count - 1].position;

        if (displacement.z < 0F)
            return;

        var sqrLength = displacement.sqrMagnitude;

        if (sqrLength >= distanceBetweenBones * distanceBetweenBones)
        {
            var position = currentBones[currentBones.Count - 1].position + displacement.normalized * distanceBetweenBones;

            var bone = Instantiate(bonePrefab, position, bonePrefab.transform.rotation);

            bone.transform.right = Vector3.up;

            var targetForward = (currentBones[currentBones.Count - 1].position - bone.transform.position).normalized;

            bone.transform.rotation = Quaternion.AngleAxis(90F, targetForward) * Quaternion.AngleAxis(
                Vector3.Angle(bone.transform.forward, targetForward),
                bone.transform.right);

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
