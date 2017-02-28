using UnityEngine;
using System.Collections;
using VRSceneManagement;
using Tribal;

[RequireComponent(typeof(BoxCollider))]
public class MapOverviewState : MonoBehaviour, SceneState {

	private int m_iSceneId;

	bool doLoad = false, doUnload = false;

	private MapNode centerNode = null;

	public GameObject [] BarrenPrefabs = null;

	public GameObject [] FamilyPrefabs = null;

	public short InitialMapSize = 1;

	private GameObject selectedObject = null;

	private float raycastDelay = 0.1f;
	private float raycastTime = 0f;

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

		raycastTime += Time.deltaTime;

		if( raycastTime > raycastDelay )
		{
			raycastTime = 0f;

			RaycastHit lk_hit;
			Vector3 origin, direction;
		
			#if UNITY_EDITOR
				Ray r = Camera.main.ScreenPointToRay( Input.mousePosition );
				origin = r.origin;
				direction = r.direction;
				//this.transform.position = r.origin;
				//this.transform.LookAt( this.transform.position + (r.direction * 1f) );
			#endif

			if( Physics.Raycast( origin, direction, out lk_hit, 200f, (1 << LayerMask.NameToLayer( "HexPad" )), QueryTriggerInteraction.Collide) )
			{
				if( lk_hit.collider.gameObject != selectedObject )
				{
					SelectionExit( selectedObject );
					SelectionEnter( lk_hit.collider.gameObject );
					selectedObject = lk_hit.collider.gameObject;
				}
				
			} else if( null != selectedObject )
				SelectionExit( selectedObject );
		}

		if( null != selectedObject )
		{
			if( Input.GetMouseButtonUp( 0 ) )
			{
				HexControlScript hControl = selectedObject.GetComponentInParent<HexControlScript>();

				if( null == hControl ) return;

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

	private void SelectionExit( GameObject other )
	{
		if( null == selectedObject ) return;

		if( other == selectedObject )
		{
			selectedObject = null;

			HexControlScript hControl = other.gameObject.GetComponentInParent<HexControlScript>();

			if( null != hControl )
				hControl.SetSelected( false );
		}
	}

	private void SelectionEnter( GameObject other )
	{
		HexControlScript hControl = other.GetComponentInParent<HexControlScript>();

		if( null != hControl )
			hControl.SetSelected( true );
		else
			return;

		selectedObject = other;
	}
}

