using DG.Tweening;
using UnityEngine;

public class CameraControls : MonoBehaviour
{

    public const float RotationIncrement = 45f;
    public const float RotationDuration = 1f;
    public const float ZoomIncrement = 2f;
    public const float ZoomDuration = 0.5f;
    public const float MovementSpeed = 0.3f;
    public const int MaxCameraZoomIn = 3;
    public const int MaxCameraZoomOut = -3;


    public Transform target;
    public Transform camera;

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(new Vector3(Input.GetAxis("Horizontal") * MovementSpeed, 0, Input.GetAxis("Vertical") * MovementSpeed));

        //Zoom(Input.GetAxis("Mouse ScrollWheel"));

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            Zoom(1);
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            Zoom(-1);
        }
        if (Input.GetKeyUp(KeyCode.Comma))
        {
            RotateCamera(Rotation.Clockwise);
        }
        if (Input.GetKeyUp(KeyCode.Period))
        {
            RotateCamera(Rotation.Anticlockwise);
        }
    }

    private int CameraZoomLevel = 0;
    private bool isZooming = false;

    private void Zoom(int sign)
    {
        if (isZooming)
        {
            return;
        }
        if (CameraZoomLevel + sign < MaxCameraZoomOut || CameraZoomLevel + sign > MaxCameraZoomIn)
        {
            return;
        }
        isZooming = true;
        CameraZoomLevel += sign;
        var targetPosition = target.localPosition * 0.25f * CameraZoomLevel;
        camera.DOLocalMove(targetPosition, ZoomDuration).OnComplete(() => isZooming = false);
    }

    private bool isRotating = false;

    private void RotateCamera(Rotation rotation)
    {
        if (isRotating)
        {
            return;
        }
        isRotating = true;
        short sign = 1;
        if (rotation == Rotation.Anticlockwise)
        {
            sign = -1;
        }
        var targetRotation = transform.localEulerAngles;
        targetRotation.y += sign * RotationIncrement;
        transform.DORotate(targetRotation, RotationDuration).SetEase(Ease.InOutCirc).OnComplete(() => isRotating = false);
    }

    private enum Rotation { Clockwise, Anticlockwise };
}

