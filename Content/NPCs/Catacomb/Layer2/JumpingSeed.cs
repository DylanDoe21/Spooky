using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
	public class JumpingSeed1 : ModNPC
	{
		float destinationX = 0f;
		float destinationY = 0f;

		bool Emerge = false;
		bool Jumping = false;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Frame = 3
            };

			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
		}

		public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(Emerge);
			writer.Write(Jumping);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			Emerge = reader.ReadBoolean();
			Jumping = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
			NPC.lifeMax = 90;
            NPC.damage = 35;
            NPC.defense = 0;
			NPC.width = 56;
			NPC.height = 44;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JumpingSeed1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
		{
			//still frame
			if (!Emerge && !Jumping)
			{
				NPC.frame.Y = 0 * frameHeight;
			}

			//emerging animation
			if (Emerge && !Jumping)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 7)
            	{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 3)
				{
					NPC.frame.Y = 2 * frameHeight;
				}
			}

			//jumping animations
			if (Jumping)
			{
				//jumping and falling frame
				if (NPC.ai[0] < 40)
				{
					if (NPC.velocity.Y < 0)
					{
						NPC.frame.Y = 3 * frameHeight;
					}
					if (NPC.velocity.Y > 0)
					{
						NPC.frame.Y = 4 * frameHeight;
					}
				}

				//play the animation in reverse when going into the ground
				if (NPC.ai[0] >= 40 && NPC.velocity.Y == 0)
				{
					NPC.frameCounter++;
					if (NPC.frameCounter > 7)
					{
						NPC.frame.Y = NPC.frame.Y - frameHeight;
						NPC.frameCounter = 0;
					}

					//this checks specifically for one frame past the max frame count to make it invisible before finding a new place to re-emerge
					if (NPC.frame.Y <= frameHeight * -1)
					{
						NPC.frame.Y = -1 * frameHeight;
					}
				}
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
			//dont hit players if finding a new location
            return NPC.frame.Y != -1 * NPC.height;
        }

        public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			if (NPC.Distance(player.Center) > 120f && !Emerge && !Jumping)
            {
				NPC.velocity.X *= 0;
			}

			if (NPC.Distance(player.Center) <= 120f && !Emerge && !Jumping)
            {
				Emerge = true;
			}

			if (NPC.frame.Y == 2 * NPC.height)
			{
				NPC.aiStyle = 66;

				if (!Jumping)
				{
					Emerge = false;
					Jumping = true;
				}
			}

			if (Jumping)
			{
				NPC.ai[0]++;

				//jump up
				if (NPC.ai[0] == 5)
				{
					Vector2 JumpTo = new(player.Center.X, NPC.Center.Y - 150);

					Vector2 velocity = JumpTo - NPC.Center;
            		velocity.Normalize();

					float speed = MathHelper.Clamp(velocity.Length() / 36, 10, 12);

					NPC.velocity = velocity * speed;
				}


				//find a new location to teleport to
				if (NPC.ai[0] == 40 && destinationX == 0f && destinationY == 0f)
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

									bool ValidForTeleport = Main.tileSolid[(int)Main.tile[num92, num94].TileType];

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

				//dust telegraph at new location
				if (NPC.ai[0] >= 40 && NPC.ai[0] <= 100 && NPC.velocity.Y == 0 && destinationX != 0f && destinationY != 0f)
				{
					NPC.dontTakeDamage = true;

					float PositionX = destinationX * 16f - (float)(NPC.width / 2) + 8f;
					float PositionY = destinationY * 16f - (float)NPC.height;

					Dust dust = Dust.NewDustDirect(new Vector2(PositionX, PositionY - 5), NPC.width, NPC.height, DustID.Grass, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-2f, 0f), 50, default, 1.5f);
					dust.noGravity = true;
				}

				//teleport and reset ai
				if (NPC.ai[0] >= 110 && destinationX != 0f && destinationY != 0f)
				{
					NPC.dontTakeDamage = false;

					NPC.position.X = destinationX * 16f - (float)(NPC.width / 2) + 8f;
					NPC.position.Y = destinationY * 16f - (float)NPC.height;
					destinationX = 0f;
					destinationY = 0f;

					NPC.velocity *= 0;
					
					SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

					for (int numDusts = 0; numDusts < 15; numDusts++)
					{
						Dust dust = Dust.NewDustDirect(new Vector2(NPC.position.X, NPC.position.Y + 24), NPC.width, NPC.height, DustID.Grass, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-2f, 0f), 50, default, 1.5f);
						dust.noGravity = true;
					}

					NPC.ai[0] = 0;

					Emerge = false;
					Jumping = false;
					
					NPC.netUpdate = true;
				}
			}
        }

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantChunk>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSeed1Gore").Type);
				}

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSeedLeafGore").Type);
                    }
                }
            }
        }
    }

	public class JumpingSeed2 : JumpingSeed1
	{
		public override void SetDefaults()
		{
			NPC.lifeMax = 90;
            NPC.damage = 35;
            NPC.defense = 0;
			NPC.width = 56;
			NPC.height = 48;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
			NPC.noGravity = false;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.Grass;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JumpingSeed2"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSeed2Gore").Type);
				}

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSeedLeafGore").Type);
                    }
                }
            }
        }
	}

	public class JumpingSeed3 : JumpingSeed1
	{
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.JumpingSeed3"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSeed3Gore").Type);
				}

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/JumpingSeedLeafGore").Type);
                    }
                }
            }
        }
	}
}