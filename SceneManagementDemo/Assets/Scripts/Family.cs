using System;
using System.Collections.Generic;
using System.Linq;

namespace Tribal
{
	public class Family
	{
        public int Population { get; private set; }

        public List<RawMaterial> Materials { get; private set;}
        public List<FinishedGood> FinishedGoods { get; private set; }

		public List<MapNode> FamilyNodes {get; private set;}
		public MapNode StartingNode {get; private set; }

		public string FamilyName {get; private set;}

        public List<TribalEvent> FamilyEvents { get; private set; }

        /// TODO: Add public readonly property: List<Skill> "Skills"

        /// TODO: Add public property: Skill "CurrentActivity", when changed check "ActivityStarted" value to calculate

        /// TODO: Add public readonly property: float "Capability", and add it's calculation into the "get" property method. (See tech. document )

        /// TODO: Add public readonly property: float "Prosperity", and add it's calculation into the "get" property method. (See tech. document)

        /// TODO: Add private property: float "ActivityStarted", to represent percentage of the season that had passed before season was changed.

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

        event TribalEventListener FamilyEvent;
       
		public Family (MapNode start)
		{
			FamilyName = Community.GetNewFamilyName();
			FamilyNodes = new List<MapNode>();
			StartingNode = start;
			FamilyNodes.Add( StartingNode );
			Materials  = new List<RawMaterial>();;
			FinishedGoods = new List<FinishedGood>();

            /// TODO: Call "InitializeSkills"

			Community.AddFamily( this );
			SeasonTimer.SeasonEndEvent += FamilySeasonEnd;
		}

        /// TODO: Add private method "InitializeSkills" to perform the following:
        // Add to "Skills" list a new skill for each skill type (The skill list should contain the same number of skills as there are skill types)
        //      Use the following to get the number of skill types : Enum.GetNames( typeof(Skill.SkillType) ).Length
        //      Start each skill with 10 experience.
        // Starting with 100 total skill:
        //     Perform the following twice: 
        //          Get a random number between 1 and 50 and add that amount of experience to a random skill. 
        //     Add any remaining skill out of the 100 starting total to a random skill.

        /// TODO: Add public method "GetSkillValue" that takes a Skill.SkillType as a parameter, and returns this family's experience value of that skill.

        public void AddMaterials(RawMaterialType matTyp, int count = 1)
        {
            for (int i = 0; i < count; i++ )
                Materials.Add( new RawMaterial(matTyp));
        }

        public void AddGood(FinishedGoodType goodTyp, short tier, int count = 1)
        {
            for (int i = 0; i < count; i++ )
                FinishedGoods.Add(new FinishedGood(goodTyp, tier));
        }

        public int RawMaterialCount(RawMaterialType typ)
        {
            return Materials.Where(x => x.Type == typ).Count();
        }

        public int FinishedGoodCount(FinishedGoodType typ)
        {
            return FinishedGoods.Where(x => x.Type == typ).Count();
        }

		public void AddFamilyNode( MapNode n )
		{
			if( null == n ) return;

			FamilyNodes.Add( n );
		}

		private void FamilySeasonEnd( SeasonTimer.SeasonEndEventArgs e )
		{
            /// TODO: FamilyEvents.Add( new "CalculatePopulationGrowth" )

			if( null != FamilyEvent )
                FamilyEvent(FamilyEvents);
		}

        /// TODO: Add private method "CalculatePopulationGrowth" that will return a TribalEvent representing this family's population growth based on calculation in tech. document.

    }
}

