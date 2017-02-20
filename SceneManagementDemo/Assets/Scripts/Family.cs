using System;

namespace Tribal
{
	public class Family
	{
		int Population;

		// TODO: Add list of raw materials

		// TODO: Add list of finished goods.

		// TODO: Create a property "TotalWealth" that returns the total wealth of all raw materials and finished goods

		delegate void FamilyEventListener();

		event FamilyEventListener FamilyEvent;

		public Family ()
		{
			SeasonTimer.SeasonEndEvent += FamilySeasonEnd;
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

