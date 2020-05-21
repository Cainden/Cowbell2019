using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorArrow : MonoBehaviour
{
	[SerializeField] float minAngle, maxAngle;

    public void SetArrowAngle(float t)
    {
        transform.localRotation = Quaternion.Euler(Mathf.Lerp(minAngle, maxAngle, t), 90, 90);
    }

    public void SetArrowAngle(int angle)
    {
        transform.localRotation = Quaternion.Euler(angle, 90, 90);
    }
}
