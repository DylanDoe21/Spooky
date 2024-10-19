using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Food;
using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.SpookyHell.Projectiles;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ManHoleFleshy : ModNPC  
    {
        public float destinationX = 0f;
		public float destinationY = 0f;

        bool FoundPosition = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
			writer.Write(FoundPosition);

            //floats
            writer.Write(destinationX);
            writer.Write(destinationY);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
			FoundPosition = reader.ReadBoolean();

            //floats
            destinationX = reader.ReadSingle();
            destinationY = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 140;
            NPC.damage = 35;
            NPC.defense = 10;
            NPC.width = 78;
            NPC.height = 48;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ManHoleFleshy"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            if (NPC.ai[0] < 300)
            {
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {   
                if (NPC.ai[0] == 300)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }

                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 11)
                {
                    NPC.frame.Y = 11 * frameHeight;
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

            if (NPC.Distance(player.Center) <= 700f || NPC.ai[0] >= 230)
            {
                NPC.ai[0]++;
            }

            if (NPC.ai[0] == 240)
            {   
                SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X, NPC.Center.Y + 5), 
                new Vector2(player.Center.X < NPC.Center.X ? Main.rand.Next(-10, -2) : Main.rand.Next(2, 10), -10), ModContent.ProjectileType<ManHoleBloodBall>(), NPC.damage, 4.5f);
            }

            if (NPC.ai[0] >= 300)
            {
                if (!FoundPosition && destinationX == 0f && destinationY == 0f)
				{
					Vector2 center = new Vector2(player.Center.X, player.Center.Y - 100);

					center.X += Main.rand.Next(-500, 500);

					int numtries = 0;
					int x = (int)(center.X / 16);
					int y = (int)(center.Y / 16);

					while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
					{
						y++;
						center.Y = y * 16;
					}
					while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10)
					{
						numtries++;
						y--;
						center.Y = y * 16;
					}

					destinationX = center.X;
					destinationY = center.Y;

					FoundPosition = true;
				}
				else
				{
					NPC.ai[1]++;

					if (NPC.ai[1] >= 30 && NPC.ai[1] <= 90)
					{
						float PositionX = destinationX - (float)(NPC.width / 2);
                        float PositionY = destinationY - (float)NPC.height;

                        Dust dust = Dust.NewDustDirect(new Vector2(PositionX, PositionY + 32), NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, default, 2.5f);
                        dust.noGravity = true;
					}

					if (NPC.ai[1] > 120 && destinationX != 0f && destinationY != 0f)
					{
						NPC.position.X = destinationX - (float)(NPC.width / 2);
						NPC.position.Y = destinationY - (float)(NPC.height / 2) - 12;
						destinationX = 0f;
						destinationY = 0f;

						SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.Center);

						for (int numDusts = 0; numDusts < 15; numDusts++)
						{
							Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, default, 2.5f);
							dust.noGravity = true;
						}
						
						NPC.ai[0] = 0;
						NPC.ai[1] = 0;

						FoundPosition = false;

						NPC.netUpdate = true;
					}
				}
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepyChunk>(), 3, 1, 2));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterBloodVial>(), 100));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GoofyPretzel>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int Repeats = 1; Repeats <= 2; Repeats++)
                {
                    for (int numGores = 1; numGores <= 3; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleFleshyGore" + numGores).Type);
                        }
                    }
                }
            }
        }
    }
}