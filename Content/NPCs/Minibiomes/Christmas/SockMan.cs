using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
	public class SockManGreen : ModNPC
	{
        Vector2 SavePosition;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 7;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
			SavePosition = reader.ReadVector2();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 0;
			NPC.defense = 10;
			NPC.width = 26;
			NPC.height = 50;
            NPC.npcSlots = 0.5f;
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SockManGreen"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 6)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
        {
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
                    NPC.velocity.Y = -8f;
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
                if (Main.netMode != NetmodeID.Server) 
                {
                    //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ShroomHopperGore").Type);
                }
            }
        }
	}

    public class SockManOrange : SockManGreen
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SockManOrange"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}
    }

    public class SockManRed : SockManGreen
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SockManRed"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}
    }
}