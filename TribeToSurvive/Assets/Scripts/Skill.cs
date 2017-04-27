using System;
using System.Collections;
using System.Collections.Generic;

namespace Tribal
{
    public class Skill
    {
        public enum SkillType
        {
        	None = 0,
            Hunting,
            Gathering,
            Farming, 
            Production
        }

        public SkillType Type { get; private set; }

        public short Experience { get; private set; }

        public Skill(SkillType type)
        {
            Type = type;
        }

        public void AddExperience(short value)
        {
            if (value > 0)
                Experience += value;
        }

         public static Dictionary<RawMaterialType, float> MaterialYield( SkillType stype )
         {
         	Dictionary<RawMaterialType, float> returnDict = new Dictionary<RawMaterialType, float>();

         	float a = 0.25f;
         	float b = 0.4f;
         	float c = 0.6f;
         	float d = 1f;

			switch( stype )
			{
				case SkillType.Hunting:
					returnDict.Add( RawMaterialType.Bone, b );
					returnDict.Add( RawMaterialType.Skins, c );
					returnDict.Add( RawMaterialType.Food, d );
				break;

				case SkillType.Gathering:
					returnDict.Add( RawMaterialType.Clay, b );
					returnDict.Add( RawMaterialType.Shells, c );
					returnDict.Add( RawMaterialType.Stone, d );
					returnDict.Add( RawMaterialType.Wood, d );
					returnDict.Add( RawMaterialType.Food, a );
				break;
				case SkillType.Farming:
					returnDict.Add( RawMaterialType.Clay, a );
					returnDict.Add( RawMaterialType.Fiber, b );
					returnDict.Add( RawMaterialType.Shells, a );
					returnDict.Add( RawMaterialType.Stone, a );
					returnDict.Add( RawMaterialType.Food, a );
				break;
			}

			return returnDict;
         }
    }
}