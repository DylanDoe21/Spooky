using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

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

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 180;
            NPC.damage = 50;
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

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y + 5, player.Center.X < NPC.Center.X ? Main.rand.Next(-10, -2) : Main.rand.Next(2, 10), -10,
                    ModContent.ProjectileType<ManHoleBloodBall>(), NPC.damage / 4, 1, Main.myPlayer, 0, 0);
                }
            }

            if (NPC.ai[0] >= 300)
            {
                NPC.dontTakeDamage = true;

                if (NPC.frame.Y == 11 * NPC.height && NPC.ai[0] == 360 && destinationX == 0f && destinationY == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num86 = (int)player.position.X / 16;
                        int num87 = (int)player.position.Y / 16;
                        int num88 = (int)NPC.position.X / 16;
                        int num89 = (int)NPC.position.Y / 16;
                        int num90 = 20;
                        int num91 = 0;
                        bool flag4 = false;

                        while (!flag4 && num91 < 300)
                        {
                            int num = num91;
                            num91 = num + 1;
                            int num92 = Main.rand.Next(num86 - num90, num86 + num90);
                            int num93 = Main.rand.Next(num87 - num90, num87 + num90);
                            for (int num94 = num93; num94 < num87 + num90; num94 = num + 1)
                            {
                                if ((num94 < num87 - 4 || num94 > num87 + 4 || num92 < num86 - 4 || num92 > num86 + 4) && (num94 < num89 - 1 || num94 > num89 + 1 || num92 < num88 - 1 || num92 > num88 + 1) && Main.tile[num92, num94].HasUnactuatedTile)
                                {
                                    bool flag5 = true;
                                    if ((Main.tile[num92, num94 - 1].LiquidType == LiquidID.Lava))
                                    {
                                        flag5 = false;
                                    }

                                    bool ValidForTeleport = Main.tileSolid[(int)Main.tile[num92 - 2, num94].TileType] && Main.tileSolid[(int)Main.tile[num92 - 1, num94].TileType] &&
                                    Main.tileSolid[(int)Main.tile[num92, num94].TileType] && Main.tileSolid[(int)Main.tile[num92 + 1, num94].TileType] && Main.tileSolid[(int)Main.tile[num92 + 2, num94].TileType];

                                    if (flag5 && ValidForTeleport && !Collision.SolidTiles(num92 - 1, num92 + 1, num94 - 4, num94 - 1))
                                    {
                                        destinationX = (float)num92;
                                        destinationY = (float)num94;
                                        flag4 = true;
                                        break;
                                    }
                                }

                                num = num94;
                            }
                        }

                        NPC.netUpdate = true;
                    }
                }

                if (NPC.ai[0] >= 360 && NPC.ai[0] <= 400)
                {
                    float PositionX = destinationX * 16f - (float)(NPC.width / 2) + 8f;
                    float PositionY = destinationY * 16f - (float)NPC.height;

                    Dust dust = Dust.NewDustDirect(new Vector2(PositionX, PositionY + 32), NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, default, 2.5f);
                    dust.noGravity = true;
                }

                if (NPC.ai[0] >= 420 && destinationX != 0f && destinationY != 0f)
                {
                    NPC.dontTakeDamage = false;

                    NPC.position.X = destinationX * 16f - (float)(NPC.width / 2) + 8f;
                    NPC.position.Y = destinationY * 16f - (float)NPC.height;
                    destinationX = 0f;
                    destinationY = 0f;
                    NPC.netUpdate = true;

                    SoundEngine.PlaySound(SoundID.NPCDeath12, NPC.Center);

                    for (int numDusts = 0; numDusts < 15; numDusts++)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y + 24), NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-12f, -8f), 50, default, 2.5f);
                        dust.noGravity = true;
                    }
                    
                    NPC.ai[0] = 0;
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