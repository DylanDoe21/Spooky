using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.NoseCult
{
    public class MocoIdol : ModNPC  
    {
        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 68;
			NPC.height = 64;
            NPC.npcSlots = 0.5f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
			NPC.dontTakeDamage = true;
        }

        public override void AI()
        {
            if (NPC.localAI[0] == 0)
            {
                NPC.ai[0] = NPC.position.Y;
                NPC.localAI[0]++;
            }

            NPC.ai[1]++;
            NPC.position.Y = NPC.ai[0] + (float)Math.Sin(NPC.ai[1] / 100) * 10;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 0; numGores < 20; numGores++)
                {
                }
            }
		}
    }
}