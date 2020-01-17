using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextHitScript : MonoBehaviour
{
    [SerializeField]
    public GameObject originObj;

    [SerializeField]
    public GameObject overHeadTextPrefab;

    [SerializeField]
    public Color[] textColors;

    [SerializeField]
    public float textKillTime;

    float fElapsedTime = 0;


    private void Update()
    {
        fElapsedTime += Time.deltaTime;

        if (fElapsedTime >= 2.0f)
        {
            fElapsedTime = 0;
            OverheadTextManager.Ref.OverheadHoots('+' + 10.ToString() + '$', transform.position);
        }


        if( Input.GetMouseButtonDown(0))
        {

            OverheadTextManager.Ref.StartText("Click!", Color.blue, transform.position, 1.0f);
        }
    }


}
