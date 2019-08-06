using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowflake : Spell
{
    public override void Cast(Tile casterTile, Tile targetTile)
    {
        base.Cast(casterTile, casterTile);
    }
}
