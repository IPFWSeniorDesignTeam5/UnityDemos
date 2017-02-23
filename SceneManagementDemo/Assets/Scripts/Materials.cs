using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Tribal
{
	public enum RawMaterialType
	{
		// TODO: Add all raw material types
	}

	public enum FinishedGoodType
	{
		// TODO: Add all finished good types
		Homes,
		Tents,
		Pottery,
		Tools,
		Jewelry,
		Clothes,
		Fire
	}

	public class RawMaterial  {
		RawMaterialType MaterialType;

		public string Name { 
			get { 
				return Enum.GetName (typeof(RawMaterialType), MaterialType); 
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
			return (List<RawMaterial>)mats.Where( x => x.MaterialType == typ );
		}

		// TODO: Create private static method "GetRawMaterialWealth" that takes RawMaterialType and returns an int

		// TODO: Create public, readonly property "WealthValue" (int) that returns GetRawMaterialWealth in its get method.

		// TODO: Create constructor that takes RawMaterialType as input.


	}

	public class FinishedGood {

		List<RawMaterial> Materials;

		public FinishedGoodType GoodType {get; private set;}

		public string Name { 
			get { 
				return Enum.GetName (typeof(FinishedGoodType), GoodType); 
			}
			private set { Name = value; } 
		}

		private void AddMaterial( RawMaterial mat )
		{
			Materials.Add( mat );
		}

		public FinishedGood( FinishedGoodType type )
		{
			GoodType = type;
			Materials = new List<RawMaterial>();
		}

		public static FinishedGood CreateFromMaterials( List<RawMaterial> mats, FinishedGoodType goodType )
		{
			FinishedGood returnGood = new FinishedGood(goodType);

			try
			{
				Dictionary<RawMaterialType, int> requirements = GetProductionRequirements( goodType );

				List<RawMaterial> matsToMove = new List<RawMaterial>();

				foreach( RawMaterialType typ in requirements.Keys )
				{
					List<RawMaterial> hasMats = RawMaterial.GetMaterials( mats, typ );
					if( null != hasMats && hasMats.Count > 0 )
					{
						int i = 0;

						requirements.TryGetValue(typ, out i);

						if( hasMats.Count < i )
							return null;	// Insufficient materials

						for( int j = 0; j < i; j++ )
							matsToMove.Add( hasMats[j] );
					}
					else
						return null; // Insufficient materials
				}

				if( matsToMove.Count > 0 )
				{
					foreach( RawMaterial mat in matsToMove )
					{
						returnGood.AddMaterial( mat );
						mats.Remove( mat );
					}
				} 
				else
					throw new Exception( "No materials selected to use for FinishedGood produciton." );

			} catch( Exception e )
			{
				returnGood = null;
				Debug.LogError( "Exception in CreateFromMaterials (Materials.cs): " + e.Message );
			}

			return returnGood;
		}

		/// <summary>
		/// Gets a list of FinishedGood(s) within the given list that match the given type.
		/// </summary>
		/// <returns>The goods that match the given type (if any).</returns>
		/// <param name="goods">Goods list to search.</param>
		/// <param name="typ">Given type to search for.</param>
		List<FinishedGood> GetGoods( List<FinishedGood> goods, FinishedGoodType typ )
		{
			return (List<FinishedGood>)goods.Where( x => x.GoodType == typ );
		}

		private static Dictionary<RawMaterialType, int> GetProductionRequirements( FinishedGoodType goodType )
		{
			return null;
			// TODO: Based on tables in tech. design document, create switch statement to return new dictionary object with material (type and count of each) required to create givent goodType.
		}

		// TODO: Create private static method "GetTimeToProduce" that takes FinishedGoodType and returns a float;

		// TODO: Create public, readonly property "TimeToProduce" (float) that returns GetTimeToProduce in its get method.


		// TODO: Create property "WealthValue" (int) that sums wealth values of raw materials, and includes TimeToProduce in calculating (See tech. design document).

	}


}
