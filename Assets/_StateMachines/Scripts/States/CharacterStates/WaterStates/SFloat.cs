using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFloat : SWater
{
    public SFloat(Character character) : base(character)
    {
    }

    public override Type FixedTick()
    {
        ft= base.FixedTick();
        if (ft!=null)
        {
            return ft;
        }

        Vector2 direction = character.Reader.Direction;

        if (!direction.Equals(Vector2.zero))
        {
            ft = typeof(SSwim);
        }
        //else if (character.InputReader.Dash)
        //{
        //  ft = typeof(SDashSwim);
        //}


        return ft;
    }
}
