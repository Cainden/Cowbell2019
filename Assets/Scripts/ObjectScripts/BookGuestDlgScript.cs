using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookGuestDlgScript : MonoBehaviour
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
    void Start()
    {
        //SetActive(false);
        b0.gameObject.SetActive(false);
        b1.gameObject.SetActive(false);
        b2.gameObject.SetActive(false);
        b3.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //Button[] buttons = GetComponentsInChildren<Button>();
        //
        // for (int i = 0; i < ManManager.Instance.bookingList.Count; ++i)
        // {
        //     if (i >= 4)
        //         break;
        //
        //     buttons[i].gameObject.SetActive(true);
        // }
        ManManager manRef = ManManager.Instance;
        if (manRef.bookingList.Count >= 1)
        {
            b0.gameObject.SetActive(true);
            t0.text = manRef.bookingList[0].manFirstName + " " + manRef.bookingList[0].manLastName;
        }
        else
            b0.gameObject.SetActive(false);

        if (manRef.bookingList.Count >= 2)
        {
            b1.gameObject.SetActive(true);
            t1.text = manRef.bookingList[1].manFirstName + " " + manRef.bookingList[1].manLastName;
        }
        else
            b1.gameObject.SetActive(false);

        if (manRef.bookingList.Count >= 3)
        {
            b2.gameObject.SetActive(true);
            t2.text = manRef.bookingList[2].manFirstName + " " + manRef.bookingList[2].manLastName;
        }
        else
            b2.gameObject.SetActive(false);

        if (manRef.bookingList.Count >= 4)
        {
            b3.gameObject.SetActive(true);
            t3.text = manRef.bookingList[3].manFirstName + " " + manRef.bookingList[3].manLastName;
        }
        else
            b3.gameObject.SetActive(false);

        //if(manRef.bookingList.)
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }
}
