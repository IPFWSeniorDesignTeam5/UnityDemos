using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tribal
{
	public class MapNode
	{
		private GameObject m_Object;
		private  MapNode [] m_Sides;
		private Vector3 m_Position;

		public static GameObject DefaultPrefab;

		public MapNode( GameObject obj, Vector3 pos )
		{
			int angle = Random.Range (0, 7);

			m_Position = pos;
			m_Object = GameObject.Instantiate( obj, pos, Quaternion.AngleAxis( 60f * angle, Vector3.up ));
			m_Sides = new MapNode[6];	
		}

		public void Initialize( MapNode [] sides)
		{
			if( null == sides ) return;

			for( short i = 0; i < 6; i++ )
			{
				if( null != sides[i] && null == m_Sides[i] ) 
				{
					m_Sides[i] = sides[i];
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
			m_Sides[side] = node;
		}

		public List<MapNode> Expand()
		{
			bool [] lbz_new = new bool[] { false, false, false, false, false, false };

			if( null == m_Sides[0] ) {
				m_Sides[0] = new MapNode(DefaultPrefab, new Vector3( m_Position.x + 7.5f, m_Position.y, m_Position.z + 4.3f ));
				lbz_new[0] = true;
			}
			if( null == m_Sides[1] ) {
				m_Sides[1] = new MapNode(DefaultPrefab, new Vector3( m_Position.x + 7.5f, m_Position.y, m_Position.z - 4.3f ));
				lbz_new[1] = true;
			}
			if( null == m_Sides[2] ) {
				m_Sides[2] = new MapNode(DefaultPrefab, new Vector3( m_Position.x, m_Position.y, m_Position.z - 8.66f ));
				lbz_new[2] = true;
			}
			if( null == m_Sides[3] ) {
				m_Sides[3] = new MapNode(DefaultPrefab, new Vector3( m_Position.x - 7.5f, m_Position.y, m_Position.z - 4.3f ));
				lbz_new[3] = true;
			}
			if( null == m_Sides[4] ) {
				m_Sides[4] = new MapNode(DefaultPrefab, new Vector3( m_Position.x - 7.5f, m_Position.y, m_Position.z + 4.3f ));
				lbz_new[4] = true;
			}
			if( null == m_Sides[5] ) {
				m_Sides[5] = new MapNode(DefaultPrefab, new Vector3( m_Position.x, m_Position.y, m_Position.z + 8.66f ));
				lbz_new[5] = true;
			}

			if( lbz_new[0] )
				m_Sides[0].Initialize( new MapNode[] { null, null, m_Sides[1], this, m_Sides[5], null } );
			
			if( lbz_new[1] )
				m_Sides[1].Initialize( new MapNode[] { null, null, null, m_Sides[2], this, m_Sides[0] } );
			
			if( lbz_new[2] ) 
				m_Sides[2].Initialize( new MapNode[] { m_Sides[1], null, null, null, m_Sides[3], this } );
			
			if( lbz_new[3] )
				m_Sides[3].Initialize( new MapNode[] { this, m_Sides[2], null, null, null, m_Sides[4] } );
			
			if( lbz_new[4] ) 
				m_Sides[4].Initialize( new MapNode[] { m_Sides[5], this, m_Sides[3], null, null, null } );
			
			if( lbz_new[5] ) 
				m_Sides[5].Initialize( new MapNode[] { null, m_Sides[0], this, m_Sides[4], null, null } );

			List<MapNode> returnNew = new List<MapNode> ();

			for (int i = 0; i < 6; i++) {
				if (lbz_new [i])
					returnNew.Add (m_Sides [i]);
			}

			return returnNew;
		}
	}

	public static class Map  {
		public static void Expand( MapNode node, short addRings )
		{
			MapNode n = node;
			List<MapNode> newNodes = null;
			List<MapNode> nextNodes = new List<MapNode> ();

			newNodes = node.Expand();

			for( int i = 0; i < addRings; i++ )
			{
				nextNodes.Clear ();

				foreach (MapNode newNode in newNodes)
					nextNodes.InsertRange (0, newNode.Expand ());

				newNodes.Clear();
				newNodes.InsertRange (0, nextNodes);
			}
		}
	}
}