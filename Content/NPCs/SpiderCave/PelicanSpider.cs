using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class PelicanSpider1 : ModNPC  
    {
		public int SwimmingHeightOffset = 28;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/PelicanSpiderBrownBestiary",
				Position = new Vector2(0f, 30f),
				PortraitPositionYOverride = 30f
			};
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 400;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 42;
			NPC.height = 86;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.2f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = -0.65f };
			NPC.DeathSound = SoundID.NPCDeath26 with { Pitch = -0.5f };
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PelicanSpider1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

        public override void FindFrame(int frameHeight)
		{
			if (NPC.localAI[0] == 0)
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
			else
			{
				//running animation
				if (NPC.velocity.Y == 0)
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 7 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}

					if (NPC.frame.Y < frameHeight * 5)
					{
						NPC.frame.Y = 4 * frameHeight;
					}

					if (NPC.frame.Y >= frameHeight * 9)
					{
						NPC.frame.Y = 4 * frameHeight;
					}
				}

				//jumping frame
				if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 1)
					{
						NPC.frame.Y = NPC.frame.Y + frameHeight;
						NPC.frameCounter = 0;
					}

					if (NPC.frame.Y < frameHeight * 10)
					{
						NPC.frame.Y = 9 * frameHeight;
					}

					if (NPC.frame.Y >= frameHeight * 12)
					{
						NPC.frame.Y = 9 * frameHeight;
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, new Vector2(NPC.Center.X, NPC.Center.Y - SwimmingHeightOffset) - screenPos + new Vector2(0, NPC.gfxOffY + 4), 
			NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2f, 1f, effects, 0f);

			return false;
		}

		public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
		{
			npcHitbox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y - SwimmingHeightOffset, NPC.width, NPC.height);

			return false;
		}

		public override void AI()
		{
			int CurrentDirection = NPC.direction;
			int CurrentTarget = NPC.target;

			NPC.TargetClosest();
			Player player = Main.player[NPC.target];

			if (NPC.localAI[0] == 0)
			{
				SwimmingHeightOffset = 28;

				NPC.aiStyle = -1;
				NPC.noGravity = true;

				if (CurrentTarget >= 0 && CurrentDirection != 0)
				{
					NPC.direction = CurrentDirection;
				}

				bool HasLineOfSight = NPC.Distance(player.Center) <= 200f && Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
				if (HasLineOfSight || !NPC.wet)
				{
					NPC.velocity.Y = -12;

					NPC.localAI[0]++;
					NPC.netUpdate = true;
				}
				else
				{
					if (NPC.wet && WorldGen.InWorld((int)(NPC.Center.X + (float)((NPC.width / 2 + 8) * NPC.direction)) / 16, (int)(NPC.Center.Y / 16f), 5))
					{
						float MovementSpeed = 2f;

						NPC.velocity.X = (NPC.velocity.X * 19f + MovementSpeed * (float)NPC.direction) / 20f;

						int NPCCenterX = (int)(NPC.Center.X + (float)((NPC.width / 2 + 8) * NPC.direction)) / 16;
						int NPCPositionY = (int)((NPC.position.Y + (float)NPC.height) / 16f);
						int NPCCenterY = (int)(NPC.Center.Y / 16f);
						int TileYPos = (int)(NPC.position.Y / 16f);

						Tile tile1 = Main.tile[NPCCenterX, NPCCenterY];
						Tile tile2 = Main.tile[NPCCenterX, NPCPositionY];

						if (tile1 == null)
						{
							tile1 = new Tile();
						}
						if (tile2 == null)
						{
							tile2 = new Tile();
						}
						if (NPCCenterX < 5 || NPCCenterX > Main.maxTilesX - 5 || WorldGen.SolidTile(NPCCenterX, NPCCenterY) || WorldGen.SolidTile(NPCCenterX, TileYPos) || WorldGen.SolidTile(NPCCenterX, NPCCenterY) || tile2.LiquidAmount == 0)
						{
							if (NPC.ai[0] == 0)
							{
								NPC.direction *= -1;
							}
							else
							{
								NPC.velocity.X = 0;
							}
						}

						NPC.spriteDirection = NPC.direction;

						if (NPC.velocity.Y > 0f)
						{
							NPC.velocity.Y *= 0.5f;
						}

						NPCCenterX = (int)(NPC.Center.X / 16f);
						NPCCenterY = (int)(NPC.Center.Y / 16f);
						float NPCBottom = NPC.position.Y + (float)NPC.height;

						Tile tile3 = Main.tile[NPCCenterX, NPCCenterY - 1];
						Tile tile4 = Main.tile[NPCCenterX, NPCCenterY];
						Tile tile5 = Main.tile[NPCCenterX, NPCCenterY + 1];

						if (tile3 == null)
						{
							tile3 = new Tile();
						}
						if (tile4 == null)
						{
							tile4 = new Tile();
						}
						if (tile5 == null)
						{
							tile5 = new Tile();
						}

						if (tile3.LiquidAmount > 0)
						{
							NPCBottom = NPCCenterY * 16;
							NPCBottom -= (float)(tile3.LiquidAmount / 16);
						}
						else if (tile4.LiquidAmount > 0)
						{
							NPCBottom = (NPCCenterY + 1) * 16;
							NPCBottom -= (float)(tile4.LiquidAmount / 16);
						}
						else if (tile5.LiquidAmount > 0)
						{
							NPCBottom = (NPCCenterY + 2) * 16;
							NPCBottom -= (float)(tile5.LiquidAmount / 16);
						}

						NPCBottom -= 6f;
						if (NPC.Center.Y > NPCBottom)
						{
							NPC.velocity.Y -= 0.1f;
							if (NPC.velocity.Y < -8f)
							{
								NPC.velocity.Y = -8f;
							}
							if (NPC.Center.Y + NPC.velocity.Y < NPCBottom)
							{
								NPC.velocity.Y = NPCBottom - NPC.Center.Y;
							}
						}
						else
						{
							NPC.velocity.Y = NPCBottom - NPC.Center.Y;
						}
					}
				}
			}
			else
			{
				SwimmingHeightOffset = 0;

				NPC.aiStyle = 26;
				NPC.noGravity = false;

				if (NPC.velocity.Y > 0)
				{   
					NPC.localAI[2]++;

					if (NPC.localAI[2] >= 10)
					{
						NPC.velocity.Y *= 0.85f;
					}
				}
				else
				{
					NPC.localAI[2] = 0;
				}

				NPC.localAI[1]++;

				if (NPC.wet)
				{
					if (NPC.localAI[1] >= 60)
					{
						NPC.velocity.Y -= 1;
					}

					if (NPC.localAI[1] >= 300)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.netUpdate = true;
					}
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
				Vector2 OffsetCenter = new Vector2(NPC.Center.X, NPC.Center.Y - SwimmingHeightOffset);

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), OffsetCenter, NPC.velocity, ModContent.Find<ModGore>("Spooky/PelicanSpiderBrownGore" + numGores).Type);
                    }
                }

				for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), OffsetCenter, NPC.velocity, ModContent.Find<ModGore>("Spooky/PelicanSpiderWingGore").Type);
                    }
                }
            }
        }
	}

	public class PelicanSpider2 : PelicanSpider1  
    {
		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 12;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/PelicanSpiderBrownBestiary",
				Position = new Vector2(0f, 30f),
				PortraitPositionYOverride = 30f
			};
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PelicanSpider2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
				Vector2 OffsetCenter = new Vector2(NPC.Center.X, NPC.Center.Y - SwimmingHeightOffset);

                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), OffsetCenter, NPC.velocity, ModContent.Find<ModGore>("Spooky/PelicanSpiderRedGore" + numGores).Type);
                    }
                }

				for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), OffsetCenter, NPC.velocity, ModContent.Find<ModGore>("Spooky/PelicanSpiderWingGore").Type);
                    }
                }
            }
        }
	}
}