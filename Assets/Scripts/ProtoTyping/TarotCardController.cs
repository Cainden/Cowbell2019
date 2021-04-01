using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TarotCardController : MonoBehaviour
{
	public List<TarotCard> TarotCards = new List<TarotCard>();
	private Animator TarotCardAnimator;
	
	// Start is called before the first frame update
	void Start()
    {
        TarotCardAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
		// If we're in the "Flip Cards" state, Flip
		if (TarotCardAnimator.GetCurrentAnimatorStateInfo(0).IsName("FlipCards"))
		{
			foreach (TarotCard tc in TarotCards)
			{
				tc.Flip();
			}
			
			Debug.Log("Flipped all cards!");

			// Set bool to get us out of this state
			TarotCardAnimator.SetBool("FlipCards", false);
		}
		
		// Select a card
		if (Input.GetMouseButton(0) && !TarotCardAnimator.GetBool("FlipCards"))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.transform != null  && hit.transform.gameObject.tag == "tarrot") // "tarrot" is the tag, though it is incorrectly spelled
				{
					hit.transform.gameObject.GetComponent<TarotCard>().Flip();
				}
			}
		}
    }
}
