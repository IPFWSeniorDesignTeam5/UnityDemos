using UnityEngine;
using System.Collections;
using VRSceneManagement;

public class MapOverviewState : MonoBehaviour, SceneState {

	private int m_iSceneId;

	bool doLoad = false, doUnload = false;

	private MapNode centerNode = null;

	public GameObject nodePrefab = null;

	private class MapNode
	{
		private MapNode [] m_Sides;
		private GameObject m_kPrefab;
		private Vector3 m_Position;

		public MapNode()
		{
			m_Sides = new MapNode[6];
		}

		public void Initialize(GameObject prefab, Vector3 position, MapNode [] sides)
		{
			m_Position = position;
			m_kPrefab = prefab;

			if( null == sides ) return;

			for( short i = 0; i < 6; i++ )
			{
				if( null == m_Sides[i] )
					m_Sides[i] = sides[i];
			}
		}

		public void SetSide( short side, MapNode node )
		{
			m_Sides[side] = node;
		}

		public GameObject Render( int radius )
		{
			GameObject lk_return = GameObject.Instantiate( m_kPrefab, m_Position, new Quaternion( 0f, 0f, 0f, 0f) );

			if( radius > 0 )
			{
				bool [] lbz_new = new bool[] { false, false, false, false, false, false };

				if( null == m_Sides[0] ) {
					m_Sides[0] = new MapNode();
					lbz_new[0] = true;
				}
				if( null == m_Sides[1] ) {
					m_Sides[1] = new MapNode();
					lbz_new[1] = true;
				}
				if( null == m_Sides[2] ) {
					m_Sides[2] = new MapNode();
					lbz_new[2] = true;
				}
				if( null == m_Sides[3] ) {
					m_Sides[3] = new MapNode();
					lbz_new[3] = true;
				}
				if( null == m_Sides[4] ) {
					m_Sides[4] = new MapNode();
					lbz_new[4] = true;
				}
				if( null == m_Sides[5] ) {
					m_Sides[5] = new MapNode();
					lbz_new[5] = true;
				}

				if( lbz_new[0] )
				{
					m_Sides[0].Initialize( m_kPrefab, new Vector3( m_Position.x + 7.5f, m_Position.y, m_Position.z + 4.3f ), new MapNode[] { null, null, m_Sides[1], this, m_Sides[5], null } );
					if( null != m_Sides[5] ) m_Sides[5].SetSide( 1, m_Sides[0] );
					if( null != m_Sides[1] ) m_Sides[1].SetSide( 5, m_Sides[0] );
				}
				if( lbz_new[1] )
				{
					m_Sides[1].Initialize( m_kPrefab, new Vector3( m_Position.x + 7.5f, m_Position.y, m_Position.z - 4.3f ), new MapNode[] { null, null, null, m_Sides[2], this, m_Sides[0] } );
					if( null != m_Sides[0] ) m_Sides[0].SetSide( 2, m_Sides[1] );
					if( null != m_Sides[2] ) m_Sides[2].SetSide( 0, m_Sides[1] );
				}	
				if( lbz_new[2] ) {
					m_Sides[2].Initialize( m_kPrefab, new Vector3( m_Position.x, m_Position.y, m_Position.z - 8.66f ), new MapNode[] { m_Sides[1], null, null, null, m_Sides[3], this } );
					if( null != m_Sides[3] ) m_Sides[3].SetSide( 1, m_Sides[2] );
					if( null != m_Sides[1] ) m_Sides[1].SetSide( 3, m_Sides[2] ); 
				}
				if( lbz_new[3] ){
					m_Sides[3].Initialize( m_kPrefab, new Vector3( m_Position.x - 7.5f, m_Position.y, m_Position.z - 4.3f ), new MapNode[] { this, m_Sides[2], null, null, null, m_Sides[4] } );
					if( null != m_Sides[4] ) m_Sides[4].SetSide( 2, m_Sides[3] );
					if( null != m_Sides[2] ) m_Sides[2].SetSide( 4, m_Sides[3] );
				}
				if( lbz_new[4] ) {
					m_Sides[4].Initialize( m_kPrefab, new Vector3( m_Position.x - 7.5f, m_Position.y, m_Position.z + 4.3f ), new MapNode[] { m_Sides[5], this, m_Sides[3], null, null, null } );
					if( null != m_Sides[3] ) m_Sides[3].SetSide( 5, m_Sides[4] );
					if( null != m_Sides[5] ) m_Sides[5].SetSide( 3, m_Sides[4] );
				}
				if( lbz_new[5] ) {
					m_Sides[5].Initialize( m_kPrefab, new Vector3( m_Position.x, m_Position.y, m_Position.z + 8.66f ), new MapNode[] { null, m_Sides[0], this, m_Sides[4], null, null } );
					if( null != m_Sides[4] ) m_Sides[4].SetSide( 0, m_Sides[5] );
					if( null != m_Sides[0] ) m_Sides[0].SetSide( 4, m_Sides[5] );
				}

				for( int i = 0; i < 6; i++ )
					if( lbz_new[i] ) m_Sides[i].Render( radius - 1 );
			}

			return lk_return;
		}

	}

	public void Load( )
	{
		if( null == nodePrefab ) 
			Debug.LogError( "Null prefab node object." );

		centerNode = new MapNode();
		centerNode.Initialize( nodePrefab, Vector3.zero, null );

		centerNode.Render( 2 );
	}

	public void Unload()
	{

	}

	void Update()
	{
		if (doLoad) {

		} else if (doUnload) {
			
		}
	}
}
