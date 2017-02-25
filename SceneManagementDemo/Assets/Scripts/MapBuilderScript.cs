using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tribal;

public class MapBuilderScript : MonoBehaviour {

	public void DestroyMap()
	{
		Map.DestroyMap();
	}

	public void CreateMap()
	{
		Map.RenderMap();
	}

	public void AddRing()
	{
		Map.Expand(1);
	}
}
