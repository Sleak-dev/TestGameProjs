using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    #region Variables
    public Transform target;
    [Header("Base Settings")]
    public float sensitivity = 2;
    public Vector2 lookMinMax = new Vector2(-30, 30);
    [Header("Positioning")]
    public Vector3 offset;
    public float transitionTime = 0.2f;
    [Header("Field of View")]
    public float normalViewAngle = 70;
    public float aimViewAngle = 30;
    public float aimAdjustmentTime = 0.05f;
    [Header("Wall Collision")]
    public float minDist = 0f;
    private float maxDist;
    public float smooth = 0.1f;
    Vector3 dollyDir;
    private float zDistance;
    private float xDistance;
    [Header("Overides")]
    public bool overideCamera;

    private float mouseX, mouseY, look, turn, finalX, finalY;
    #endregion
    #region On Game Start
    private void Start()
    {
        dollyDir = transform.localPosition.normalized;
        zDistance = transform.localPosition.magnitude;
        maxDist = offset.z;
    }
    #endregion
    #region Updates
    private void Update()
    {
        CameraCollisionCheck();
    }
    void LateUpdate()
    {
        InputSettings();
        Rotation();
        OtherInputs();
    }
    #endregion
    #region Completed Functions
    void InputSettings()
    {
        //Look
        mouseX -= Input.GetAxis("Mouse Y");
        mouseX = Mathf.Clamp(mouseX, lookMinMax.x, lookMinMax.y);
        look -= Input.GetAxis("Look");
        look = Mathf.Clamp(look, lookMinMax.x, lookMinMax.y);

        //Turn
        mouseY += Input.GetAxis("Mouse X");
        turn += Input.GetAxis("Turn");

        finalX = (mouseX + look) * sensitivity;
        finalY = (mouseY + turn) * sensitivity;
    }
    void Rotation()
    {
        Quaternion newRot = Quaternion.Euler(finalX, finalY, 0);
        Quaternion playerRot = Quaternion.Euler(0, finalY, 0);

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || Input.GetButton("Aim") || overideCamera)
            target.parent.GetComponentInParent<Transform>().rotation = Quaternion.Slerp(target.parent.GetComponent<Transform>().rotation, playerRot, transitionTime);

        target.rotation = newRot;

        transform.position = Vector3.Lerp(transform.position, target.transform.position - (newRot * offset), transitionTime);
    }
    void OtherInputs()
    {
        if (Input.GetButton("Aim"))
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, aimViewAngle, aimAdjustmentTime);
            Time.timeScale = 0.7f;
        }
        else if (GetComponent<Camera>().fieldOfView != normalViewAngle)
        {
            GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, normalViewAngle, aimAdjustmentTime);
            Time.timeScale = 1f;
        }
    }
    void CameraCollisionCheck()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDist);
        RaycastHit hit;

        if (Physics.Linecast(transform.parent.position, desiredCameraPos, out hit))
        {
            zDistance = Mathf.Clamp((hit.distance * 0.9f), minDist, maxDist);
        }
        else
        {
            zDistance = maxDist;
        }
        Vector3 newPos = new Vector3(offset.x, offset.y, -zDistance);

        transform.localPosition = Vector3.Lerp(newPos, dollyDir * zDistance, smooth);

    }
    #endregion
}
