using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class JackInTheBox : ModNPC  
    {
        Vector2 SavePosition;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 25;
            NPC.defense = 0;
            NPC.width = 36;
			NPC.height = 60;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit15;
			NPC.DeathSound = SoundID.NPCDeath15;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JackInTheBox"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }

            if (NPC.ai[1] == 0)
            {
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                if (NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }

                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }
        
        public override void AI()
        {
            NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

            if (NPC.ai[1] == 0)
            {
                if (NPC.life < NPC.lifeMax || NPC.Distance(player.Center) <= 150)
                {
                    SoundEngine.PlaySound(SoundID.Zombie121 with { Pitch = 1f, Volume = 0.5f }, NPC.Center);
                    NPC.ai[1] = 1;
                }

                if (NPC.ai[0] == 0)
                {
                    SavePosition = NPC.Center;
                    
                    NPC.ai[0]++;
                }
                else
                {
                    NPC.spriteDirection = NPC.direction = NPC.velocity.X <= 0 ? -1 : 1;

                    //prevents the pet from getting stuck on sloped tiled
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                    Vector2 center2 = NPC.Center;
                    Vector2 vector48 = SavePosition - center2;
                    float CenterDistance = vector48.Length();

                    if (CenterDistance > 400f && NPC.velocity.Y == 0)
                    {
                        NPC.ai[0] = 0;
                    }

                    if (NPC.velocity.Y == 0 && HoleBelow() && CenterDistance > 100f)
                    {
                        NPC.velocity.X = -NPC.velocity.X;
                        NPC.netUpdate = true;
                    }

                    if ((NPC.velocity.X < 0f && NPC.direction == -1) || (NPC.velocity.X > 0f && NPC.direction == 1))
                    {
                        if (NPC.velocity.Y == 0 && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * 35) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 3))
                        {
                            NPC.velocity.Y = -8f;
                            NPC.netUpdate = true;
                        }
                    }

                    if (NPC.collideX)
                    {
                        NPC.velocity.X = -NPC.velocity.X;
                    }

                    NPC.velocity.Y += 0.35f;

                    if (NPC.velocity.Y > 15f)
                    {
                        NPC.velocity.Y = 15f;
                    }

                    if (CenterDistance > 90f)
                    {
                        if (SavePosition.X - NPC.position.X > 0f)
                        {
                            NPC.velocity.X += 0.01f;
                            if (NPC.velocity.X > 1f)
                            {
                                NPC.velocity.X = 1f;
                            }
                        }
                        else
                        {
                            NPC.velocity.X -= 0.01f;
                            if (NPC.velocity.X < -1f)
                            {
                                NPC.velocity.X = -1f;
                            }
                        }
                    }
                    else
                    {
                        if (NPC.velocity.X >= 0)
                        {
                            NPC.velocity.X += 0.01f;
                            if (NPC.velocity.X > 1f)
                            {
                                NPC.velocity.X = 1f;
                            }
                        }
                        else
                        {
                            NPC.velocity.X -= 0.01f;
                            if (NPC.velocity.X < -1f)
                            {
                                NPC.velocity.X = -1f;
                            }
                        }
                    }
                }
            }
            else
            {
                NPC.spriteDirection = NPC.direction;

                NPC.aiStyle = 26;
                NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y, 0, 12);

                Vector2 position = NPC.Center + new Vector2(5 * NPC.spriteDirection, 5) + Vector2.Normalize(NPC.velocity);
                
                int newDust = Dust.NewDust(NPC.Center, 1, 1, DustID.Torch, 0f, 0f, 0, default, 2f);
                Main.dust[newDust].position = position;
                Main.dust[newDust].fadeIn = 0.5f;
                Main.dust[newDust].noGravity = true;

                NPC.localAI[0]++;

                if (NPC.localAI[0] >= 140)
                {
                    SoundEngine.PlaySound(SoundID.Item14 with { Pitch = -0.5f }, NPC.Center);

                    //flame dusts
					for (int numDust = 0; numDust < 45; numDust++)
					{
						int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
						Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-7f, 8f);
						Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-4f, 5f);
						Main.dust[dustGore].noGravity = true;
					}

					//explosion smoke
					for (int numExplosion = 0; numExplosion < 5; numExplosion++)
					{
						int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, 0.45f);
						Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-1f, 2f);
						Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-1f, 2f);
						Main.dust[DustGore].noGravity = true;
					}

                    foreach (Player playerToHurt in Main.ActivePlayers)
					{
						if (!playerToHurt.dead && playerToHurt.Distance(NPC.Center) <= 135f)
						{
							playerToHurt.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.JackInTheBoxExplosion").ToNetworkText(playerToHurt.name)), NPC.damage + Main.rand.Next(0, 30), 0);
						}
					}

                    player.ApplyDamageToNPC(NPC, NPC.lifeMax, 0, 0, false, null, true);
                }
            }
        }

        public bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(NPC.Center.X / 16f) - tileWidth;
            if (NPC.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JackInTheBoxGore" + numGores).Type);
                    }
                }
            }
        }
    }
}