using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Dusts;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class BanditWizard : ModNPC
	{
		float addedStretch = 0f;
		float stretchRecoil = 0f;
		float ShieldAlpha = 0f;

		bool Shake = false;

		Vector2 SavePlayerPosition;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> ShieldTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
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

			//bools
            writer.Write(Shake);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
            SavePlayerPosition = reader.ReadVector2();

			//bools
            Shake = reader.ReadBoolean();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 2750;
            NPC.damage = 35;
			NPC.defense = 15;
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

			//draw forcefield if being buffed
            if (ShieldAlpha > 0)
            {
                ShieldTexture ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/EggShieldNoise");

				float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                var center = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);
                DrawData drawData = new DrawData(ShieldTexture.Value, center + new Vector2(0, 25), new Rectangle(0, 0, 500, 400), Color.Lerp(Color.Cyan, Color.Blue, fade) * ShieldAlpha, 0, new Vector2(250f, 250f), NPC.scale * (0.5f + fade * 0.05f), SpriteEffects.None, 0);
                GameShaders.Misc["ForceField"].UseColor(new Vector3(1f + fade * 0.5f));
                GameShaders.Misc["ForceField"].Apply(drawData);
                drawData.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			//casting animation
			if (Parent.ai[0] == 3 && ((Parent.localAI[0] > 60 && Parent.localAI[0] < 140 && Parent.localAI[1] < 3) || (Parent.localAI[0] >= 140 && Parent.localAI[0] < 220 && Parent.localAI[1] >= 3)))
			{
				if (NPC.frame.Y < frameHeight * 5)
				{
					NPC.frame.Y = 5 * frameHeight;
				}

				NPC.frameCounter++;
				if (NPC.frameCounter > 7)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 10)
				{
					NPC.frame.Y = 7 * frameHeight;
				}
			}
			//idle animation
			else
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 7)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 5)
				{
					NPC.frame.Y = 0 * frameHeight;
				}
			}
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
			return false;
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

			//draw forcefield if being buffed
            if (NPC.HasBuff(ModContent.BuffType<GhostBanditDefense>()))
            {
				if (ShieldAlpha < 1)
				{
					ShieldAlpha += 0.05f;
				}
			}
			else
			{
				if (ShieldAlpha > 0)
				{
					ShieldAlpha -= 0.05f;
				}
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

			//buff defense
			if (NPC.HasBuff(ModContent.BuffType<GhostBanditDefense>()))
            {
				NPC.defense = 50;
			}
			else
			{
				NPC.defense = 0;
			}

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
						}
					}
					else
					{
						NPC.rotation -= 0.01f;
						if (NPC.rotation < -0.12f)
						{
							Shake = true;
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
						//save position based on the side of the player its on
						if (Parent.localAI[0] == 5)
						{
							if (player.Center.X > NPC.Center.X)
							{
								SavePlayerPosition = new Vector2(player.Center.X - 200, player.Center.Y - Main.rand.Next(50, 120));
							}
							else
							{
								SavePlayerPosition = new Vector2(player.Center.X + 200, player.Center.Y - Main.rand.Next(50, 120));
							}

							NPC.netUpdate = true;
						}

						//go to the player, slow down if too close
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

						//shoot out ghost bolts
						bool ShootExtra = Parent.ai[1] > 0 || Parent.ai[3] > 0;
						bool ShootFaster = Parent.ai[1] > 0 && Parent.ai[3] > 0;

						if (Parent.localAI[0] == 80 || Parent.localAI[0] == 100 || (Parent.localAI[0] == 120 && ShootExtra))
						{
							SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

							stretchRecoil = 0.5f;

							Vector2 ShootSpeed = player.Center - NPC.Center;
							ShootSpeed.Normalize();
							ShootSpeed *= ShootFaster ? 11f : 7f;

							Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 45f;
							Vector2 position = new Vector2(NPC.Center.X, NPC.Center.Y);

							if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
							{
								position += muzzleOffset;
							}

							NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<BanditWizardBall>(), NPC.damage, 3.5f);
						}

						if (Parent.localAI[0] >= 140)
						{
							Parent.localAI[0] = 0;
							Parent.localAI[1]++;

							NPC.netUpdate = true;
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

									NPCGlobalHelper.ShootHostileProjectile(NPC, position, Vector2.Zero, ModContent.ProjectileType<BanditWizardBallSplitting>(), NPC.damage, 3.5f, ai1: player.whoAmI);
								}

								//swap to next attack
								if (Parent.localAI[0] >= 440)
								{
									Parent.localAI[0] = 0;
									Parent.localAI[1] = 0;
									Parent.ai[0] = 1;

									NPC.netUpdate = true;
								}
							}
							//otherwise swap to the next ghost to attack
							else
							{
								Parent.localAI[0] = 0;
								Parent.localAI[1] = 0;
								Parent.ai[0] = 1;

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