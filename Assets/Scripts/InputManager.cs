using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    
    public static InputManager Instance { get; private set; }
    public Vector3 mouseViewportPosition { get; private set; }

    public Vector3 mouseViewportPositionDelta { get; private set; }

    public Vector3 mouseWorldPosition { get; private set; }
    public Vector3 mouseWorldPositionDelta { get; private set; }

    public bool validWorldPos { get; private set; }

    public LayerMask mouseRaycastTarget;

    private Vector3 lastViewportMousePos, lastWorldMousePos;

    private Camera mainCam;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainCam = Camera.main;

        mouseViewportPosition = lastViewportMousePos = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, Input.mousePosition.z);

        SetMouseWorldPos(mouseViewportPosition);
    }

    private void Update()
    {
        mouseViewportPosition = new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height, Input.mousePosition.z);

        mouseViewportPositionDelta = mouseViewportPosition - lastViewportMousePos;

        lastViewportMousePos = mouseViewportPosition;

        SetMouseWorldPos(mouseViewportPosition);
    }

    private void SetMouseWorldPos(Vector3 mouseViewportPos)
    {
        if (Physics.Raycast(mainCam.ViewportPointToRay(mouseViewportPos), out RaycastHit hitInfo, Mathf.Infinity, mouseRaycastTarget.value))
        {
            mouseWorldPosition = hitInfo.point;

            mouseWorldPositionDelta = mouseWorldPosition - lastWorldMousePos;

            lastWorldMousePos = mouseWorldPosition;

            validWorldPos = true;
        } else
        {
            validWorldPos = false;
        }
    }

}
