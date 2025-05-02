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

using Spooky.Core;
using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;
using Spooky.Content.Tiles.Relic;

namespace Spooky.Content.NPCs.Quest
{
	public class BanditBook : ModNPC
	{
		public bool Shake = false;

		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> GlowTexture1;
		private static Asset<Texture2D> GlowTexture2;
		private static Asset<Texture2D> GlowTexture3;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/GhostBanditsBestiary",
                Position = new Vector2(-12f, -55f),
				PortraitPositionXOverride = -6f,
                PortraitPositionYOverride = 25f
            };
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
			writer.Write(NPC.localAI[3]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
			NPC.localAI[3] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 8000;
            NPC.damage = 40;
			NPC.defense = 15;
			NPC.width = 32;
			NPC.height = 42;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CemeteryBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			//this gets every default hp value for every bandit, then scales them individually and adds them up to get an accurate result of how much hp they actually have
			//this npcs default hp of 8000 is every bandits default hp added up together
			NPC.lifeMax = (int)(2750 * 0.75f * balance * bossAdjustment) + (int)(2750 * 0.75f * balance * bossAdjustment) + (int)(2500 * 0.75f * balance * bossAdjustment);
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BanditBook"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/BanditBookGlow");
			GlowTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/BanditBookGlowRed");
			GlowTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/BanditBookGlowGreen");
			GlowTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/BanditBookGlowBlue");

			//draw aura
			for (int i = 0; i < 360; i += 30)
			{
				Color color = Color.White;

				//change color based on which ghost is attacking
				if (NPC.ai[0] == -1 || NPC.ai[0] == 0 || NPC.ai[0] == 1)
				{
					Color[] ColorList = new Color[]
					{
						Color.Red, Color.Green, Color.Cyan
					};

					float fade = Main.GameUpdateCount % 60 / 60f;
					int index = (int)(Main.GameUpdateCount / 60 % 3);

					color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(ColorList[index], ColorList[(index + 1) % 3], fade));
				}
				if (NPC.ai[0] == 2)
				{
					color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.Red, Color.OrangeRed, i / 30));
				}
				if (NPC.ai[0] == 3)
				{
					color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.Lime, Color.Green, i / 30));
				}
				if (NPC.ai[0] == 4)
				{
					color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lerp(Color.Gold, Color.Cyan, i / 30));
				}
				
				Vector2 circular = new Vector2(Main.rand.NextFloat(3.5f, 5), 0).RotatedBy(MathHelper.ToRadians(i));
				spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color * 0.75f, NPC.rotation, NPC.frame.Size() / 2, NPC.scale * 1.1f, SpriteEffects.None, 0);
			}
			
			//draw the book itself
			spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);

			//draw the glowmask based on which ghost is attacking
			if (NPC.ai[0] == 0 || NPC.ai[0] == 1)
			{
				spriteBatch.Draw(GlowTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			}
			if (NPC.ai[0] == 2)
			{
				spriteBatch.Draw(GlowTexture1.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			}
			if (NPC.ai[0] == 3)
			{
				spriteBatch.Draw(GlowTexture2.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			}
			if (NPC.ai[0] == 4)
			{
				spriteBatch.Draw(GlowTexture3.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, SpriteEffects.None, 0);
			}

			return false;
		}

		public override void FindFrame(int frameHeight)
        {
			//sleeping animation
			if (NPC.ai[0] == 0 && NPC.localAI[0] > 0)
			{
				NPC.frameCounter++;
				if (NPC.frameCounter > 6)
				{
					NPC.frame.Y = NPC.frame.Y + frameHeight;
					NPC.frameCounter = 0;
				}
				if (NPC.frame.Y >= frameHeight * 3)
				{
					NPC.frame.Y = 2 * frameHeight;
				}
			}
			else
			{
				NPC.frame.Y = 0 * frameHeight;
			}
        }

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

			//NPC.ai[1, 2, 3] are used as a check if each individual ghost is downed, corresponding to the number
			//will consist of a value of either 0 or 1, where 0 = that respective ghost is alive, and 1 = that respective ghost is dead
			//NPC.ai[1] > 0 = bruiser dead
			//NPC.ai[2] > 0 = mage dead
			//NPC.ai[3] > 0 = priest dead

			//bruiser desperation
			if (NPC.ai[2] > 0 && NPC.ai[3] > 0)
			{
				NPC.ai[0] = 2;
			}
			//mage desperation
			if (NPC.ai[1] > 0 && NPC.ai[3] > 0)
			{
				NPC.ai[0] = 3;
			}
			//priest desperation
			if (NPC.ai[1] > 0 && NPC.ai[2] > 0)
			{
				NPC.ai[0] = 4;
			}

			//death animation if all of the ghosts are dead
			if (NPC.ai[1] > 0 && NPC.ai[2] > 0 && NPC.ai[3] > 0)
			{
				//set ai to -1 so it doesnt do anything
				NPC.ai[0] = -1;

				NPC.velocity *= 0.97f;

				NPC.localAI[2]++;

                NPC.localAI[3] += 0.15f;

                if (Shake)
                {
                    NPC.rotation += NPC.localAI[3] / 20;
                    if (NPC.rotation > 0.5f)
                    {
                        Shake = false;
                    }
                }
                else
                {
                    NPC.rotation -= NPC.localAI[3] / 20;
                    if (NPC.rotation < -0.5f)
                    {
                        Shake = true;
                    }
                }

				if (NPC.localAI[2] >= 120)
                {
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageSummonSkeleton with { Pitch = -1f }, NPC.Center);

					float maxAmount = 30;
					int currentAmount = 0;

					int[] DustTypes = new int[] { DustID.GreenTorch, DustID.RedTorch, DustID.BlueTorch };

					while (currentAmount <= maxAmount)
					{
						Vector2 velocity = new Vector2(5f, 5f);
						Vector2 Bounds = new Vector2(3f, 3f);
						float intensity = 5f;

						Vector2 vector12 = Vector2.UnitX * 0f;
						vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
						vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
						int num104 = Dust.NewDust(NPC.Center, 0, 0, Main.rand.Next(DustTypes), 0f, 0f, 100, default, 3f);
						Main.dust[num104].noGravity = true;
						Main.dust[num104].position = NPC.Center + vector12;
						Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
						currentAmount++;
					}

					while (currentAmount <= maxAmount)
					{
						Vector2 velocity = new Vector2(10f, 10f);
						Vector2 Bounds = new Vector2(5f, 5f);
						float intensity = 5f;

						Vector2 vector12 = Vector2.UnitX * 0f;
						vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
						vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
						int num104 = Dust.NewDust(NPC.Center, 0, 0, Main.rand.Next(DustTypes), 0f, 0f, 100, default, 3f);
						Main.dust[num104].noGravity = true;
						Main.dust[num104].position = NPC.Center + vector12;
						Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
						currentAmount++;
					}

					while (currentAmount <= maxAmount)
					{
						Vector2 velocity = new Vector2(15f, 15f);
						Vector2 Bounds = new Vector2(7f, 7f);
						float intensity = 5f;

						Vector2 vector12 = Vector2.UnitX * 0f;
						vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
						vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
						int num104 = Dust.NewDust(NPC.Center, 0, 0, Main.rand.Next(DustTypes), 0f, 0f, 100, default, 3f);
						Main.dust[num104].noGravity = true;
						Main.dust[num104].position = NPC.Center + vector12;
						Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
						currentAmount++;
					}

                    NPC.immortal = false;
					NPC.dontTakeDamage = false;
					NPC.netUpdate = true;
					player.ApplyDamageToNPC(NPC, NPC.lifeMax * 2, 0, 0, false);
                }
			}
			else
			{
				NPC.rotation = NPC.velocity.X * 0.03f;
			}

			switch ((int)NPC.ai[0])
			{
				//spawn intro
				case 0:
				{
					if (NPC.localAI[1] == 0)
					{
						NPC.localAI[3] = NPC.position.Y;
						NPC.localAI[1]++;
					}

					NPC.localAI[2]++;
					NPC.position.Y = NPC.localAI[3] + (float)Math.Sin(NPC.localAI[2] / 30) * 30;

					//activate spawn intro if the player gets close
					if (player.Distance(NPC.Center) <= 200f && NPC.localAI[0] <= 0) 
					{
						NPC.localAI[0]++;
					}

					//spawn intro
					if (NPC.localAI[0] > 0)
					{
						NPC.localAI[0]++;

						if (NPC.localAI[0] == 120)
						{
							NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(-5, -5), ModContent.ProjectileType<GhostSpawner1>(), 0, 0f, ai2: NPC.whoAmI);
						}

						if (NPC.localAI[0] == 180)
						{
							NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(0, -5), ModContent.ProjectileType<GhostSpawner2>(), 0, 0f, ai2: NPC.whoAmI);
						}

						if (NPC.localAI[0] == 240)
						{
							NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center, new Vector2(5, -5), ModContent.ProjectileType<GhostSpawner3>(), 0, 0f, ai2: NPC.whoAmI);
						}

						if (NPC.localAI[0] >= 320)
						{
							NPC.localAI[0] = 0;
							NPC.localAI[1] = 0;
							NPC.localAI[2] = 0;
							NPC.localAI[3] = 0;
							NPC.ai[0]++;

							NPC.netUpdate = true;
						}
					}

					break;
				}

				//book chases the player while ghosts float above it
				case 1:
				{
					NPC.localAI[0]++;

					if (player.Distance(NPC.Center) > 200f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 6;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						NPC.velocity *= 0.95f;
					}

					if (NPC.localAI[0] >= 360)
					{
						NPC.localAI[0] = 0;
						NPC.ai[0]++;
					
						NPC.netUpdate = true;
					}

					break;
				}

				//bruiser attacking
				case 2:
				{
					NPC.localAI[0]++;

					//go above the player
					Vector2 AbovePlayer = new Vector2(player.Center.X, player.Center.Y - 220);

					if (AbovePlayer.Distance(NPC.Center) > 100f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(AbovePlayer) * 12;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						NPC.velocity *= 0.98f;
					}

					//if the bruiser is dead, skip to mage attacking
					if (NPC.ai[1] > 0)
					{
						NPC.ai[0]++;

						NPC.netUpdate = true;
					}

					break;
				}

				//caster attacking
				case 3:
				{
					NPC.localAI[0]++;

					//go above the player
					Vector2 AbovePlayer = new Vector2(player.Center.X, player.Center.Y - 220);

					if (AbovePlayer.Distance(NPC.Center) > 100f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(AbovePlayer) * 12;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						NPC.velocity *= 0.98f;
					}

					//if the mage is dead, skip to priest attacking
					if (NPC.ai[2] > 0)
					{
						NPC.ai[0] = 1;

						NPC.netUpdate = true;
					}

					break;
				}

				//priest desperation phase
				case 4:
				{
					//go above the player
					Vector2 AbovePlayer = new Vector2(player.Center.X, player.Center.Y - 220);

					if (AbovePlayer.Distance(NPC.Center) > 100f)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(AbovePlayer) * 12;
						NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
					}
					else
					{
						NPC.velocity *= 0.98f;
					}

					break;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<BanditBookRelicItem>()));

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BountyItem2>()));
        }
	}
}