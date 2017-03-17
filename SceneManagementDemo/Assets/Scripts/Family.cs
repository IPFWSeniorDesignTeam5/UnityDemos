using System;
using System.Collections.Generic;
using System.Linq;

namespace Tribal
{
	public class Family
	{
		private const short STARTING_EXPERIENCE = 100;

        public float Population { get; private set; }

        public List<RawMaterial> Materials { get; private set;}
        public List<FinishedGood> FinishedGoods { get; private set; }

		public List<MapNode> FamilyNodes {get; private set;}
		public MapNode StartingNode {get; private set; }

		public string FamilyName {get; private set;}

        public List<TribalEvent> FamilyEvents { get; private set; }

        public List<Skill> Skills {get; private set;}

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
			FamilyEvents = new List<TribalEvent>();

			Population = TribeControl.RandomGen.Next( 4, 10 );

            InitializeSkills();

			Community.AddFamily( this );
			FamilyEvent += ProcessFamilyEvents;
			SeasonTimer.SeasonEndEvent += FamilySeasonEnd;
		}

        private void InitializeSkills()
        {
        	short totalExp = STARTING_EXPERIENCE;
        	short skillExp = 0;

        	for(int i = 1; i < Enum.GetNames(typeof(Skill.SkillType)).Length; i++ )
        	{
        		Skill.SkillType typ = (Skill.SkillType)i;
        		Skill newSkill = new Skill(typ);
				newSkill.AddExperience( 10 );
				Skills.Add( newSkill );
        	}

        	for(int i = 0; i < 2; i++ )
        	{
				skillExp = (short)TribeControl.RandomGen.Next((int)(totalExp / 2f));
				Skill s = Skills[TribeControl.RandomGen.Next(Skills.Count)];
				s.AddExperience( skillExp );
				totalExp -= skillExp;
        	}

			for(int i = 0; i < 10; i++ )
        	{
				skillExp = (short)TribeControl.RandomGen.Next(totalExp);
				Skill s = Skills[TribeControl.RandomGen.Next(Skills.Count)];
				s.AddExperience( skillExp );
				totalExp -= skillExp;
				if( totalExp == 0 ) break;
        	}

        	if( totalExp > 0 )
				Skills[TribeControl.RandomGen.Next(Skills.Count)].AddExperience( totalExp );
        }

        public float GetSkillValue( Skill.SkillType typ )
        {
        	return Skills.Where( x => x.Type == typ ).First().Experience;
        }

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

        public float BonusYield( Skill.SkillType typ )
        {
        	// TODO: Calculate bonus yield
        	return 0f;
        }

		public void AddFamilyNode( MapNode n )
		{
			if( null == n ) return;

			FamilyNodes.Add( n );
		}

		private void FamilySeasonEnd( SeasonTimer.SeasonEndEventArgs e )
		{
           FamilyEvents.Add( CalculatePopulationGrowth() );
           TribalEvent rawMats = CalculateRawMaterialYield();

           if( null != rawMats ) FamilyEvents.Add( rawMats );

			if( null != FamilyEvent )
                FamilyEvent(FamilyEvents);
		}

		private void ProcessFamilyEvents( List<TribalEvent> events )
		{
			List<TribalEvent> expired = new List<TribalEvent>();

			foreach( TribalEvent evt in events )
			{
				foreach( StatusModifier mod in evt.StatusModifiers )
				{
					switch( mod.StatusEffect.target )
					{
						case StatusModifier.Effect.Target.Property:
							switch( mod.Description.ToLower() )
							{
								case "population":
									Population += mod.Value;
								break;
							}
						break;
						case StatusModifier.Effect.Target.RawMaterial:
							string [] names = Enum.GetNames(typeof(RawMaterialType));

							RawMaterialType matTyp = RawMaterialType.Food;

							for( int i = 0; i < names.Length; i++ )
							{
								if( names[i].ToLower() == mod.Description.ToLower() )
									matTyp = (RawMaterialType)i;
							}

							AddMaterials( matTyp, (int)mod.Value );
						break;
					}
				}

				evt.SeasonsAffected--;
				if( evt.SeasonsAffected == 0 )
					expired.Add(evt);
			}

			foreach( TribalEvent evt in expired )
				FamilyEvents.Remove( evt );
		}
			
		private TribalEvent CalculatePopulationGrowth()
		{
			float value = FamilyNodes.Count * Prosperity * 0.5f;

			return new TribalEvent ("Season Population Change", 
									1, 
									new StatusModifier(
										new StatusModifier.Effect(
											StatusModifier.Effect.Modifier.Amount, 
											StatusModifier.Effect.Target.Property
										), 
									"Population", 
									value));
		}

		private TribalEvent CalculateRawMaterialYield()
		{
			Dictionary<Skill.SkillType, int> householdActivity = new Dictionary<Skill.SkillType, int>();
			int housesActive = 0;

			for( int i = 1; i < Enum.GetNames( typeof(Skill.SkillType) ).Length; i++ )
			{
				housesActive = FamilyNodes.Where( x => x.gameObject.GetComponent<FamilyControlScript>().CurrentActivity == (Skill.SkillType) i ).Count();
				if( housesActive > 0 )
					householdActivity.Add( (Skill.SkillType) i, housesActive);
			}

			if( householdActivity.Count == 0 ) return null;

			List<StatusModifier> mats = new List<StatusModifier>();

			foreach( Skill.SkillType skillTyp in householdActivity.Keys )
			{
				Dictionary<RawMaterialType, float> matYield = Skill.MaterialYield(skillTyp);
				float yieldMod = 0f;
				foreach( RawMaterialType matTyp in matYield.Keys )
				{
					matYield.TryGetValue( matTyp, out yieldMod );
					mats.Add( new StatusModifier( new StatusModifier.Effect( StatusModifier.Effect.Modifier.Amount, StatusModifier.Effect.Target.RawMaterial ),
											   	  Enum.GetName(typeof(RawMaterialType), matTyp),
											   	  ((Community.Capability * 0.01f) * yieldMod) +
												  ((Capability * 0.1f) * yieldMod * (1+BonusYield(skillTyp))) +
												  (((GetSkillValue(skillTyp) * householdActivity.Count) * 0.1f) * yieldMod)));

				}
			}

			TribalEvent evt = new TribalEvent("Material Yield");

			foreach( var moder in mats )
				evt.AddModifier( moder );

			return evt;
		}
    }
}

