using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelBTNDestroyParent : MonoBehaviour
{
	public void destroyPanel()
	{
		Destroy(transform.parent.gameObject);
	}
}
