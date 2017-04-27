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

		if( Map.AutoSettle )
			Map.SettleMap();	
	}

	public void AddRing()
	{
		Map.Expand(1);
	}

	public void SettleMap()
	{
		Map.SettleMap();
	}
}
