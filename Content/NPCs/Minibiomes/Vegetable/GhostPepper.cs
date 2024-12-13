using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
    public class GhostPepper : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1000;
            NPC.damage = 60;
            NPC.defense = 12;
            NPC.width = 62;
            NPC.height = 120;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(0, 0, 10, 0);
            NPC.HitSound = SoundID.NPCHit37;
            NPC.DeathSound = SoundID.NPCDeath52;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.GhostPepper"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 2;
            NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

            if (player.Distance(NPC.Center) >= 380f)
            {
                if (NPC.alpha < 255)
                {
                    NPC.alpha += 2;
                }
            }
            else
            {
                if (NPC.alpha > 0)
                {
                    NPC.alpha -= 5;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
			if (NPC.life <= 0) 
            {
                for (int numDusts = 0; numDusts < 12; numDusts++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, 192, 0f, -2f, 0, default, 1.5f);
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                }
            }
        }
    }
}