using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class MazeController : MonoBehaviour
{
    [SerializeField] private TMP_Text accInput;
    [SerializeField] private TMP_Text rotationText;
    [SerializeField] private float delay;   // in inspector 0.5f
    [SerializeField] private float maxTiltAngle;   // in inspector 45f
    [SerializeField] private float sensitivityMultiplier;   // in inspector 2f

    private Quaternion offsetOrientation;
    private Quaternion currentOrientation;
    private Quaternion relativeRotation;
    private bool isCalibrated = false;

    void Start()
    {
        if (AttitudeSensor.current != null)
        {
            InputSystem.EnableDevice(AttitudeSensor.current);
            StartCoroutine(CalibrateAfterDelay());
        }
        else
        {
            rotationText.text = "AttitudeSensor support : null";
            return;
        }

    }

    void Update()
    {
        if (!isCalibrated) return;

        currentOrientation = AttitudeSensor.current.attitude.ReadValue();

        relativeRotation = Quaternion.Inverse(offsetOrientation) * currentOrientation;
        Vector3 euler = relativeRotation.eulerAngles;

        euler.x = NormalizeAngle(euler.x);
        euler.y = NormalizeAngle(euler.y);
        euler.z = NormalizeAngle(euler.z);

        // Multiply for sensitivity
        euler.x *= sensitivityMultiplier;
        euler.y *= sensitivityMultiplier;
        euler.z *= sensitivityMultiplier;

        // Clamp to ±45 degrees
        euler.x = Mathf.Clamp(euler.x, -135f, -45f);
        euler.z = Mathf.Clamp(euler.z, -maxTiltAngle, maxTiltAngle);

        // fliping y & z
        euler.y = -euler.y;
        euler.z = -euler.z;

        // Start with X and Z world rotation
        Quaternion worldRotation = Quaternion.Euler(euler.x, 0f, euler.z);

        // Create local Y rotation
        Quaternion localYRotation = Quaternion.Euler(0f, euler.y, 0f);

        // Combine: world rotation first, then local Y
        transform.rotation = worldRotation * localYRotation;

        accInput.text = "x : " + euler.x.ToString() + "\n" + "y : " + euler.y.ToString() + "\n" + "z : " + euler.z.ToString() + "\n";     
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            return angle - 360f;
        }
        return angle;
    }

    IEnumerator CalibrateAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        offsetOrientation = AttitudeSensor.current.attitude.ReadValue();
        isCalibrated = true;

        rotationText.text = "calibrated !";
    }
}