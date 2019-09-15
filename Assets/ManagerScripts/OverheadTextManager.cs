using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverheadTextManager : MonoBehaviour
{

    //Directions to pass into StartText()
    public enum eMoveDirections { STATIC = -1, N, NE, E, SE, S, SW, W, NW }

    [HideInInspector]
    public static OverheadTextManager Ref { get; private set; } // For external access of script

    //[SerializeField]
   // public GameObject originObj;

    [SerializeField]
    public GameObject overHeadTextPrefab;

    //[SerializeField]
    //public Color[] textColors;

    //[SerializeField]
    //public float textKillTime;

    void Awake()
    {
        if (Ref == null) Ref = GetComponent<OverheadTextManager>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartText(string txtStr, Color txtColor, Vector3 originPos, float txtKillTime, eMoveDirections moveDir = eMoveDirections.STATIC, float txtSpeed = 0, float txtMoveAmt = 0)
    {
        GameObject newOverheadText = Instantiate(overHeadTextPrefab, originPos, Quaternion.identity);
        newOverheadText.SetActive(true);
        newOverheadText.GetComponent<OverHeadTextScript>().SetOverheadText(txtStr,
            txtColor, moveDir, txtSpeed, txtMoveAmt);
        Destroy(newOverheadText.gameObject, txtKillTime);
    }

    public void OverheadHoots(string txtStr, Vector3 originPos)
    {
        StartText(txtStr, Color.green, originPos, 1.0f, eMoveDirections.N, 1.5f, 1.0f);
    }
}
