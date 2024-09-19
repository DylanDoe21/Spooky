using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class BanditWizard : ModNPC
	{
		float addedStretch = 0f;
		float stretchRecoil = 0f;

		bool Shake = false;

		Vector2 SavePlayerPosition;

		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
			//vector2
			writer.WriteVector2(SavePlayerPosition);

            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
			SavePlayerPosition = reader.ReadVector2();

            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 2750;
            NPC.damage = 40;
			NPC.defense = 0;
			NPC.width = 66;
			NPC.height = 98;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit54;
            NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = -1;
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * bossAdjustment);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;
			
			//limit how much it can stretch
			if (stretch > 0.5f)
			{
				stretch = 0.5f;
			}

			//limit how much it can squish
			if (stretch < -0.5f)
			{
				stretch = -0.5f;
			}

			Vector2 scaleStretch = new Vector2(1f + stretch, 1f - stretch);

			//draw aura
			Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //draw aura
            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.Lime, Color.Green, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5f), 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.75f, NPC.rotation, NPC.frame.Size() / 2, scaleStretch * 1.1f, effects, 0f);
            }

			//draw npc manually for stretching
            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, scaleStretch, effects, 0f);

			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];

            return Parent.ai[0] == 3;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			Player player = Main.player[Parent.target];

			NPC.TargetClosest(true);
			NPC.spriteDirection = NPC.direction;

			//kill this npc if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<BanditBook>())
			{
                NPC.active = false;
			}

			//stretch stuff
            if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.1f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

			//reset rotation when attacking
			if (Parent.ai[0] == 3)
			{
				NPC.rotation = 0;
			}

			switch ((int)Parent.ai[0])
			{
				//idle, fly to the book
				case 1:
				{
					GoToPosition(0, -155, 0.45f);

					if (Shake)
					{
						NPC.rotation += 0.01f;
						if (NPC.rotation > 0.12f)
						{
							Shake = false;
							NPC.netUpdate = true;
						}
					}
					else
					{
						NPC.rotation -= 0.01f;
						if (NPC.rotation < -0.12f)
						{
							Shake = true;
							NPC.netUpdate = true;
						}
					}

					break;
				}

				//attacks
				case 3:
				{
					//fly to the player and shoot out erratic magic balls, repeat 3 times
					if (Parent.localAI[1] < 3)
					{
						if (Parent.localAI[0] == 5)
						{
							if (player.Center.X > NPC.Center.X)
							{
								SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-420, -180), player.Center.Y - Main.rand.Next(50, 120));
							}
							else
							{
								SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(180, 420), player.Center.Y - Main.rand.Next(50, 120));
							}

							NPC.netUpdate = true;
						}

						if (Parent.localAI[0] > 5 && Parent.localAI[0] < 40)
						{
							Vector2 GoTo = SavePlayerPosition;

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 20);
							NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

							//slow down when too close to the player
							if (NPC.Distance(player.Center) <= 250f)
							{
								NPC.velocity *= 0.95f;
							}
						}
						else
						{
							NPC.velocity *= 0.92f;
						}

						bool ShootExtra = Parent.ai[1] > 0 || Parent.ai[3] > 0;
						bool ShootFaster = Parent.ai[1] > 0 && Parent.ai[3] > 0;

						if (Parent.localAI[0] == 60 || Parent.localAI[0] == 80 || (Parent.localAI[0] == 100 && ShootExtra))
						{
							SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

							stretchRecoil = 0.5f;

							Vector2 ShootSpeed = player.Center - NPC.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= ShootFaster ? 12f : 8.5f;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;
							Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

							if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
							{
								position += muzzleOffset;
							}

							Projectile.NewProjectile(NPC.GetSource_FromAI(), position, ShootSpeed, ModContent.ProjectileType<BanditWizardBall>(), NPC.damage / 4, 0f, Main.myPlayer);
						}

						if (Parent.localAI[0] >= 120)
						{
							Parent.localAI[0] = 0;
							Parent.localAI[1]++;
						}
					}
					else
					{
						//after finishing the attack, do next attack/loop
						if (Parent.localAI[0] >= 60)
						{
							//fire out giant magic ball that splits into a spread of smaller ones
							if (Parent.ai[1] > 0 || Parent.ai[3] > 0)
							{
								//go to the player again
								if ((Parent.localAI[0] > 60 && Parent.localAI[0] < 120) || Parent.localAI[0] > 230)
								{
									//slow down when too close to the player
									if (NPC.Distance(player.Center) > 300f)
									{
										Vector2 GoTo = player.Center;

										float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 5, 10);
										NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);
									}
									else
									{
										NPC.velocity *= 0.95f;
									}
								}
								else
								{
									NPC.velocity *= 0.92f;
								}

								bool ShootExtra = Parent.ai[1] > 0 && Parent.ai[3] > 0;

								if (Parent.localAI[0] == 180 || (Parent.localAI[0] == 220 && ShootExtra))
								{
									SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

									stretchRecoil = 0.5f;

									Vector2 ShootSpeed = player.Center - NPC.Center;
									ShootSpeed.Normalize();
									ShootSpeed *= 5f;

									Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;
									Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

									if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
									{
										position += muzzleOffset;
									}

									Projectile.NewProjectile(NPC.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<BanditWizardBallSplitting>(), NPC.damage / 4, 0f, player.whoAmI);
								}

								if (Parent.localAI[0] >= 440)
								{
									Parent.localAI[0] = 0;
									Parent.localAI[1] = 0;
									Parent.ai[0]++;

									NPC.netUpdate = true;
								}
							}
							//otherwise swap to the next ghost to attack
							else
							{
								Parent.localAI[0] = 0;
								Parent.localAI[1] = 0;
								Parent.ai[0]++;

								NPC.netUpdate = true;
							}
						}
					}

					break;
				}

				//go idle when the other bandits are attacking
				case 2:
				{
					goto case 1;
				}
				case 4:
				{
					goto case 1;
				}
			}
		}

		public void GoToPosition(float X, float Y, float speed)
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			float goToX = (Parent.Center.X + X) - NPC.Center.X;
			float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;

			NPC.ai[1]++;

			if (NPC.velocity.X > speed)
			{
				NPC.velocity.X *= 0.98f;
			}
			if (NPC.velocity.Y > speed)
			{
				NPC.velocity.Y *= 0.98f;
			}

			Vector2 GoTo = new Vector2(goToX, goToY);

			//slow down when close enough to the parent npc
			if (Parent.Distance(GoTo) <= 150f)
			{
				NPC.velocity *= 0.92f;
			}

			if (NPC.velocity.X < goToX)
			{
				NPC.velocity.X = NPC.velocity.X + speed;
				if (NPC.velocity.X < 0f && goToX > 0f)
				{
					NPC.velocity.X = NPC.velocity.X + speed;
				}
			}
			else if (NPC.velocity.X > goToX)
			{
				NPC.velocity.X = NPC.velocity.X - speed;
				if (NPC.velocity.X > 0f && goToX < 0f)
				{
					NPC.velocity.X = NPC.velocity.X - speed;
				}
			}
			if (NPC.velocity.Y < goToY)
			{
				NPC.velocity.Y = NPC.velocity.Y + speed;
				if (NPC.velocity.Y < 0f && goToY > 0f)
				{
					NPC.velocity.Y = NPC.velocity.Y + speed;
					return;
				}
			}
			else if (NPC.velocity.Y > goToY)
			{
				NPC.velocity.Y = NPC.velocity.Y - speed;
				if (NPC.velocity.Y > 0f && goToY < 0f)
				{
					NPC.velocity.Y = NPC.velocity.Y - speed;
					return;
				}
			}
		}

		public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0) 
            {
				NPC Parent = Main.npc[(int)NPC.ai[0]];

				Parent.localAI[0] = 0;
				Parent.localAI[1] = 0;
				Parent.localAI[2] = 0;
				Parent.localAI[3] = 0;
				Parent.ai[2] = 1;

                for (int numDusts = 0; numDusts < 35; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
                    Main.dust[dustGore].color = Color.Green;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
	}
}