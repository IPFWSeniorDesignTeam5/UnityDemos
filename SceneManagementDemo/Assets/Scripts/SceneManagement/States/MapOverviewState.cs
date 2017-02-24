using UnityEngine;
using System.Collections;
using VRSceneManagement;
using Tribal;

public class MapOverviewState : MonoBehaviour, SceneState {

	private int m_iSceneId;

	bool doLoad = false, doUnload = false;

	private MapNode centerNode = null;

	public GameObject nodePrefab = null;

	public short InitialMapSize = 3;

	public void Awake()
	{
		MapNode.DefaultPrefab = nodePrefab;
	}

	public void Load( )
	{
		if( null == nodePrefab ) 
			Debug.LogError( "Null prefab node object." );

		centerNode = new MapNode(MapNode.DefaultPrefab, Vector3.zero);
		centerNode.Initialize( null );

		Map.Expand (centerNode, InitialMapSize);
	}

	public void Unload()
	{

	}

	void Update()
	{
		if (doLoad) {
			
			doLoad = false;
		} else if (doUnload) {
			
			doUnload = false;
		}
	}
}
