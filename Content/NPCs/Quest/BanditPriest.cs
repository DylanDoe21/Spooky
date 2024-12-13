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
using Spooky.Content.Dusts;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class BanditPriest : ModNPC
	{
		int ChosenGhostToBuff;

		float addedStretch = 0f;
		float stretchRecoil = 0f;

		bool Shake = false;

		Vector2 SavePlayerPosition;

		private static Asset<Texture2D> NPCTexture;

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

			//floats
            writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//vector2
			SavePlayerPosition = reader.ReadVector2();

			//floats
            NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 2500;
            NPC.damage = 35;
			NPC.defense = 10;
			NPC.width = 50;
			NPC.height = 112;
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
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
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
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.Gold, Color.Cyan, i / 30));

                Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5f), 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.75f, NPC.rotation, NPC.frame.Size() / 2, scaleStretch * 1.1f, effects, 0f);
            }

			//draw npc manually for stretching
            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, scaleStretch, effects, 0f);

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			//casting animation
			if ((Parent.ai[0] != 4 && NPC.localAI[2] > 1040 && NPC.localAI[2] < 1200) || (Parent.ai[0] == 4 && ((NPC.localAI[1] < 5 && NPC.localAI[0] >= 30) || (NPC.localAI[1] >= 5 && NPC.localAI[0] > 60))))
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

			NPC.TargetClosest(true);
			NPC.spriteDirection = NPC.direction;

			Player player = Main.player[Parent.target];

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

			if (Parent.ai[0] > 0)
			{
				//if not in its desperation phase, then randomly buff one of the other ghost bandits
				if (Parent.ai[0] != 4)
				{
					GoToPosition(155, -135, 0.45f);

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

					NPC.localAI[2]++;

					if (NPC.localAI[2] == 1100)
					{
						//choose random ghost by default
						ChosenGhostToBuff = Main.rand.NextBool() ? ModContent.NPCType<BanditBruiser>() : ModContent.NPCType<BanditWizard>();

						//if the bruiser is dead, only choose the wizard
						if (Parent.ai[1] > 0)
						{
							ChosenGhostToBuff = ModContent.NPCType<BanditWizard>();
						}

						//if the wizard is dead, only choose the bruiser
						if (Parent.ai[2] > 0)
						{
							ChosenGhostToBuff = ModContent.NPCType<BanditBruiser>();
						}

						for (int i = 0; i < Main.maxNPCs; i++)
						{
							//choose to buff either the bruiser or wizard, and make sure that they share the same parent npc as this one
							if (Main.npc[i].active && Main.npc[i].type == ChosenGhostToBuff && Main.npc[i].ai[0] == NPC.ai[0])
							{
								ChosenGhostToBuff = Main.npc[i].whoAmI;
								break;
							}
						}
					}

					if (NPC.localAI[2] > 1100 && NPC.localAI[2] < 1200)
					{
						if (NPC.localAI[2] % 10 == 0)
						{
							SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

							stretchRecoil = 0.5f;

							NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, Vector2.Zero, ModContent.ProjectileType<BanditPriestBuffer>(), 0, 0f, ai0: ChosenGhostToBuff);
						}
					}

					if (NPC.localAI[2] >= 1200)
					{
						NPC.localAI[2] = 0;

						NPC.netUpdate = true;
					}
				}
				//desperation attacks
				else
				{
					NPC.rotation = 0;

					NPC.localAI[0]++;

					if (NPC.localAI[1] < 5)
					{
						if (NPC.localAI[0] == 2)
						{
							if (player.Center.X > NPC.Center.X)
							{
								SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(-350, -250), player.Center.Y - Main.rand.Next(50, 120));
							}
							else
							{
								SavePlayerPosition = new Vector2(player.Center.X + Main.rand.Next(250, 350), player.Center.Y - Main.rand.Next(50, 120));
							}

							NPC.netUpdate = true;
						}

						if (NPC.localAI[0] > 2 && NPC.localAI[0] < 30)
						{
							Vector2 GoTo = SavePlayerPosition;

							float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 15, 22);
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

						if (NPC.localAI[0] >= 60)
						{
							SoundEngine.PlaySound(SoundID.Item29 with { Volume = 0.25f, Pitch = 1.2f }, NPC.Center);

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

							NPCGlobalHelper.ShootHostileProjectile(NPC, position, ShootSpeed, ModContent.ProjectileType<BanditPriestCross>(), NPC.damage, 3.5f, ai0: player.whoAmI);

							NPC.localAI[0] = 0;
							NPC.localAI[1]++;
						}
					}
					//shoot out spreads of magic bolts
					else
					{
						Vector2 GoTo = player.Center;
						GoTo.X += player.Center.X > NPC.Center.X ? -450 : 450;

						float vel = MathHelper.Clamp(NPC.Distance(GoTo) / 12, 12, 25);
						NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.DirectionTo(GoTo) * vel, 0.08f);

						if (NPC.localAI[0] > 80)
						{
							if (NPC.localAI[0] % 60 == 0)
							{
								SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

								for (int numProjectiles = -1; numProjectiles <= 1; numProjectiles++)
								{
									NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + new Vector2(player.Center.X < NPC.Center.X ? -25 : 25, 0), 
									12f * NPC.DirectionTo(player.Center).RotatedBy(MathHelper.ToRadians(22) * numProjectiles), ModContent.ProjectileType<BanditPriestBall>(), NPC.damage, 3.5f);
								}

								stretchRecoil = 0.5f;
							}
						}

						//loop attack
						if (NPC.localAI[0] >= 300)
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;

							NPC.netUpdate = true;
						}
					}
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
			if (NPC.Distance(GoTo) <= 250f)
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
				Parent.ai[3] = 1;

                for (int numDusts = 0; numDusts < 35; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.35f);
                    Main.dust[dustGore].color = Color.Cyan;
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
	}
}