using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    [SerializeField]
    private float m_rotationSpeed = 4.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, m_rotationSpeed * Time.unscaledDeltaTime);
    }
}
