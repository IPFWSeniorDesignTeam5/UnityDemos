using System.Collections;
using System.Collections.Generic;

public class Skill {

	public enum SkillType
	{
		// TODO: Enter all skill types here i.e. Hunting, Gathering, etc.
	}

	public SkillType Type { get; private set; }

	public short Experience { get; private set; }

	public Skill( SkillType type )
	{
		Type = type;
	}

	public void AddExperience( short value )
	{
		if( value > 0 )
			Experience += value;
	}
}
