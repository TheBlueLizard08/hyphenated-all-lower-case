using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraMovementType { Loop, Random}

public class SecurityCamera : MonoBehaviour
{
    public Transform bodyPivot;
    public Transform[] lookPoints;
    [Tooltip("The speed in angles/second that the camera moves at")] 
    public float moveSpeed;
    [Tooltip("The time in seconds a camera hangs at a point before moving to the next one")]
    public float pointDelay;
    [Tooltip("The way the camera picks a next point from the list")]
    public CameraMovementType movementType;
    public Camera myCamera;

    Transform currentLookPoint;
    int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //bodyPivot.LookAt(lookPoints[0].position, Vector3.up);
        bodyPivot.rotation = Quaternion.LookRotation(lookPoints[0].position - bodyPivot.position, Vector3.up);
        StartCoroutine(MovementC());
    }

    IEnumerator MovementC()
    {
        while(true)
        {
            yield return new WaitForSeconds(pointDelay);
            
            currentLookPoint = PickNewPoint();

            Quaternion targetRotation = Quaternion.LookRotation(currentLookPoint.position - bodyPivot.position, Vector3.up);

            while (Quaternion.Angle(bodyPivot.rotation, targetRotation) > 0.5f)
            {
                bodyPivot.rotation = Quaternion.RotateTowards(bodyPivot.rotation, targetRotation, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }

    Transform PickNewPoint()
    {
        switch(movementType)
        {
            case CameraMovementType.Loop:
                currentIndex++;
                currentIndex%=lookPoints.Length;
                return lookPoints[currentIndex];
                break;
            case CameraMovementType.Random:
                return Utility.Pick(lookPoints);
            default: throw new System.ArgumentException("Camera movement type not defined");
        }
    }

    public void PlayerInSight()
    {
        myCamera.gameObject.SetActive(true);
        GameManager.Instance.SetCameraUI(true);
    }

    public void PlayerOutOfSight()
    {
        myCamera.gameObject.SetActive(false);
        GameManager.Instance.SetCameraUI(false);
    }
}
