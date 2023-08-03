using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class ZomboidNecromancerSkull : ModNPC
    {
        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

        public override void SetDefaults()
        {
            NPC.lifeMax = 60;
            NPC.damage = 30;
            NPC.defense = 0;
            NPC.width = 28;
            NPC.height = 24;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.aiStyle = 10;
			AIType = NPCID.CursedSkull;
        }

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] >= 420)
            {
                NPC.direction = NPC.spriteDirection = NPC.velocity.X > 0f ? 1 : -1;
                NPC.rotation = NPC.velocity.ToRotation();

                if (NPC.spriteDirection == -1)
                {
                    NPC.rotation += MathHelper.Pi;
                }
            }

            if (NPC.localAI[0] == 420)
            {
                NPC.velocity *= 0.2f;

                Vector2 ChargeDirection = player.Center - NPC.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= -5;
                ChargeDirection.Y *= -5;  
                NPC.velocity.X = ChargeDirection.X;
                NPC.velocity.Y = ChargeDirection.Y;
            }

            if (NPC.localAI[0] == 440)
            {
                SoundEngine.PlaySound(SoundID.NPCHit36, NPC.Center);

                Vector2 ChargeDirection = player.Center - NPC.Center;
                ChargeDirection.Normalize();
                ChargeDirection *= 12;
                NPC.velocity = ChargeDirection;
            }

            if (NPC.localAI[0] >= 480)
            {
                SoundEngine.PlaySound(SoundID.NPCHit54, NPC.Center);

                for (int numDust = 0; numDust < 15; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.HallowSpray, 0f, -2f, 0, default, 1.5f);
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[DustGore].noGravity = true;

                    if (Main.dust[DustGore].position != NPC.Center)
                    {
                        Main.dust[DustGore].velocity = NPC.DirectionTo(Main.dust[DustGore].position) * Main.rand.NextFloat(1f, 2f);
                    }
                }
                
                NPC.active = false;
            }
        }
    }
}