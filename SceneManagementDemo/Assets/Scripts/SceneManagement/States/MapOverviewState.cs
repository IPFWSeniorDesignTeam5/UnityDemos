using UnityEngine;
using System.Collections;
using VRSceneManagement;
using Tribal;

public class MapOverviewState : MonoBehaviour, SceneState {

	private int m_iSceneId;

	bool doLoad = false, doUnload = false;

	private MapNode centerNode = null;

	public GameObject [] BarrenPrefabs = null;

	public GameObject [] FamilyPrefabs = null;

	public short InitialMapSize = 1;

	public void Awake()
	{
		if( null == BarrenPrefabs ) 
			Debug.LogError( "Null prefab node object." );

		foreach( GameObject o in BarrenPrefabs )
			Map.AddDefaultPrefab( o );	

		foreach( GameObject o in FamilyPrefabs )
			Map.AddFamilyPrefab( o );	

		centerNode = new MapNode(Map.GetRandomDefaultPrefab(), Vector3.zero, MapNodeType.Barren );

		Map.Initialize( centerNode );
		Map.Expand (InitialMapSize);
	}

	public void Load( )
	{
		Map.RenderMap();
	}

	public void Unload()
	{
		Map.DestroyMap();
	}

	void Update()
	{
		if (doLoad) {
			
			doLoad = false;
		} else if (doUnload) {
			
			doUnload = false;
		}

		RaycastHit lk_hit;

		if( Physics.Raycast( Camera.main.ScreenPointToRay( Input.mousePosition ), out lk_hit, (1 << LayerMask.NameToLayer( "HexPad" )) ) )
		{
			HexControlScript hControl = lk_hit.collider.gameObject.GetComponentInParent<HexControlScript>();
			if( null != hControl )
			{
				hControl.SetSelected( true );
			}

			if( Input.GetMouseButtonUp( 0 ) )
			{
				if( hControl.HexMapNode.NodeType == MapNodeType.Barren )
				{
					bool b_adjacentFamily = false;
					Family adjacentFamily = null;

					foreach( MapNode n in hControl.HexMapNode.Sides )
						if( null != n && (n.NodeType == MapNodeType.Family || n.NodeType == MapNodeType.Household) )
						{
							b_adjacentFamily = true;
							adjacentFamily = Community.GetFamilyByNode( n );
							adjacentFamily.AddFamilyNode( hControl.HexMapNode );
							break;
						} 

					if( b_adjacentFamily )
					{
						hControl = hControl.HexMapNode.ChangeNode( MapNodeType.Household );
						hControl.HexMapNode.gameObject.GetComponent<FamilyControlScript>().SetFamily( adjacentFamily );
					}
					else
						hControl.HexMapNode.ChangeNode( MapNodeType.Family, 1 );
				}
			}
		}
	}
}
