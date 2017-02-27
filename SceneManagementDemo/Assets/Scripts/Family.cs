using System;
using System.Collections.Generic;

namespace Tribal
{
	public class Family
	{
		int Population;

		// TODO: Add list of raw materials
		List<RawMaterial> rawMaterials;

		// TODO: Add list of finished goods.

		// TODO: Create a property "TotalWealth" that returns the total wealth of all raw materials and finished goods

		delegate void FamilyEventListener();

		public List<MapNode> FamilyNodes {get; private set;}
		public MapNode StartingNode {get; private set; }

		public string FamilyName {get; private set;}

		event FamilyEventListener FamilyEvent;

		public Family (MapNode start)
		{
			FamilyName = Community.GetNewFamilyName();
			FamilyNodes = new List<MapNode>();
			StartingNode = start;
			FamilyNodes.Add( StartingNode );

			Community.AddFamily( this );

			SeasonTimer.SeasonEndEvent += FamilySeasonEnd;
		}

		public void AddFamilyNode( MapNode n )
		{
			if( null == n ) return;

			FamilyNodes.Add( n );
		}

		private void FamilySeasonEnd( SeasonTimer.SeasonEndEventArgs e )
		{
			if( null != FamilyEvent )
				FamilyEvent();
		}

		// TODO: Create method to add a raw material

		// TODO: Create method to add a finished good

	}
}

