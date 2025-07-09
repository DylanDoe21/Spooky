using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Mounts;
using Spooky.Content.Dusts;
using Spooky.Content.Items.BossSummon;
using Terraria.DataStructures;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class Turkey : ModNPC
	{
		int numSeedsEaten = 0;

        bool PeckingGround = false;
        bool ScaredRunning = false;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 18;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void Load()
		{
			On_Main.DrawMiscMapIcons += DrawTurkeyIcons;
		}

		public override void Unload()
		{
			On_Main.DrawMiscMapIcons -= DrawTurkeyIcons;
		}

		private static void DrawTurkeyIcons(On_Main.orig_DrawMiscMapIcons orig, Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
		{
			orig(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
			DrawTamedTurkeyMapIcons(self, spriteBatch, mapTopLeft, mapX2Y2AndOff, mapRect, mapScale, drawScale, ref mouseTextString);
		}

		private static void DrawTamedTurkeyMapIcons(Main self, SpriteBatch spriteBatch, Vector2 mapTopLeft, Vector2 mapX2Y2AndOff, Rectangle? mapRect, float mapScale, float drawScale, ref string mouseTextString)
		{
			if (Main.gameMenu)
			{
				return;
			}

			foreach (NPC npc in Main.ActiveNPCs)
			{
				if (npc.type == ModContent.NPCType<Turkey>() && npc.GetGlobalNPC<NPCGlobal>().TurkeyTamed)
				{
					float alphaMult = 1f;
					Vector2 vec = npc.Center / 16f - mapTopLeft;
					vec *= mapScale;
					vec += mapX2Y2AndOff;
					vec = vec.Floor();
					bool draw = true;
					if (mapRect.HasValue)
					{
						Rectangle value2 = mapRect.Value;
						if (!value2.Contains(vec.ToPoint()))
						{
							draw = false;
						}
					}
					if (draw)
					{
						Texture2D value = ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/TurkeyMapIcon").Value;
						Rectangle rectangle = value.Frame();

						spriteBatch.Draw(value, vec, rectangle, Color.White * alphaMult, 0f, rectangle.Size() / 2f, drawScale, 0, 0f);
						Rectangle rectangle2 = Utils.CenteredRectangle(vec, rectangle.Size() * drawScale);
						if (rectangle2.Contains(Main.MouseScreen.ToPoint()))
						{
							mouseTextString = Language.GetTextValue("Mods.Spooky.NPCs.Turkey.DisplayName");
						}
					}
				}
			}
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//ints
			writer.Write(numSeedsEaten);

			//bools
			writer.Write(PeckingGround);
			writer.Write(ScaredRunning);

			//floats
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//ints
			numSeedsEaten = reader.ReadInt32();

			//bools
			PeckingGround = reader.ReadBoolean();
			ScaredRunning = reader.ReadBoolean();

			//floats
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 50;
            NPC.damage = 0;
			NPC.defense = 10;
			NPC.width = 18;
			NPC.height = 40;
            NPC.npcSlots = 0.5f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
			NPC.dontTakeDamageFromHostiles = false;
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

            if (NPC.localAI[1] > 0)
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
                    if (!ScaredRunning)
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
			ScaredRunning = true;
		}

		public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
		{
			ScaredRunning = true;
		}

		public override bool CheckActive()
		{
			return !NPC.GetGlobalNPC<NPCGlobal>().TurkeyTamed; //has to be the opposite, return false = npc will not despawn
		}

		public override bool NeedSaving()
		{
			return NPC.GetGlobalNPC<NPCGlobal>().TurkeyTamed;
		}

		public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
		{
			npcHitbox = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width * 2, NPC.height);

			return false;
		}

		public override void AI()
        {
			Rectangle TurkeyRealHitbox = new Rectangle((int)(NPC.Center.X - 34), (int)NPC.position.Y, 68, NPC.height);

			foreach (Player player in Main.ActivePlayers)
			{
				if (TurkeyRealHitbox.Intersects(new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 1, 1)) &&
				NPC.Distance(player.Center) <= 65f && !Main.mapFullscreen && Main.myPlayer == player.whoAmI)
				{
					if (Main.mouseRight && Main.mouseRightRelease && !ScaredRunning)
					{
						if (!NPC.GetGlobalNPC<NPCGlobal>().TurkeyTamed)
						{
							if (ItemGlobal.ActiveItem(player).type == ModContent.ItemType<RottenSeed>() && player.ConsumeItem(ModContent.ItemType<RottenSeed>()))
							{
								SoundEngine.PlaySound(SoundID.Item2, NPC.Center);

								for (int i = 0; i < 20; i++)
								{
									Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FoodPiece, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, default, new Color(211, 109, 58), 0.75f);
								}

								if (numSeedsEaten < 9)
								{
									numSeedsEaten++;
								}
								else
								{
									NPC.GetGlobalNPC<NPCGlobal>().TurkeyTamed = true;
								}
							}
						}
						else
						{
							if (!player.HasBuff(ModContent.BuffType<TurkeyMountBuff>()))
							{
								player.Center = NPC.Center - new Vector2(0, 15);
								player.AddBuff(ModContent.BuffType<TurkeyMountBuff>(), 2);
								NPC.active = false;
							}
						}
					}
				}
			}

			//follow players around if they holding a rotten seed
			foreach (Player player in Main.ActivePlayers)
			{
				bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
				if (lineOfSight && player.Distance(NPC.Center) <= 250 && ItemGlobal.ActiveItem(player).type == ModContent.ItemType<RottenSeed>())
				{
					NPC.TargetClosest(true);
				}
			}

			//tamed turkey behavior
			if (NPC.GetGlobalNPC<NPCGlobal>().TurkeyTamed)
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

				//follow the closest player around if they are far enough away, but if they are extremely far away dont bother
				foreach (Player player in Main.ActivePlayers)
				{
					bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
					if (lineOfSight && player.Distance(NPC.Center) >= 200 && player.Distance(NPC.Center) <= 650)
					{
						NPC.TargetClosest(true);
					}
				}
			}

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

			//when hit, the turkey should run super fast but multiplying its X-velocity
			if (ScaredRunning)
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

				NPC.localAI[2]++;
				if (NPC.localAI[2] > 300)
				{
					ScaredRunning = false;
					NPC.localAI[2] = 0;
				}
			}
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