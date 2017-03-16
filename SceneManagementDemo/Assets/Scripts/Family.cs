using System;
using System.Collections.Generic;
using System.Linq;

namespace Tribal
{
	public class Family
	{
		private const short STARTING_EXPERIENCE = 100;

        public int Population { get; private set; }

        public List<RawMaterial> Materials { get; private set;}
        public List<FinishedGood> FinishedGoods { get; private set; }

		public List<MapNode> FamilyNodes {get; private set;}
		public MapNode StartingNode {get; private set; }

		public string FamilyName {get; private set;}

        public List<TribalEvent> FamilyEvents { get; private set; }

        public List<Skill> Skills {get; private set;}

		public Skill CurrentActivity  {get{ return CurrentActivity; } private set{ CurrentActivity = value; ActivityProgress = 0f; } }

        public float ActivityProgress { get; private set; }

        public float Capability { 
        	get{
        		return (Skills.Sum( x => x.Experience * 0.1f ) / 100f) * Population;
        	}
        }

        public float Prosperity { 
        	get
	        {
	        	return (Capability + TotalWealth) / Population;
	        }
        }
     
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
			Skills = new List<Skill>();

            InitializeSkills();

			Community.AddFamily( this );
			SeasonTimer.SeasonEndEvent += FamilySeasonEnd;
		}

        private void InitializeSkills()
        {
        	short totalExp = STARTING_EXPERIENCE;
        	short skillExp = 0;

        	for(int i = 0; i < Enum.GetNames(typeof(Skill.SkillType)).Length; i++ )
        	{
        		Skill.SkillType typ = (Skill.SkillType)i;
        		Skill newSkill = new Skill(typ);
				newSkill.AddExperience( 10 );
				Skills.Add( newSkill );
        	}

        	for(int i = 0; i < 2; i++ )
        	{
				skillExp = (short)Community.RandomGen.Next((int)(totalExp / 2f));
				Skill s = Skills[Community.RandomGen.Next(Skills.Count)];
				s.AddExperience( skillExp );
				totalExp -= skillExp;
        	}

			for(int i = 0; i < 10; i++ )
        	{
				skillExp = (short)Community.RandomGen.Next(totalExp);
				Skill s = Skills[Community.RandomGen.Next(Skills.Count)];
				s.AddExperience( skillExp );
				totalExp -= skillExp;
				if( totalExp == 0 ) break;
        	}

        	if( totalExp > 0 )
				Skills[Community.RandomGen.Next(Skills.Count)].AddExperience( totalExp );
        }

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

