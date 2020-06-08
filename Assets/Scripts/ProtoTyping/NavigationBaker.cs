using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class NavigationBaker : MonoBehaviour
{
	public List<NavMeshSurface> surfaces = new List<NavMeshSurface>();
 
	public void addToNavmesh(NavMeshSurface a)
	{
		
		surfaces.Add(a);
		a.BuildNavMesh();
	}
}
