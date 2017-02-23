using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tribal
{
	public class Community {
		List<Family> m_Families;

		public Community()
		{
			m_Families = new List<Family>();
			SeasonTimer.SeasonEndEvent += SeasonEnded;
		}

		public void AddFamily( Family addNew )
		{
			if (null != addNew)
				m_Families.Add (addNew);
			else
				Debug.LogError ("Null family added to Community.");
		}

		public void SeasonEnded( SeasonTimer.SeasonEndEventArgs e )
		{
			// Roll all environmental events for the last season

			// Roll all Community/Historical events
		}
	}
}