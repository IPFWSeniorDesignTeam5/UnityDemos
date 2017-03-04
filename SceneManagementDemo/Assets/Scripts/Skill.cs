using System;
using System.Collections;
using System.Collections.Generic;

namespace Tribal
{
    public class Skill
    {
        public enum SkillType
        {
            // TODO: Enter all skill types here i.e. Hunting, Gathering, etc.
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

         // TODO: Add public static method "MaterialYield" that takes a SkillType as input and returns Dictionary<RawMaterialType, float> representing the skill's produced material types and their yield modifiers

    }
}