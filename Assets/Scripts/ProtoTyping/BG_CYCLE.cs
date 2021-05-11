using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_CYCLE : MonoBehaviour
{
	public TimeManager timetrack;
	public Material bgMAT;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		bgMAT.SetFloat("trans", TimeManager.RatioCycleTime);
		Debug.Log(TimeManager.RatioCycleTime);
    }
}
