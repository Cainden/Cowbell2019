using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SendToNavBaker : MonoBehaviour
{
	NavigationBaker navBake;
	
    // Start is called before the first frame update
    void Start()
    {
		navBake = GameObject.FindGameObjectWithTag("NavBaker").GetComponent<NavigationBaker>();
		navBake.addToNavmesh(this.gameObject.GetComponent<NavMeshSurface>());



    }

 
}
