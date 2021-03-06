using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using VRSceneManagement;

using Tribal;
using UnityEngine.UI;

public class MapOverviewState : MonoBehaviour, SceneState {

	public bool AutoSettleMap = true;

	public float CameraHeightPerHouse = 0.025f;
    
	bool loaded = false;

	private MapNode centerNode = null;

	public GameObject [] BarrenPrefabs = null;

	public GameObject [] FamilyPrefabs = null;

	public GameObject SeasonTimerPrefab = null;

	public short InitialMapSize = 1;

    public Canvas FamilyInfoCanvas = null;
    FamilyInfoControlScript famInfo = null;

	public float FamilyInfoSmoothTime = 0.1f;

	public float UISelectedScale = 1f;
	public float UIDeselectedScale = 4f;

	private bool placingHousehold = true;

	private List<GameObject> selections = new List<GameObject>();

	public void Awake()
	{
		if( null == BarrenPrefabs ) 
			Debug.LogError( "Null prefab node object." );

		foreach( GameObject o in BarrenPrefabs )
			Map.AddDefaultPrefab( o );	

		foreach( GameObject o in FamilyPrefabs )
			Map.AddFamilyPrefab( o );

		if( null != FamilyInfoCanvas )
			famInfo = FamilyInfoCanvas.gameObject.GetComponent<FamilyInfoControlScript>();
	}

	public void Load( )
	{
        if (null == Map.m_CenterNode)
        {
			centerNode = new MapNode(SeasonTimerPrefab, Vector3.zero, MapNodeType.SeasonTimer);
			Map.AutoSettle = AutoSettleMap;
            Map.Initialize(centerNode);
            Map.Expand(InitialMapSize);
        } else 
			centerNode = Map.m_CenterNode;

		CalculateCameraHeight();
		Map.RenderMap();
        loaded = true;
	}

	public void Unload()
	{
		Map.DestroyMap();
        loaded = false;
	}

	void Update()
	{
        if (!loaded) return;

		RaycastHit lk_hit = new RaycastHit();
		GameObject UIHit = null;
		GameObject check = null;

		Map.AutoSettle = AutoSettleMap;

		CheckFamilySelection();

		if( TribeControl.RaycastAll( ref lk_hit, ref UIHit, (1 << LayerMask.NameToLayer( "HexPad" )) ) )
		{
			if( null != lk_hit.collider )
			{
				check = SelectedMapNode();
				if( lk_hit.collider.gameObject != check )
				{
					SelectionExit( check );
					SelectionEnter( lk_hit.collider.gameObject );
				}
			} else
				ClearSelections( "HexPad" );

			if( null != UIHit )
			{
				check = SelectedUI();
				if( check != UIHit )
				{
					SelectionExit( check );
					SelectionEnter( UIHit );
				}
			} else
				ClearSelections( "UI" );

		} else 
		{
			foreach( var obj in selections )
				SelectionExit( obj, false );

			selections.Clear();

			return;
		}

		placingHousehold = true;

		if( placingHousehold )
		{
			if( DoPlaceHousehold() )
			{
				CalculateCameraHeight();
				placingHousehold = false;
			}
		}
		else
			DoSelectActivity();
	}

	private bool DoSelectActivity()
	{
		GameObject selectedObject = SelectedUI();

		if( null != selectedObject )
		{
			if( Input.GetMouseButtonUp( 0 ) || Input.GetButtonUp( "Fire1" ) || Input.GetButtonUp( "Submit" ))
			{
				// Activity Selected
				HexControlScript hControl = selectedObject.GetComponentInParent<HexControlScript>();
				FamilyControlScript fControl = hControl.HexMapNode.gameObject.GetComponent<FamilyControlScript>();

				if( fControl.FamilyInfo.StartingNode == hControl.HexMapNode )	// Starting Family Node
					placingHousehold = true;
				else
				{
					// TODO: Change household activity
				}
				return true;
			}
		}

		return false;
	}

    private void CheckFamilySelection()
    {
        GameObject lk_selected = SelectedMapNode();
        if (null == lk_selected)
        {
            DoFamilyDeselect();
            return;
        }

        FamilyControlScript lk_fam = lk_selected.GetComponent<FamilyControlScript>();
        if (null == lk_fam)
        {
            DoFamilyDeselect();
            return;
        }

        DoFamilySelect(lk_fam.FamilyInfo);        
    }

    private void DoFamilyDeselect()
    {
        if (null == FamilyInfoCanvas) return;

		Vector3 vel = Vector3.zero, dest = new Vector3( UIDeselectedScale, UIDeselectedScale, UIDeselectedScale);

		if( Vector3.Distance( FamilyInfoCanvas.transform.localScale, dest ) > 0.001f )
		{
			FamilyInfoCanvas.transform.localScale = Vector3.SmoothDamp( FamilyInfoCanvas.transform.localScale, dest, ref vel, FamilyInfoSmoothTime );
			if( Vector3.Distance( FamilyInfoCanvas.transform.localScale, dest ) <= 0.001f )
			{
				FamilyInfoCanvas.transform.localScale = dest;
				famInfo.SetFamily( null );
			}
		}
    }

    private void DoFamilySelect( Family lk_fam )
    {
        if (null == FamilyInfoCanvas) return;

		Vector3 vel = Vector3.zero, dest = new Vector3( UISelectedScale, UISelectedScale, UISelectedScale);

		if( Vector3.Distance( FamilyInfoCanvas.transform.localScale, dest ) > 0.001f )
		{
			FamilyInfoCanvas.transform.localScale = Vector3.SmoothDamp( FamilyInfoCanvas.transform.localScale, dest, ref vel, FamilyInfoSmoothTime );
			if( Vector3.Distance( FamilyInfoCanvas.transform.localScale, dest ) <= 0.001f )
			{
				FamilyInfoCanvas.transform.localScale = dest;
			}
		}

		famInfo.SetFamily( lk_fam );
    }

	private bool DoPlaceHousehold()
	{
		GameObject selectedObject = SelectedMapNode();

		if( null != selectedObject )
		{
			if( Input.GetMouseButtonUp( 0 ) || Input.GetButtonUp( "Fire1" ) || Input.GetButtonUp( "Submit" ))
			{
				HexControlScript hControl = selectedObject.GetComponentInParent<HexControlScript>();

				if( null == hControl ) return false;

				selections.Remove( selectedObject );

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

					FamilyControlScript fControl = null;

					if( b_adjacentFamily )
					{
						hControl = hControl.HexMapNode.ChangeNode( MapNodeType.Household );
						fControl = hControl.HexMapNode.gameObject.GetComponent<FamilyControlScript>();
						fControl.CurrentActivity = (Skill.SkillType)TribeControl.RandomGen.Next(1,5);
						fControl.SetFamily( adjacentFamily );
					}
					else
					{
						hControl = hControl.HexMapNode.ChangeNode( MapNodeType.Family, 1 );
						fControl = hControl.HexMapNode.gameObject.GetComponent<FamilyControlScript>();
						fControl.CurrentActivity = Skill.SkillType.Hunting;
					}

					return true;
				}
			}
		}

		return false;
	}

	private void ClearSelections( string layerName )
	{
		List<GameObject> rem = new List<GameObject>();
		foreach( var obj in selections )
			if( LayerMask.LayerToName(obj.layer) == layerName )
				rem.Add( obj );

		foreach( var obj in rem )
		{
			SelectionExit( obj, false );
			selections.Remove( obj );
		}
	}

	private GameObject SelectedUI()
	{
		if( selections.Count == 0 ) return null;

		foreach( var obj in selections )
			if( LayerMask.LayerToName(obj.layer) == "UI" )
				return obj;

		return null;
	}

	private GameObject SelectedMapNode()
	{
		if( selections.Count == 0 ) return null;

		foreach( var obj in selections )
			if( LayerMask.LayerToName(obj.layer) == "HexPad" )
				return obj;

		return null;
	}

	private void SelectionExit( GameObject other, bool removeFromList = true )
	{
		if( !selections.Contains(other) ) return;

		switch( other.tag ) 
		{
			case "MapNode" :
				HexControlScript hControl = other.gameObject.GetComponentInParent<HexControlScript>();

			if( null != hControl )
				hControl.SetSelected( false );
			break;

			case "HomeActivity":
				Button but = other.GetComponent<Button>();
				Image img = other.GetComponent<Image>();

				if( null != but )
				{
					img.sprite = but.spriteState.disabledSprite;
				}
			break;
		}

		if( removeFromList )
			selections.Remove( other );
	}

	private void SelectionEnter( GameObject other )
	{
		if( null == other ) return;

		switch( other.tag ) 
			{
				case "MapNode" :
						HexControlScript hControl = other.GetComponentInParent<HexControlScript>();

					if( null != hControl )
					{
						if( (hControl.HexMapNode.NodeType == MapNodeType.Family || hControl.HexMapNode.NodeType == MapNodeType.Household) || placingHousehold )
							hControl.SetSelected( true );
					}
					else
						return;
				break;

				case "HomeActivity":
					Button but = other.GetComponent<Button>();
					Image img = other.GetComponent<Image>();
					if( null != but )
					{
						img.sprite = but.spriteState.highlightedSprite;
					}
				break;
			}

		selections.Add( other );
	}

	public void CalculateCameraHeight()
	{
		Transform cam = GetComponentInParent<Transform>();
		Vector3 newPos = cam.position;
		newPos.y += (CameraHeightPerHouse * Map.Nodes.FindAll( x => x.NodeType == MapNodeType.Family || x.NodeType == MapNodeType.Household ).Count);
		cam.position = newPos;
	}
}

