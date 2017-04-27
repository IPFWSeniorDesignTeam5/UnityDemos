using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Tribal
{
	public static class Community {
		private static List<Family> m_Families = new List<Family>();

		private static string [] FamilyPreNames = new string[] { "Lum", "Trot", "Rruli", "Eim", "Arro", "Whoe", "Sh'o", "Uro'", "Yya'", "M'ol" };
		private static string [] FamilySufNames = new string[] { "ma", "eeami", "tiaro", "mon", "ogo", "tek", "ram", "soon", "lee", "ba" };

		public static short StartingFamilySkill{ get; private set; }

		public static float Capability { 
			get{
				return m_Families.Sum( x => x.Capability );
			}
		}

		static Community()
		{
			StartingFamilySkill = 100;
			m_Families = new List<Family>();
			SeasonTimer.SeasonEndEvent += SeasonEnded;
		}

		public static Family GetFamilyByNode( MapNode n )
		{
			return m_Families.Where( x => x.FamilyNodes.Contains(n)).FirstOrDefault();
		}

		public static string GetNewFamilyName ()
		{
			short i, j;
			short tried = 0;
			string newName = "";

			do
			{
				i = (short)TribeControl.RandomGen.Next( 0, FamilyPreNames.Length - 1 );
				j = (short)TribeControl.RandomGen.Next( 0, FamilySufNames.Length - 1 );
				newName = FamilyPreNames[i] + FamilySufNames[j];
				tried ++;
				if( tried > 50 )
				{
					Debug.LogError( "Failed to get distinct random family name." );
					return "Unknown";
				}
			} while ( m_Families.Where( x => x.FamilyName == newName ).Count() > 0 );

			return newName;
		}

		public static void AddFamily( Family addNew )
		{
			if (null != addNew)
			{
				m_Families.Add (addNew);
			}
			else
				Debug.LogError ("Null family added to Community.");
		}

		public static void SeasonEnded( SeasonTimer.SeasonEndEventArgs e )
		{
			// Roll all environmental events for the last season

			// Roll all Community/Historical events
		}
	}
}