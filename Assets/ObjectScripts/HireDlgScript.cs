using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HireDlgScript : MonoBehaviour
{
    public Text WindowText;

    public Button b0;
    public Text t0;
    public Button b1;
    public Text t1;
    public Button b2;
    public Text t2;
    public Button b3;
    public Text t3;


    // Start is called before the first frame update
    void Awake()
    {
        SetActive(false);
        b0.gameObject.SetActive(false);
        b1.gameObject.SetActive(false);
        b2.gameObject.SetActive(false);
        b3.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        // Convert to loop later?
        ManManager manRef = ManManager.Ref;
        if (manRef.hireList.Count >= 1)
        {
            b0.gameObject.SetActive(true);
            t0.text = manRef.hireList[0].ManFirstName + " " + manRef.hireList[0].ManLastName;
        }
        else
            b0.gameObject.SetActive(false);

        if (manRef.hireList.Count >= 2)
        {
            b1.gameObject.SetActive(true);
            t1.text = manRef.hireList[1].ManFirstName + " " + manRef.hireList[1].ManLastName;
        }
        else
            b1.gameObject.SetActive(false);

        if (manRef.hireList.Count >= 3)
        {
            b2.gameObject.SetActive(true);
            t2.text = manRef.hireList[2].ManFirstName + " " + manRef.hireList[2].ManLastName;
        }
        else
            b2.gameObject.SetActive(false);

        if (manRef.hireList.Count >= 4)
        {
            b3.gameObject.SetActive(true);
            t3.text = manRef.hireList[3].ManFirstName + " " + manRef.hireList[3].ManLastName;
        }
        else
            b3.gameObject.SetActive(false);

        //if(manRef.hireList.)
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
