using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class streetLights : MonoBehaviour
{
	[Range(0.0f,1f)]
	public float strtTntesity;

	[SerializeField]
	List<Light2D> srtLights = new List<Light2D>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		for(int i = 0; i< srtLights.Count; i++)
		{
			srtLights[i].intensity = strtTntesity;
		}
		
	}
}
