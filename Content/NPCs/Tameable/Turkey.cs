using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Mounts;
using Spooky.Content.Items.BossSummon;

namespace Spooky.Content.NPCs.Tameable
{
	public class Turkey : ModNPC
	{
		int numSeedsEaten = 0;
		int SoundTimer = 0;
		int FallTimer = 0;
		int RunTimer = 0;

        bool PeckingGround = false;
        bool RunningFast = false;
		bool FlyToPlayer = false;

		public static readonly SoundStyle GobbleSound1 = new("Spooky/Content/Sounds/TurkeyGobble1", SoundType.Sound) { PitchVariance = 0.5f };
		public static readonly SoundStyle GobbleSound2 = new("Spooky/Content/Sounds/TurkeyGobble2", SoundType.Sound) { PitchVariance = 0.5f };
		public static readonly SoundStyle FlapSound = new("Spooky/Content/Sounds/TurkeyFlap", SoundType.Sound) { Volume = 0.2f };

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 18;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
			NPCID.Sets.TakesDamageFromHostilesWithoutBeingFriendly[NPC.type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//ints
			writer.Write(numSeedsEaten);
			writer.Write(SoundTimer);
			writer.Write(FallTimer);
			writer.Write(RunTimer);

			//bools
			writer.Write(PeckingGround);
			writer.Write(RunningFast);
			writer.Write(FlyToPlayer);

			//floats
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
			writer.Write(NPC.localAI[3]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//ints
			numSeedsEaten = reader.ReadInt32();
			SoundTimer = reader.ReadInt32();
			FallTimer = reader.ReadInt32();
			RunTimer = reader.ReadInt32();

			//bools
			PeckingGround = reader.ReadBoolean();
			RunningFast = reader.ReadBoolean();
			FlyToPlayer = reader.ReadBoolean();

			//floats
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
			NPC.localAI[3] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 20;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 18;
			NPC.height = 40;
            NPC.npcSlots = 0.5f;
			NPC.noGravity = false;
			NPC.chaseable = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 7;
			AIType = NPCID.Bunny;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Turkey"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            NPC.frameCounter++;

            if (NPC.localAI[1] > 0 || FlyToPlayer)
            {
                //falling wing flapping animation
                if (NPC.frame.Y < frameHeight * 10)
                {
                    NPC.frame.Y = 10 * frameHeight;
                }

                if (NPC.frameCounter > 3)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 14)
                {
                    NPC.frame.Y = 10 * frameHeight;
                }
            }
            else
            {
                //walking animation
                if (NPC.velocity.X != 0)
                {
                    //regular walking
                    if (!RunningFast)
                    {
                        if (NPC.frameCounter > 5)
                        {
                            NPC.frame.Y = NPC.frame.Y + frameHeight;
                            NPC.frameCounter = 0;
                        }
                        if (NPC.frame.Y >= frameHeight * 5)
                        {
                            NPC.frame.Y = 0 * frameHeight;
                        }
                    }
                    //running fast
                    else
                    {
                        if (NPC.frame.Y < frameHeight * 15)
                        {
                            NPC.frame.Y = 14 * frameHeight;
                        }

                        if (NPC.frameCounter > 3)
                        {
                            NPC.frame.Y = NPC.frame.Y + frameHeight;
                            NPC.frameCounter = 0;
                        }
                        if (NPC.frame.Y >= frameHeight * 18)
                        {
                            NPC.frame.Y = 14 * frameHeight;
                        }
                    }
                }
                //idle animations
                else
                {
                    //standing still
                    if (!PeckingGround)
                    {
                        NPC.frame.Y = 5 * frameHeight;
                    }
                    //pecking animation
                    else
                    {
                        if (NPC.frame.Y < frameHeight * 5)
                        {
                            NPC.frame.Y = 5 * frameHeight;
                        }

                        if (NPC.frameCounter > 4)
                        {
                            NPC.frame.Y = NPC.frame.Y + frameHeight;
                            NPC.frameCounter = 0;
                        }
                        if (NPC.frame.Y >= frameHeight * 10)
                        {
                            NPC.frame.Y = 8 * frameHeight;
                        }
                    }
                }
            }
		}

		public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
		{
			RunningFast = true;
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			RunningFast = true;
		}

		public override bool CheckActive()
		{
			return !NPC.GetGlobalNPC<NPCGlobal>().NPCTamed; //has to be the opposite, return false = npc will not despawn
		}

		public override bool NeedSaving()
		{
			return NPC.GetGlobalNPC<NPCGlobal>().NPCTamed;
		}

		public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
		{
			npcHitbox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width * 2, NPC.height);

			return false;
		}

		public override void AI()
        {
			//sprite direction stuff
			if (NPC.velocity.X < 0)
			{
				NPC.spriteDirection = -1;
			}
			else if (NPC.velocity.X > 0)
			{
				NPC.spriteDirection = 1;
			}
			else
			{
				NPC.spriteDirection = NPC.direction;
			}

			if (Main.rand.NextBool(2000) && !NPC.GetGlobalNPC<NPCGlobal>().NPCTamed)
            {
                switch (numSeedsEaten)
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

			//constantly call stepup collision so it doesnt get stuck on blocks
			Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

			//limit Y-velocity and play flapping animation while falling
			if (NPC.velocity.Y > 2)
			{
				NPC.velocity.Y = 2;
				NPC.localAI[1] = 1;
			}
			else
			{
				NPC.localAI[1] = 0;
			}

			//randomly do ground pecking animation while idle
			if (Main.rand.NextBool(150) && NPC.velocity.X == 0 && !PeckingGround)
            {
                PeckingGround = true;
                NPC.netUpdate = true;
            }

			//pecking animation timers
            if (PeckingGround)
            {
                NPC.localAI[0]++;
                if (NPC.localAI[0] >= Main.rand.Next(60, 120))
                {
                    NPC.localAI[0] = 0;
                    PeckingGround = false;
                    NPC.netUpdate = true;
                }
            }

			//randomly play a gobble sound
			if (Main.rand.NextBool(500))
            {
				if (Main.rand.NextBool())
				{
					SoundEngine.PlaySound(GobbleSound1, NPC.Center);
				}
				else
				{
					SoundEngine.PlaySound(GobbleSound2, NPC.Center);
				}
			}

			//if falling, increase a timer and play flapping sound
			if (NPC.velocity.Y != 0)
			{
				FallTimer++;
				if (FallTimer > 30)
				{
					if (SoundTimer == 0)
					{
						SoundEngine.PlaySound(FlapSound, NPC.Center);
						SoundTimer = 15;

						NPC.netUpdate = true;
					}
					else
					{
						SoundTimer--;
					}
				}
			}
			else
			{
				FallTimer = 0;
				SoundTimer = 0;
			}

			//right click functionality
			Rectangle RealHitbox = new Rectangle((int)(NPC.Center.X - 34), (int)NPC.position.Y, 68, NPC.height);
			foreach (Player player in Main.ActivePlayers)
			{
				if (RealHitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 1, 1)) &&
				NPC.Distance(player.Center) <= 100f && !Main.mapFullscreen)
				{
					if (Main.mouseRight && Main.mouseRightRelease && !RunningFast)
					{
						if (!NPC.GetGlobalNPC<NPCGlobal>().NPCTamed)
						{
							if (ItemGlobal.ActiveItem(player).type == ModContent.ItemType<RottenSeed>() && player.ConsumeItem(ModContent.ItemType<RottenSeed>()))
							{
								SoundEngine.PlaySound(SoundID.Item2, NPC.Center);

								for (int i = 0; i < 20; i++)
								{
									Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FoodPiece, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, default, new Color(211, 109, 58), 0.75f);
								}

								if (numSeedsEaten < 4)
								{
									numSeedsEaten++;
									NPC.netUpdate = true;
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

								NPC.netUpdate = true;
							}
						}
						else
						{
							if (!player.HasBuff(ModContent.BuffType<TurkeyMountBuff>()))
							{
								player.Center = NPC.Center - new Vector2(0, 15);
								player.AddBuff(ModContent.BuffType<TurkeyMountBuff>(), 2);
								NPC.life = 0;

								if (Main.netMode == NetmodeID.Server)
								{
									NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI, 0f, 0f, 0f, 0);
								}
							}
						}
					}
				}
			}

			//tamed turkey behavior
			if (NPC.GetGlobalNPC<NPCGlobal>().NPCTamed)
			{
				NPC.friendly = true; //prevents players from killing the turkey

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
						NPC.aiStyle = 7;
						AIType = NPCID.Bunny;
						break;
					}
					//following
					case 1:
					{
						NPC.aiStyle = -1;

						NPC.TargetClosest(true);
						Player player = Main.player[NPC.target];

						if (!FlyToPlayer)
						{
							NPC.noTileCollide = false;
							NPC.noGravity = false;

							Vector2 vector48 = player.Center - NPC.Center;
							float playerDistance = vector48.Length();

							if (NPC.velocity.Y == 0 && ((HoleBelow() && playerDistance > 100f) || (playerDistance > 100f && NPC.position.X == NPC.oldPosition.X)))
							{
								NPC.velocity.Y = -5f;
							}

							if (NPC.velocity.X < -4f || NPC.velocity.X > 4f)
							{
								RunningFast = true;
							}
							else
							{
								RunningFast = false;
							}

							if (playerDistance > 450f)
							{
								FlyToPlayer = true;
								NPC.velocity = Vector2.Zero;
								NPC.netUpdate = true;
							}

							if (playerDistance > 75f)
							{
								if (player.position.X - NPC.position.X > 0f)
								{
									NPC.velocity.X += 0.1f;
									if (NPC.velocity.X > 5f)
									{
										NPC.velocity.X = 5f;
									}
								}
								else
								{
									NPC.velocity.X -= 0.1f;
									if (NPC.velocity.X < -5f)
									{
										NPC.velocity.X = -5f;
									}
								}
							}

							//slow down more when super close
							if (playerDistance < 50f)
							{
								NPC.velocity.X *= 0.75f;
							}
						}
						else
						{
							NPC.noTileCollide = true;
							NPC.noGravity = true;

							Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 12;
           				 	NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

							if (NPC.Hitbox.Intersects(player.Hitbox))
							{
								FlyToPlayer = false;
								NPC.netUpdate = true;
							}
						}

						break;
					}
					//staying put
					case 2:
					{
						NPC.aiStyle = 0;
						AIType = NPCID.BoundMechanic;
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
					bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
					if (lineOfSight && player.Distance(NPC.Center) <= 250 && ItemGlobal.ActiveItem(player).type == ModContent.ItemType<RottenSeed>())
					{
						NPC.TargetClosest(true);
					}
				}

				//when hit, the turkey should run super fast but multiplying its X-velocity
				if (RunningFast)
				{
					NPC.velocity.X = NPC.velocity.X * 1.5f;
					NPC.velocity.X = MathHelper.Clamp(NPC.velocity.X, -6f, 6f);

					//dont allow the turkey to randomly stop while running
					if (NPC.velocity.X == 0)
					{
						NPC.velocity.X = 2 * NPC.direction;
					}

					//reverse direction on collision with tiles immediately
					if (NPC.collideX)
					{
						NPC.velocity.X *= -1;
					}

					RunTimer++;
					if (RunTimer > 300)
					{
						RunningFast = false;
						RunTimer = 0;

						NPC.netUpdate = true;
					}
				}
			}
        }

		private bool HoleBelow()
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
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TurkeyGore" + numGores).Type);
                    }
                }

                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/TurkeyFeatherGore").Type);
                    }
                }
            }
        }
	}
}