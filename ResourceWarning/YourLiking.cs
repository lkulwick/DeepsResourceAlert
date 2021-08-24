using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace ResourceWarning
{
    public class ThingDef_PlagueBullet : ThingDef
    {
        public float AddHediffChance = 0.05f;
        public HediffDef HediffToAdd;


        public override void ResolveReferences()
        {
            Log.Message("deep test bullet");
            HediffToAdd = HediffDefOf.Plague;
        }
    }
}
