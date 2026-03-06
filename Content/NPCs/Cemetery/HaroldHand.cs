using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Cemetery
{
    public class HaroldHandLeft : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.width = 42;
			NPC.height = 30;
            NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
        }

        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.direction;

            NPC.alpha = Parent.alpha;

            if (!Parent.active || Parent.type != ModContent.NPCType<Harold>())
            {
                NPC.active = false;
            }

            Vector2 ParentPosition = Parent.Center + new Vector2(Parent.spriteDirection == -1 ? -100 : 100, 0);
            Vector2 destination = ParentPosition + (MathHelper.TwoPi - MathHelper.PiOver2 + NPC.localAI[0]).ToRotationVector2() * 15f;
            NPC.position = destination - (NPC.Size / 2);
            NPC.localAI[0] -= MathHelper.ToRadians(2f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
            }
		}
    }

    public class HaroldHandRight : HaroldHandLeft  
    {
        public override void AI()
        {
            NPC Parent = Main.npc[(int)NPC.ai[3]];

            NPC.spriteDirection = Parent.direction;

            NPC.alpha = Parent.alpha;

            if (!Parent.active || Parent.type != ModContent.NPCType<Harold>()) 
            {
                NPC.active = false;
            }

            Vector2 ParentPosition = Parent.Center + new Vector2(Parent.spriteDirection == 1 ? -100 : 100, 0);
            Vector2 destination = ParentPosition + (MathHelper.TwoPi - MathHelper.PiOver2 + NPC.localAI[0]).ToRotationVector2() * 15f;
            NPC.position = destination - (NPC.Size / 2);
            NPC.localAI[0] += MathHelper.ToRadians(2f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
            }
		}
    }
}