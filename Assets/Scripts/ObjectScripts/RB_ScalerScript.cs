using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RB_ScalerScript : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField] Transform mesh;
    [Header("The multiplier that determines how small the mesh scale can get")]
    [SerializeField] float baseLowMult = 0.2f;
    [Header("The multiplier that determines how large the mesh scale can get")]
    [SerializeField] float baseHighMult = 3;
    [Header("The multiplier that determines how quickly the object scales when moving along the z-axis")]
    [SerializeField] float scaleSpeedMult = 0.05f;
    #endregion

    /// <summary>
    /// Starting scale for the object
    /// </summary>
    private Vector3 baseScale, basePos;

    private float baseLow, baseHigh;
    private float topY; //needs to work with sprites as well

    private void Start()
    {
        baseScale = transform.localScale;
        basePos = mesh.transform.localPosition;
        baseLow = baseScale.magnitude * baseLowMult;
        baseHigh = baseScale.magnitude * baseHighMult;

        topY = mesh.GetComponent<MeshRenderer>().bounds.max.y;
    }

    /// <summary>
    /// The z position of the object from the previous frame of fixed update
    /// </summary>
    private float prevZ;
    void FixedUpdate()
    {
        //Only scale the object if it is moving along the z axis
        if (transform.position.z != prevZ)
        {
            Vector3 scale = mesh.transform.localScale;
            float dist = (transform.position.z - MySpace.Constants.GridPositionWalkZOffset);

            scale = baseScale * (1 - (dist * scaleSpeedMult));


            if (scale.magnitude < baseLow)
                scale = baseScale * baseLow;
            else if (scale.magnitude > baseHigh)
                scale = baseScale * baseHigh;

            mesh.transform.localScale = scale;

            Vector3 pos = mesh.transform.localPosition;
            pos.y = ((baseScale.y - scale.y) / 1.2f) + basePos.y;
            mesh.transform.localPosition = pos;
        }

        prevZ = transform.position.z;
    }
}
