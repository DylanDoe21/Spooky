using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Localization;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave.Furniture;

namespace Spooky.Content.NPCs.Friendly
{
    public class MushGnome1 : ModNPC  
    {
        int TimeBeforeEnterHouse = 0;
        Vector2 SavePosition;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.defense = 5;
            NPC.width = 34;
			NPC.height = 46;
            NPC.friendly = true;
            NPC.noGravity = false;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.75f;
            NPC.HitSound = SoundID.DD2_GoblinScream with { Pitch = 1f, Volume = 0.4f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MushGnome1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            //walking animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //jumping/falling frame
            if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
            {
                NPC.frame.Y = 4 * frameHeight;
            }

            //still frame
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        /*
        public override bool CanChat() 
        {
			return true;
		}

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override string GetChat()
		{
			return Language.GetTextValue("Mods.Spooky.Dialogue.PartySkeleton.Dialogue" + dialogueStyle.ToString());
		}
        */

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction = NPC.velocity.X <= 0 ? -1 : 1;

            //use ai[2] to keep track of how long the gnome exists before it can go back in its house
            if (NPC.ai[0] > 0)
            {
                NPC.ai[2]++;
                if (NPC.ai[2] >= TimeBeforeEnterHouse)
                {
                    ushort[] MushroomHouses = new ushort[] { (ushort)ModContent.TileType<GnomeHouse1>(), (ushort)ModContent.TileType<GnomeHouse2>(),
                    (ushort)ModContent.TileType<GnomeHouse3>(), (ushort)ModContent.TileType<GnomeHouse4>() };

                    int TileX = (int)SavePosition.X / 16;
                    int TileY = (int)SavePosition.Y / 16;

                    if (NPC.Distance(SavePosition) <= 12f && MushroomHouses.Contains(Main.tile[TileX, TileY].TileType))
                    {
                        NPC.active = false;
                    }
                }
            }

            switch ((int)NPC.ai[0])
            {
                //save position and time before it can go back in its house
                case 0:
                {
                    SavePosition = NPC.Center;
                    TimeBeforeEnterHouse = Main.rand.Next(5, 15) * 60;
                    NPC.ai[0]++;
                    NPC.netUpdate = true;

                    break;
                }
                
                case 1:
                {
                    MoveBackAndFourth(SavePosition, 1f, 0.025f, 150, true);

                    if (Main.rand.NextBool(1000))
                    {
                        NPC.ai[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }

                case 2:
                {
                    NPC.ai[1]++;

                    NPC.velocity.X = 0;

                    if (NPC.ai[1] >= 420)
                    {
                        NPC.ai[1] = 0;
                        NPC.ai[0] = 1;
                        NPC.netUpdate = true;
                    }

                    break;
                }
            }
        }

        public void MoveBackAndFourth(Vector2 Center, float MaxSpeed, float Acceleration, int Distance, bool ResetCheck)
        {
            //prevents the pet from getting stuck on sloped tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

            Vector2 center2 = NPC.Center;
            Vector2 vector48 = Center - center2;
            float CenterDistance = vector48.Length();

            if (CenterDistance > Distance * 2 && NPC.velocity.Y == 0 && ResetCheck)
            {
                NPC.ai[0] = 0;
            }

            if (NPC.collideX || (NPC.velocity.Y == 0 && HoleBelow() && CenterDistance > 100f))
            {
                NPC.velocity.X = -NPC.velocity.X;
            }

            NPC.velocity.Y += 0.1f;

            if (NPC.velocity.Y > 10f)
            {
                NPC.velocity.Y = 10f;
            }

            if (CenterDistance > Distance)
            {
                if (Center.X - NPC.position.X > 0f)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
            else
            {
                if (NPC.velocity.X >= 0)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
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
    }

    public class MushGnome2 : MushGnome1  
    {
        Vector2 SavePosition;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MushGnome2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
    }

    public class MushGnome3 : MushGnome1  
    {
        Vector2 SavePosition;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MushGnome3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
    }

    public class MushGnome4 : MushGnome1  
    {
        Vector2 SavePosition;

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.MushGnome4"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
    }
}