using System;
using System.Collections.Generic;

namespace Tribal
{
	public class Family
	{
		int Population;

        public List<RawMaterial> Materials { get; private set;}
        public List<FinishedGood> FinishedGoods { get; private set; }

		public List<MapNode> FamilyNodes {get; private set;}
		public MapNode StartingNode {get; private set; }

		public string FamilyName {get; private set;}

        public int TotalWealth
        {
            get
            {
                int total = 0;

                foreach( RawMaterial r in Materials )
                	total += r.WealthValue;

                foreach( FinishedGood g in FinishedGoods )
                	total += g.WealthValue;

                return total;
            }
            private set { TotalWealth = value; }
        }

        delegate void FamilyEventListener();

		event FamilyEventListener FamilyEvent;

		public Family (MapNode start)
		{
			FamilyName = Community.GetNewFamilyName();
			FamilyNodes = new List<MapNode>();
			StartingNode = start;
			FamilyNodes.Add( StartingNode );
			Materials  = new List<RawMaterial>();;
			FinishedGoods = new List<FinishedGood>();

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
        
        public void addRaw(RawMaterial rawMaterial)
        {
            Materials.Add(rawMaterial);
        }
        public void AddFinishedGood(FinishedGood finishedMaterial)
        {
            FinishedGoods.Add(finishedMaterial);
        }

    }
}

