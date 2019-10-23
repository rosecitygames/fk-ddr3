using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieDevTools.Demo.CrabBattle
{
    /// <summary>
    /// Static references to the crab animation trigger hash ids.
    /// </summary>
    public static class CrabAnimationTrigger
    {
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Fight = Animator.StringToHash("Fight");
        public static readonly int Explode = Animator.StringToHash("Explode");
    }
}