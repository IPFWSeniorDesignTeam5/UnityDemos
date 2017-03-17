using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tribal
{
	public enum MapNodeType {
		Barren = 0,
		SeasonTimer,
		Household,
		Family
	}
	
	public class MapNode
	{
		public  GameObject gameObject {get; private set;}
		private  GameObject m_Prefab;
		public  MapNode [] Sides {get; private set;}
		private  Vector3 m_Position;
		private  Quaternion m_Rotation;

		public short PrefabVariation {get; private set;}

		public MapNodeType NodeType { get; private set; }

		public void DestroyGameObject()
		{
			if( null != gameObject )
				GameObject.Destroy( gameObject );

			gameObject = null;
		}

		public MapNode( GameObject obj, Vector3 pos, MapNodeType type )
		{
			NodeType = type;

			m_Prefab = obj;

			int angle = TribeControl.RandomGen.Next(0, 7);

			m_Position = pos;
			Sides = new MapNode[6];
			m_Rotation = Quaternion.AngleAxis( 60f * angle, Vector3.up );
			PrefabVariation = 0;

			Map.AddMapNode( this );

			InstantiatePrefab();
		}

		public HexControlScript ChangeNode( MapNodeType newType, short variation = 0 )
		{
			if( newType == NodeType && variation == PrefabVariation )
				return gameObject.GetComponent<HexControlScript>();

			DestroyGameObject();

			switch( newType )
			{
				case MapNodeType.Barren:
					m_Prefab = Map.GetRandomDefaultPrefab();
				break;

				case MapNodeType.Family:
					m_Prefab = Map.GetFamilyPrefab( variation );
					new Family(this);
				break;

				case MapNodeType.Household:
					m_Prefab = Map.GetFamilyPrefab( variation );
				break;
			}

			PrefabVariation = variation;
			NodeType = newType;
			InstantiatePrefab();

			return gameObject.GetComponent<HexControlScript>();
		}

		public void InstantiatePrefab()
		{
			if( null == gameObject && null != m_Prefab )
			{
				gameObject = GameObject.Instantiate( m_Prefab, m_Position, m_Rotation);
				HexControlScript hControl = gameObject.GetComponent<HexControlScript>();

				if( null != hControl )
				{
					hControl.SetMapNode( this );
					Map.Settle(gameObject, 0f);
				}
				else
					Debug.LogError ( "Null HexControlScript on prefab." );

				ResolvePrefabScriptReferences();
			}
		}

		private void ResolvePrefabScriptReferences()
		{
			switch( NodeType )
			{
				case MapNodeType.Family:
				case MapNodeType.Household:
					FamilyControlScript fControl = gameObject.GetComponent<FamilyControlScript>();

					if( null != fControl )
						fControl.SetFamily( Community.GetFamilyByNode(this) );
				break;
				default: break;
			}
		}

		public void Initialize( MapNode [] sides)
		{
			if( null == sides ) return;

			for( short i = 0; i < 6; i++ )
			{
				if( null != sides[i] ) 
				{
					Sides[i] = sides[i];
					sides[i].SetSide( LinkedSide(i), this );
				}
			}
		}

		short LinkedSide( short s )
		{
			switch( s )
			{
			case 0: 	return 3;
			case 1: 	return 4;
			case 2:		return 5;
			case 3:		return 0;
			case 4:		return 1;
			case 5:		return 2;
			default:	return 0;
			}
		}

		public void SetSide( short side, MapNode node )
		{
			Sides[side] = node;
		}

		public List<MapNode> Expand()
		{
			bool [] lbz_new = new bool[] { false, false, false, false, false, false };

			if( null == Sides[0] ) {
				Sides[0] = new MapNode(Map.GetRandomDefaultPrefab(), new Vector3( m_Position.x + 7.5f, m_Position.y, m_Position.z + 4.3f ), MapNodeType.Barren );
				lbz_new[0] = true;
			}
			if( null == Sides[1] ) {
				Sides[1] = new MapNode(Map.GetRandomDefaultPrefab(), new Vector3( m_Position.x + 7.5f, m_Position.y, m_Position.z - 4.3f ), MapNodeType.Barren );
				lbz_new[1] = true;
			}
			if( null == Sides[2] ) {
				Sides[2] = new MapNode(Map.GetRandomDefaultPrefab(), new Vector3( m_Position.x, m_Position.y, m_Position.z - 8.66f ), MapNodeType.Barren );
				lbz_new[2] = true;
			}
			if( null == Sides[3] ) {
				Sides[3] = new MapNode(Map.GetRandomDefaultPrefab(), new Vector3( m_Position.x - 7.5f, m_Position.y, m_Position.z - 4.3f ), MapNodeType.Barren );
				lbz_new[3] = true;
			}
			if( null == Sides[4] ) {
				Sides[4] = new MapNode(Map.GetRandomDefaultPrefab(), new Vector3( m_Position.x - 7.5f, m_Position.y, m_Position.z + 4.3f ), MapNodeType.Barren );
				lbz_new[4] = true;
			}
			if( null == Sides[5] ) {
				Sides[5] = new MapNode(Map.GetRandomDefaultPrefab(), new Vector3( m_Position.x, m_Position.y, m_Position.z + 8.66f ), MapNodeType.Barren );
				lbz_new[5] = true;
			}

			if( lbz_new[0] )
				Sides[0].Initialize( new MapNode[] { null, null, Sides[1], this, Sides[5], null } );
			
			if( lbz_new[1] )
				Sides[1].Initialize( new MapNode[] { null, null, null, Sides[2], this, Sides[0] } );
			
			if( lbz_new[2] ) 
				Sides[2].Initialize( new MapNode[] { Sides[1], null, null, null, Sides[3], this } );
			
			if( lbz_new[3] )
				Sides[3].Initialize( new MapNode[] { this, Sides[2], null, null, null, Sides[4] } );
			
			if( lbz_new[4] ) 
				Sides[4].Initialize( new MapNode[] { Sides[5], this, Sides[3], null, null, null } );
			
			if( lbz_new[5] ) 
				Sides[5].Initialize( new MapNode[] { null, Sides[0], this, Sides[4], null, null } );

			List<MapNode> returnNew = new List<MapNode> ();

			for (int i = 0; i < 6; i++) {
				if (lbz_new [i])
					returnNew.Add (Sides [i]);
			}

			return returnNew;
		}
	}

	public static class Map  {
		public static List<MapNode> Nodes {get; private set;}
		private static List<MapNode> m_OuterNodes = new List<MapNode>();
		private static List<GameObject> DefaultPrefabs  = new List<GameObject>();
		private static List<GameObject> FamilyPrefabs = new List<GameObject>();

		public static MapNode m_CenterNode { get; private set;}

		static Map ()
		{
			Nodes = new List<MapNode>();
		}

		public static void Initialize( MapNode centerNode )
		{
			m_CenterNode = centerNode;
		}

		public static void AddMapNode( MapNode n )
		{
			Nodes.Add( n );
		}

		public static void AddDefaultPrefab( GameObject def )
		{
			if( null != def && !DefaultPrefabs.Contains(def ) )
				DefaultPrefabs.Add(def);
		}

		public static GameObject GetRandomDefaultPrefab()
		{
			if( DefaultPrefabs.Count == 0 ) return null;

			short i = (short)TribeControl.RandomGen.Next( 0, DefaultPrefabs.Count - 1 );

			return DefaultPrefabs[i];
		}

		public static void AddFamilyPrefab( GameObject fam )
		{
			if( null != fam && !FamilyPrefabs.Contains( fam ) )
				FamilyPrefabs.Add( fam );
		}

		public static GameObject GetFamilyPrefab( short homeLevel )
		{
			if( homeLevel > FamilyPrefabs.Count )
			{
				Debug.LogError( "Requested home level family prefab above available family prefab count." );
				return null;
			}

			return FamilyPrefabs[homeLevel];
		}

		public static void Expand( short addRings )
		{
			if( null == m_CenterNode )
				Debug.LogError( "Null center map node. (Map not initialized)." );

			List<MapNode> newNodes = null;
			List<MapNode> nextNodes = new List<MapNode> ();

			if( m_OuterNodes.Count > 0 )
				newNodes = m_OuterNodes;
			else
				newNodes = m_CenterNode.Expand();

			for( int i = 0; i < addRings; i++ )
			{
				nextNodes.Clear ();

				foreach (MapNode newNode in newNodes)
					nextNodes.InsertRange (nextNodes.Count, newNode.Expand ());

				newNodes.Clear();
				newNodes.InsertRange (0, nextNodes);
			}

			m_OuterNodes = nextNodes;

			if (Nodes.Count > 0)
				Debug.Log ("Total Map Nodes: " + Nodes.Count);
		}

		public static void DestroyMap()
		{
			foreach( MapNode n in Nodes )
				n.DestroyGameObject();
		}

		public static void RenderMap()
		{
			foreach( MapNode n in Nodes )
				n.InstantiatePrefab();
		}

		public static void SettleMap()
		{
			foreach( MapNode n in Nodes )
				Map.Settle(n.gameObject, 0f);
		}

		public static void Settle(GameObject obj, float distance_above_terrain = 0f, bool settleChildren = true, bool orientToNormal = false )
		{
			const float OFFSET_ABOVE = 100f;
			RaycastHit hit;

			if( !Physics.Raycast( obj.transform.position + Vector3.up * OFFSET_ABOVE, -Vector3.up, out hit, 300f, (1 << LayerMask.NameToLayer( "Terrain" ) ) ) )
			{
				Debug.LogWarning( "Failed to raycast object to terrain." );
				return;
			}

			obj.transform.position = (obj.transform.position + Vector3.up * OFFSET_ABOVE) + (-Vector3.up * hit.distance) + (Vector3.up * distance_above_terrain);

			Quaternion rot = Quaternion.FromToRotation( obj.transform.up, hit.normal );

			if( orientToNormal )
				obj.transform.Rotate( rot.eulerAngles );

			if( !settleChildren ) return;

			foreach( Transform o in obj.gameObject.transform )
			{
				switch( o.gameObject.tag )
				{
					case "HexOutline":	// Every vertex of the mesh will be settled
					case "SettleMesh":
						SettleMesh( o.gameObject );
					break;
					case "OffsetSettle": // Move only the distance parent moves
						
					break;
					default:
					if( !Physics.Raycast( o.position + Vector3.up * OFFSET_ABOVE, -Vector3.up, out hit, 300f, (1 << LayerMask.NameToLayer( "Terrain" ) ) ) )
						{
							Debug.LogWarning( "Failed to raycast object to terrain." );
							return;
						} else
						{
							o.position = (o.position + Vector3.up * OFFSET_ABOVE) + (-Vector3.up * hit.distance) + (Vector3.up * distance_above_terrain);

							if( orientToNormal )
							{
								rot = Quaternion.FromToRotation( o.transform.up, hit.normal );
								o.transform.Rotate( rot.eulerAngles );
							}
						}
					break;
				}
			}
		}

		private static void SettleMesh( GameObject obj, float distance_above_terrain = 0.02f )
		{
			const float OFFSET_ABOVE = 100f;

			RaycastHit hit;
			MeshFilter meshFilter = obj.gameObject.GetComponent<MeshFilter>();

			if( null == meshFilter )
			{
				Debug.LogWarning( "Failed to find mesh filter on gameobject." );
				return;
			}

			Mesh mesh = meshFilter.mesh;
			Vector3[] verts = mesh.vertices;
			int i = 0;

			while( i < verts.Length )
			{
				if( !Physics.Raycast( obj.transform.TransformPoint(verts[i]) + Vector3.up * OFFSET_ABOVE, -Vector3.up, out hit, 300f, (1 << LayerMask.NameToLayer( "Terrain" ) ) ) )
				{
					Debug.LogWarning( "Failed to raycast object to terrain." );
					return;
				}

				verts[i] = obj.transform.InverseTransformPoint((obj.transform.TransformPoint(verts[i]) + Vector3.up * OFFSET_ABOVE) + (-Vector3.up) * (hit.distance - distance_above_terrain));
				i++;
			}

			mesh.vertices = verts;
			mesh.RecalculateBounds();
		}
	}
}