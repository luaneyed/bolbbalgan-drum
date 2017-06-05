using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Snare : HandDrum
{
    override protected string drumName { get { return "Snare_drum"; } }
    override protected float radius { get { return 3; } }
}
