using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Core
{
    public class NPCGlobal : GlobalNPC
    {
        public static int Orro = -1;
        public static int Boro = -1;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        { 
            //all bosses drop goodie bags
            if (npc.boss)
            {
                npcLoot.Add(ItemDropRule.Common(ItemID.GoodieBag, 1, 2, 5));
            }

            //start ghost event after evil bosses
            if (((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail) && npc.boss) ||
            npc.type == NPCID.BrainofCthulhu && !NPC.downedBoss2)
            {
                SpookyWorld.ShouldStartGhostEvent = true;
            }
        }
    }
}