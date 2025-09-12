using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Tameable
{
	public class LittleDunk : ModNPC
	{
		int numChumEaten = 0;

		int CurrentFrameX = 0; //0 = swim animation  1 = open mouth animation

		bool FollowPlayerHoldingFood = false;
		bool InitializePosition = false;

		Vector2 SavePreviousPosition = Vector2.Zero;

		Player TargetedPlayer = null;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 8;
			NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/LittleDunkBestiary"
            };
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//vector2
			writer.WriteVector2(SavePreviousPosition);

			//ints
			writer.Write(numChumEaten);

			//bools
			writer.Write(FollowPlayerHoldingFood);
			writer.Write(InitializePosition);

			//floats
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
			writer.Write(NPC.localAI[3]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//vector2
			SavePreviousPosition = reader.ReadVector2();

			//ints
			numChumEaten = reader.ReadInt32();

			//bools
			FollowPlayerHoldingFood = reader.ReadBoolean();
			InitializePosition = reader.ReadBoolean();

			//floats
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
			NPC.localAI[3] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 50;
			NPC.height = 50;
			NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.waterMovementSpeed = 1f;
			NPC.noTileCollide = false;
			NPC.noGravity = true;
			NPC.friendly = true;
			NPC.chaseable = false;
			NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ZombieOceanBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LittleDunk"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ZombieOceanBiome>().ModBiomeBestiaryInfoElement),
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			var effects = NPC.velocity.X > 0f ? SpriteEffects.None : SpriteEffects.FlipVertically;

			//draw body
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
        }

		public override void FindFrame(int frameHeight)
        {
			if (Main.netMode != NetmodeID.Server)
            {
                NPC.frame.Width = TextureAssets.Npc[NPC.type].Width() / 2;
            }

            NPC.frame.X = (int)(NPC.frame.Width * CurrentFrameX);

            NPC.frameCounter++;

			if (NPC.frameCounter > 8 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
				NPC.frame.Y = NPC.frame.Y + frameHeight;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y >= frameHeight * 8)
			{
				NPC.frame.Y = 0 * frameHeight;
			}
        }

		public override bool CheckActive()
        {
            return false;
        }

		public override bool NeedSaving()
		{
			return true;
		}

		public override void AI()
		{
			//rotation stuff
			NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.TwoPi;

			if (Main.rand.NextBool(2000) && !NPC.GetGlobalNPC<NPCGlobal>().NPCTamed)
            {
                switch (numChumEaten)
                {
                    case 0:
                    {
                        EmoteBubble.NewBubble(EmoteID.Starving, new WorldUIAnchor(NPC), 200);
                        break;
                    }
                    case 1:
                    {
                        goto case 0;
                    }
                    case 2:
                    {
                    	EmoteBubble.NewBubble(EmoteID.Hungry, new WorldUIAnchor(NPC), 200);
						break;
                    }
                    case 3:
                    {
                        goto case 2;
                    }
					case 4:
                    {
                        EmoteBubble.NewBubble(EmoteID.Peckish, new WorldUIAnchor(NPC), 200);
						break;
                    }
                }
            }

			//right click functionality
			Rectangle RealHitbox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
			foreach (Player player in Main.ActivePlayers)
			{
				if (RealHitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 1, 1)) &&
				NPC.Distance(player.Center) <= 100f && !Main.mapFullscreen && Main.myPlayer == player.whoAmI)
				{
					if (Main.mouseRight && Main.mouseRightRelease)
					{
						if (!NPC.GetGlobalNPC<NPCGlobal>().NPCTamed)
						{
							if (ItemGlobal.ActiveItem(player).type == ItemID.ChumBucket && player.ConsumeItem(ItemID.ChumBucket))
							{
								SoundEngine.PlaySound(SoundID.Item2, NPC.Center);

								for (int i = 0; i < 20; i++)
								{
									Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FoodPiece, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, default, new Color(211, 109, 58), 0.75f);
								}

								if (numChumEaten < 5)
								{
									numChumEaten++;
								}
								else
								{
									SoundEngine.PlaySound(SoundID.ResearchComplete with { Volume = 0.5f }, NPC.Center);

									for (int i = 0; i < 20; i++)
									{
										int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<CartoonHeart>(), 0f, -2f, 0, default, 0.5f);
										Main.dust[newDust].velocity.X = Main.rand.NextFloat(-1.5f, 1.5f);
										Main.dust[newDust].velocity.Y = Main.rand.NextFloat(-0.2f, -0.02f);
										Main.dust[newDust].noGravity = true;
									}

									NPC.GetGlobalNPC<NPCGlobal>().NPCTamed = true;
								}
							}
						}
					}
				}
			}

			if (NPC.wet)
			{
				//tamed behavior
				if (NPC.GetGlobalNPC<NPCGlobal>().NPCTamed)
				{
					CurrentFrameX = 0;

					if (Main.rand.NextBool(50))
					{
						//spawn heart particles
						int newDust = Dust.NewDust(NPC.position, NPC.width, NPC.height / 4, ModContent.DustType<CartoonHeart>(), 0f, -2f, 0, default, 0.5f);
						Main.dust[newDust].velocity.X = Main.rand.NextFloat(-1.5f, 1.5f);
						Main.dust[newDust].velocity.Y = Main.rand.NextFloat(-0.2f, -0.02f);
						Main.dust[newDust].alpha = Main.rand.Next(0, 2);
						Main.dust[newDust].noGravity = true;
					}

					switch ((int)NPC.localAI[2])
					{
						//roaming
						case 0:
						{
							InitializePosition = false;

							NPC.aiStyle = 16;
							AIType = NPCID.Goldfish;
							break;
						}
						//following
						case 1:
						{
							InitializePosition = false;

							if (TargetedPlayer == null)
							{
								foreach (Player player in Main.ActivePlayers)
								{
									if (!player.dead && Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height) && player.wet)
									{
										TargetedPlayer = player;
										break;
									}
									else
									{
										TargetedPlayer = null;
										continue;
									}
								}
							}

							//go to the player if there is no line of sight
							if (TargetedPlayer != null)
							{
								if (!TargetedPlayer.wet)
								{
									NPC.aiStyle = 16;
									AIType = NPCID.Goldfish;
								}
								else
								{
									NPC.aiStyle = -1;

									bool HasLineOfSightToTarget = Collision.CanHitLine(TargetedPlayer.position, TargetedPlayer.width, TargetedPlayer.height, NPC.position, NPC.width, NPC.height);

									if (!HasLineOfSightToTarget)
									{
										NPC.aiStyle = 16;
										AIType = NPCID.Goldfish;
									}
									else
									{
										Vector2 PlayerGoTo = TargetedPlayer.Center - new Vector2(0, 30);

										if (NPC.Distance(PlayerGoTo) >= 100)
										{
											float vel = MathHelper.Clamp(NPC.Distance(PlayerGoTo) / 12, 0.1f, NPC.Distance(TargetedPlayer.Center) / 50f);
											NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(PlayerGoTo) * vel, 0.08f);
										}
									}
								}
							}
							else
							{
								NPC.aiStyle = 16;
								AIType = NPCID.Goldfish;
							}

							break;
						}
						//staying put
						case 2:
						{
							NPC.aiStyle = -1;

							if (!InitializePosition)
							{
								SavePreviousPosition = NPC.Center;
								InitializePosition = true;
							}
							else
							{
								Vector2 GoTo = SavePreviousPosition - new Vector2(0, 30);

								if (NPC.Distance(GoTo) >= 100)
								{
									float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 0.1f, NPC.Distance(SavePreviousPosition) / 50f);
									NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
								}
							}

							break;
						}
					}
				}
				//untamed behavior
				else
				{
					//follow players around if they holding a rotten seed
					foreach (Player player in Main.ActivePlayers)
					{
						bool HasLineOfSightToTarget = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
						if (HasLineOfSightToTarget && player.Distance(NPC.Center) <= 250 && player.wet && ItemGlobal.ActiveItem(player).type == ItemID.ChumBucket)
						{
							TargetedPlayer = player;
							FollowPlayerHoldingFood = true;
							break;
						}
						else
						{
							TargetedPlayer = null;
							FollowPlayerHoldingFood = false;
							continue;
						}
					}

					if (FollowPlayerHoldingFood && TargetedPlayer != null)
					{
						CurrentFrameX = 1;

						Vector2 PlayerGoTo = TargetedPlayer.Center - new Vector2(0, 30);

						if (NPC.Distance(PlayerGoTo) >= 100)
						{
							NPC.aiStyle = -1;

							float vel = MathHelper.Clamp(NPC.Distance(PlayerGoTo) / 12, 0.1f, NPC.Distance(TargetedPlayer.Center) / 50f);
							NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(PlayerGoTo) * vel, 0.08f);
						}
					}
					else
					{
						CurrentFrameX = 0;

						NPC.aiStyle = 16;
						AIType = NPCID.Goldfish;
					}
				}
			}
			else
			{
				NPC.aiStyle = 16;
				AIType = NPCID.Goldfish;
			}
		}
	}
}