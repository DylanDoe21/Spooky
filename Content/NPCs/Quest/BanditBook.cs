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

using Spooky.Content.Items.Quest;
using Spooky.Content.NPCs.Quest.Projectiles;

namespace Spooky.Content.NPCs.Quest
{
	public class BanditBook : ModNPC
	{
		private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> GlowTexture1;
		private static Asset<Texture2D> GlowTexture2;
		private static Asset<Texture2D> GlowTexture3;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 3;
			NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            NPC.localAI[0] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.damage = 0;
			NPC.defense = 0;
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

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BanditBook"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CemeteryBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		//TODO: make the book draw with a different color outline based on which ghost is attacking currently
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
				if (NPC.ai[0] == 0 || NPC.ai[0] == 1)
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

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.X * 0.03f;

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

			switch ((int)NPC.ai[0])
			{
				//passive floating ai
				//TODO: add little particles that spawn the ghosts instead of them just appearing
				case 0:
				{
					if (NPC.localAI[1] == 0)
					{
						NPC.localAI[3] = NPC.position.Y;
						NPC.localAI[1]++;
					}

					NPC.localAI[2]++;
					NPC.position.Y = NPC.localAI[3] + (float)Math.Sin(NPC.localAI[2] / 30) * 30;

					if (player.Distance(NPC.Center) <= 200f)
					{
						int Bandit1 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BanditBruiser>(), ai0: NPC.whoAmI);
						int Bandit2 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BanditPriest>(), ai0: NPC.whoAmI);
						int Bandit3 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BanditWizard>(), ai0: NPC.whoAmI);

						if (Main.netMode != NetmodeID.MultiplayerClient)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Bandit1);
							NetMessage.SendData(MessageID.SyncNPC, number: Bandit2);
							NetMessage.SendData(MessageID.SyncNPC, number: Bandit3);
						}

						NPC.localAI[1] = 0;
						NPC.localAI[2] = 0;
						NPC.localAI[3] = 0;
						NPC.ai[0]++;

						NPC.netUpdate = true;
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

				//bruiser attacking: he flies to the player and preforms small punches, firing small fist projectiles that chase you a tiny bit
				//if any other ghosts are dead, preform a giant punch afterward where he charges at the player while punching, which will massively knock back the player
				//if the bruiser is the only one remaining, then use both of these attacks in a pattern rapidly
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
					}

					break;
				}

				//caster attacking: go to the player, then squish and fire out homing magic green fireballs
				//if any other ghosts are dead, then fire out a large homing green fireball that splits into a spread of smaller ones towards the player
				//if the mage is the only one remaining, then use both of these attacks in a pattern rapidly
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
						NPC.ai[0]++;
					}

					break;
				}

				//priest attacking: go to the player, shoot a magic ball that has a bunch of large debuff icons that swap rapidly and then stops like a gambling machine
				//once the debuff symbol is chosen, the projectile will fly to the player at an unavoidable speed and inflict them with that debuff for a while
				//if any other ghosts are dead, then he will grant the other remaining ghost a buff to their defense
				//if the priest is the only one remaining, then use a pattern of shooting somewhat weak homing magic bolts, and using the debuff symbol attack
				case 4:
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

					//if the priest is dead, loop back to first ghost
					if (NPC.ai[3] > 0)
					{
						NPC.ai[0] = 1;
					}

					break;
				}
			}
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BountyItem2>()));
        }
	}
}