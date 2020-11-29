using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCard : MonoBehaviour
{
	public bool cardflip = false;
	Animator anim;

	
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
		if (Input.GetMouseButton(0))
		{
			
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				
				if (hit.transform != null  && hit.transform.gameObject.tag == "tarrot")
				{
					if (cardflip == false)
					{
						flipCard(hit.transform.gameObject);
					}
					
				}
			}
		}
    }


	private void flipCard(GameObject go)
	{
		anim = go.GetComponent<Animator>();
		anim.Play("Flip");
		cardflip = ! cardflip;
		Debug.Log(go.name);
	}
}
