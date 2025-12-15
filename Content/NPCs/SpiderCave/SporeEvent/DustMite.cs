using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
	public class DustMite1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 320;
            NPC.damage = 55;
            NPC.defense = 10;
			NPC.width = 46;
			NPC.height = 34;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit38 with { Pitch = 0.5f, Volume = 0.5f };
			NPC.DeathSound = SoundID.NPCDeath16;
            NPC.aiStyle = 66;
			AIType = NPCID.Worm;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SporeEventBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.DustMite1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
        {
            //jumping up frame
            if (NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            //squish frame before jumping up
            else if (NPC.ai[0] >= 75 && NPC.ai[1] > 0 && NPC.velocity.Y == 0)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            //falling frame
            else if (NPC.velocity.Y > 0)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            //idle frame
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.Y * 0.01f;
                
            NPC.ai[0]++;
            if (NPC.ai[0] >= 2)
            {
                //set where the it should be jumping towards
                Vector2 JumpTo = new(player.Center.X, NPC.Center.Y - 500);

                //set velocity and speed
                Vector2 velocity = JumpTo - NPC.Center;
                velocity.Normalize();

                float speed = MathHelper.Clamp(velocity.Length() / 36, 10, 18);

                //actual jumping
                if (NPC.velocity.X == 0)
                {
                    if (NPCGlobalHelper.IsCollidingWithFloor(NPC))
                    {
                        NPC.ai[1]++;

                        if (NPC.ai[1] == 10)
                        {
                            velocity.Y -= 0.25f;
                        }
                    }

                    if (NPC.ai[1] > 10)
                    {
                        velocity.X *= 1.1f;
                        NPC.velocity = velocity * speed;
                    }

                    NPC.netUpdate = true;
                }
            }

            //loop ai
            if (NPC.ai[0] >= 50)
            {
                NPC.ai[0] = 0;
                NPC.ai[1] = 0;

                NPC.netUpdate = true;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiteMandibles>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FungusSeed>(), 120));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DustMiteGreenGore" + numGores).Type);
                    }
                }
            }
        }
	}

    public class DustMite2 : DustMite1
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.DustMite2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/DustMiteTealGore" + numGores).Type);
                    }
                }
            }
        }
    }
}