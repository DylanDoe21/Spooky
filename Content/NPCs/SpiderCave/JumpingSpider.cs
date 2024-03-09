using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class JumpingSpider1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 80;
            NPC.damage = 30;
			NPC.defense = 5;
			NPC.width = 46;
			NPC.height = 34;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit33;
			NPC.DeathSound = SoundID.NPCDeath16;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JumpingSpider1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
        {
            if (NPC.localAI[0] == 0)
            {
                if (NPC.velocity.X != 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 4)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 1 * frameHeight;
                    }
                }
                else
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                //jumping up frame
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
                //squish frame before jumping up
                else if (NPC.ai[0] >= 75 && NPC.ai[1] > 0 && NPC.velocity.Y == 0)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
                //falling frame
                else if (NPC.velocity.Y > 0)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                //idle frame
                else
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            if (NPC.localAI[0] == 0)
            {
                NPC.aiStyle = 66;
			    AIType = NPCID.Worm;

                if (NPC.Distance(player.Center) <= 200f || player.GetModPlayer<SpookyPlayer>().WhipSpiderAggression)
                {
                    //set ai to 75 so it jumps immediately
                    NPC.ai[0] = 75;
                    NPC.localAI[0]++;
                }
            }
            else
            {
                NPC.rotation = NPC.velocity.Y * 0.05f;
                
                NPC.ai[0]++;

                if (NPC.ai[0] >= 75)
                {
                    //set where the it should be jumping towards
                    Vector2 JumpTo = new(player.Center.X, NPC.Center.Y - 500);

                    //set velocity and speed
                    Vector2 velocity = JumpTo - NPC.Center;
                    velocity.Normalize();

                    float speed = MathHelper.Clamp(velocity.Length() / 36, 12, 22);

                    //actual jumping
                    if (NPC.velocity.X == 0)
                    {
                        if (NPC.velocity.Y == 0)
                        {
                            NPC.ai[1]++;

                            if (NPC.ai[1] == 10)
                            {
                                velocity.Y -= 0.25f;
                            }
                        }

                        if (NPC.ai[1] > 10)
                        {
                            velocity.X *= 1.2f;
                            NPC.velocity = velocity * speed;
                        }
                    }
                }

                //loop ai
                if (NPC.ai[0] >= 100)
                {
                    NPC.ai[0] = Main.rand.Next(0, 45);
                    NPC.ai[1] = 0;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderChitin>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSpiderWhiteGore").Type);
                }

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSpiderWhiteLegGore").Type);
                    }
                }
            }
        }
	}

    public class JumpingSpider2 : JumpingSpider1
	{
        public override void SetDefaults()
		{
            NPC.lifeMax = 80;
            NPC.damage = 25;
			NPC.defense = 0;
			NPC.width = 46;
			NPC.height = 34;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit33;
			NPC.DeathSound = SoundID.NPCDeath16;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JumpingSpider2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Dazed, 120);
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSpiderGrayGore").Type);
                }

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSpiderGrayLegGore").Type);
                    }
                }
            }
        }
    }
}