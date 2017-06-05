using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LeftTom : HandDrum
{
    override protected string drumName { get { return "High_tom"; } }
    override protected float radius { get { return 2; } }
}
