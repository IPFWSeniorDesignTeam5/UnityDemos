using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Tribal
{
	public enum RawMaterialType
	{
		Bone,
        Clay,
        Fiber,
        Shells,
        Stone, 
        Wood,
        Skins,
        Food
	}

	public enum FinishedGoodType
	{
		Homes,
        Pottery,
        Tools,
        Jewelry,
        Clothes,
        Fire
	}

	public static class TribalAssets
	{
		private static Dictionary<string, RawMaterialType> m_MaterialDictionary = new Dictionary<string, RawMaterialType>();
		private static Dictionary<string, FinishedGoodType> m_GoodsDictionary = new Dictionary<string, FinishedGoodType>();

		public static Dictionary<string, RawMaterialType> MaterialDictionary {
			get{
				if( m_MaterialDictionary.Count == 0 )
				{
					string [] matNames = Enum.GetNames( typeof(RawMaterialType) );
					for( short i = 0; i < matNames.Length; i++ )
					{
						m_MaterialDictionary.Add( matNames[i], (RawMaterialType)i );
					}
				}
				return m_MaterialDictionary;
			}
			private set{}
		}

		public static Dictionary<string, FinishedGoodType> GoodsDictionary {
			get{
				if( m_GoodsDictionary.Count == 0 )
				{
					string [] goodNames = Enum.GetNames( typeof(FinishedGoodType) );
					for( short i = 0; i < goodNames.Length; i++ )
					{
						m_GoodsDictionary.Add( goodNames[i], (FinishedGoodType)i );
					}
				}
				return m_GoodsDictionary;
			}
			private set{}
		}
	}

	public class RawMaterial  {
        public RawMaterialType Type { get; private set; }

		public string Name { 
			get {
                return Enum.GetName(typeof(RawMaterialType), Type); 
			} 
			private set { Name = value; } 
		}

		/// <summary>
		/// Gets a list of the RawMaterial(s) within the given list that match the given type.
		/// </summary>
		/// <returns>The materials that match the given type (if any).</returns>
		/// <param name="mats">Material list to search.</param>
		/// <param name="typ">Given type to search for.</param>
		public static List<RawMaterial> GetMaterials( List<RawMaterial> mats, RawMaterialType typ )
		{
			if( mats.Count == 0 ) return null;

            return (List<RawMaterial>)mats.Where(x => x.Type == typ);
		}

        private static int GetRawMaterialWealth(RawMaterialType raw)
        {
            switch (raw)
            {
                case RawMaterialType.Bone:
                    return 4;
                case RawMaterialType.Clay:
                    return 2;
                case RawMaterialType.Fiber:
                    return 3;
                case RawMaterialType.Shells:
                    return 6;
                case RawMaterialType.Stone:
                    return 2;
                case RawMaterialType.Wood:
                    return 3;
                case RawMaterialType.Skins:
                    return 8;
            }

            return 0;
        }

        public int WealthValue
        {
            get
            {
                return GetRawMaterialWealth(Type);
            }
   			private set {WealthValue = value;}
        }

        public RawMaterial(RawMaterialType materialType)
        {
            Type = materialType;
        }
    }

    public class FinishedGood
    {

        List<RawMaterial> Materials;

        public FinishedGoodType Type { get; private set; }

        public int Tier {get; private set;}

        public string Name
        {
            get
            {
                return Enum.GetName(typeof(FinishedGoodType), Type);
            }
            private set { Name = value; }
        }

        private void AddMaterial(RawMaterial mat)
        {
            Materials.Add(mat);
        }

        public FinishedGood(FinishedGoodType type, int goodTier)
        {
        	Tier = goodTier;
            Type = type;
            Materials = new List<RawMaterial>();
        }

        public static int CanCreate(List<RawMaterial> mats, FinishedGoodType goodType, int tier)
        {
			Dictionary<RawMaterialType, int> requirements = GetProductionRequirements(goodType, tier);
			int count = int.MaxValue;

			foreach (RawMaterialType typ in requirements.Keys)
			{
				int stock = RawMaterial.GetMaterials(mats, typ).Count;

				if( stock == 0 ) return 0;

				int requiredStock = int.MaxValue;
				requirements.TryGetValue( typ, out requiredStock );

				int temporary = stock / requiredStock;

				if(temporary < count)
					count = temporary;
			}

			if( count == int.MaxValue ) return 0;

			return count;
        }


		public static void Upgrade(List<RawMaterial> mats, FinishedGood subject)
		{
			try
			{
				Dictionary<RawMaterialType, int> requirements = GetProductionRequirements(subject.Type, subject.Tier);

				List<RawMaterial> matsToMove = new List<RawMaterial>();

				foreach (RawMaterialType typ in requirements.Keys)
				{
					List<RawMaterial> hasMats = RawMaterial.GetMaterials(mats, typ);
					if (null != hasMats && hasMats.Count > 0)
					{
						int i = 0;

						requirements.TryGetValue(typ, out i);

						if (hasMats.Count < i)
							return;    // Insufficient materials

						for (int j = 0; j < i; j++)
							matsToMove.Add(hasMats[j]);
					}
					else
						return; // Insufficient materials
				}

				if (matsToMove.Count > 0)
				{
					foreach (RawMaterial mat in matsToMove)
					{
						subject.AddMaterial(mat);
						mats.Remove(mat);
					}
					subject.Tier++;
				}
				else
					throw new Exception("No materials selected to use for FinishedGood being upgraded.");

			}
			catch (Exception e)
			{
				Debug.LogError("Exception in Upgrade() (Materials.cs): " + e.Message);
			}
		}


        public static FinishedGood CreateFromMaterials(List<RawMaterial> mats, FinishedGoodType goodType, int tier)
        {
            FinishedGood returnGood = new FinishedGood(goodType, tier);

            try
            {
                Dictionary<RawMaterialType, int> requirements = GetProductionRequirements(goodType, tier);

                List<RawMaterial> matsToMove = new List<RawMaterial>();

                foreach (RawMaterialType typ in requirements.Keys)
                {
                    List<RawMaterial> hasMats = RawMaterial.GetMaterials(mats, typ);
                    if (null != hasMats && hasMats.Count > 0)
                    {
                        int i = 0;

                        requirements.TryGetValue(typ, out i);

                        if (hasMats.Count < i)
                            return null;    // Insufficient materials

                        for (int j = 0; j < i; j++)
                            matsToMove.Add(hasMats[j]);
                    }
                    else
                        return null; // Insufficient materials
                }

                if (matsToMove.Count > 0)
                {
                    foreach (RawMaterial mat in matsToMove)
                    {
                        returnGood.AddMaterial(mat);
                        mats.Remove(mat);
                    }
                }
                else
                    throw new Exception("No materials selected to use for FinishedGood produciton.");

            }
            catch (Exception e)
            {
                returnGood = null;
                Debug.LogError("Exception in CreateFromMaterials (Materials.cs): " + e.Message);
            }

            return returnGood;
        }
        
        /// <summary>
        /// Gets a list of FinishedGood(s) within the given list that match the given type.
        /// </summary>
        /// <returns>The goods that match the given type (if any).</returns>
        /// <param name="goods">Goods list to search.</param>
        /// <param name="typ">Given type to search for.</param>
        List<FinishedGood> GetGoods(List<FinishedGood> goods, FinishedGoodType typ)
        {
            return (List<FinishedGood>)goods.Where(x => x.Type == typ);
        }

        private static Dictionary<RawMaterialType, int> GetProductionRequirements(FinishedGoodType goodType, int tier)
        {
            Dictionary<RawMaterialType, int> finalGoodReqs = new Dictionary<RawMaterialType, int>();
            switch (goodType)
            {
                case FinishedGoodType.Homes:
                    finalGoodReqs.Add(RawMaterialType.Wood, 20 * tier);
                    finalGoodReqs.Add(RawMaterialType.Skins, 4 * tier);
                    break;
                case FinishedGoodType.Pottery:
                    finalGoodReqs.Add(RawMaterialType.Clay, 5 * tier);
                    finalGoodReqs.Add(RawMaterialType.Shells, 1 * tier);
                    finalGoodReqs.Add(RawMaterialType.Wood, 1 * tier);
                    break;
                case FinishedGoodType.Tools:
                    finalGoodReqs.Add(RawMaterialType.Stone, 1 * tier);
                    finalGoodReqs.Add(RawMaterialType.Wood, 2 * tier);
                    finalGoodReqs.Add(RawMaterialType.Fiber, 2 * tier);
                    break;
                case FinishedGoodType.Jewelry:
                    finalGoodReqs.Add(RawMaterialType.Shells, 5 * tier);
                    finalGoodReqs.Add(RawMaterialType.Fiber, 1 * tier);
                    break;
                case FinishedGoodType.Clothes:
                    finalGoodReqs.Add(RawMaterialType.Fiber, 3 * tier);
                    finalGoodReqs.Add(RawMaterialType.Skins, 5 * tier);
                    break;
                case FinishedGoodType.Fire:
                    finalGoodReqs.Add(RawMaterialType.Wood, 4 * tier);
                    break;
            }

            return finalGoodReqs;
        }

        private static float GetTimeToProduce(FinishedGoodType finishedG, int tier)
        {
            switch (finishedG)
            {
                case FinishedGoodType.Homes:
                    return 1f * tier;
                case FinishedGoodType.Pottery:
					return 0.1f * tier;
                case FinishedGoodType.Tools:
					return 0.2f * tier;
                case FinishedGoodType.Jewelry:
					return 0.25f * tier;
                case FinishedGoodType.Clothes:
					return 0.1f * tier;
                case FinishedGoodType.Fire:
					return 0.1f * tier;

            }
            return float.MaxValue;
        }

        public float TimeToProduce
        {
            get
            {
                return GetTimeToProduce(Type, Tier);
            }
            private set{TimeToProduce = value;}
        }

        public int WealthValue
        {
            get
            {
                int sumWealth = 0;

                foreach( RawMaterial m in Materials )
                    sumWealth += m.WealthValue;

                return sumWealth + (int)(20f * TimeToProduce);
            }
        }
    }
}
