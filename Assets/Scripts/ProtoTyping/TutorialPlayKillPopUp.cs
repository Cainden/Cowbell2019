using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayKillPopUp : MonoBehaviour
{
	public Animator anim;

	public void killPOP()
	{

		StartCoroutine(Animate());


	}
	public IEnumerator Animate()
	{
		anim.SetBool("TutorialPopOUT", true);
		yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
		kill();
	}

	void kill()
	{
		Destroy(this.transform.parent.gameObject);
	}

}
