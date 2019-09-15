// Managing different man avatar versions.

using MySpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManFactory : MonoBehaviour
{
    [HideInInspector]
    public static ManFactory Ref { get; private set; } // For external access of script

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<ManFactory>();
    }

    public GameObject CreateMan(Enums.ManTypes manType)
    {
        ManDefData DefData = GetManDefData(manType);
        GameObject ManObject = LoadMan(DefData);
        ManObject.SetActive(true);
        return (ManObject);
    }

    private GameObject LoadMan(ManDefData defData)
    {
        return (Instantiate(Resources.Load<GameObject>(defData.ManModelFile)));
    }

    public ManDefData GetManDefData(Enums.ManTypes manType)
    {
        foreach (ManDefData DefData in Constants.ManDefinitions)
        {
            if ((DefData.ManType == manType))
            {
                return (DefData);
            }
        }

        Debug.Assert(1 == 0);
        return (Constants.ManDefinitions[0]);
    }
}
