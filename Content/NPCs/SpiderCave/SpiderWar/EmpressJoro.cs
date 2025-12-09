using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.NPCs.SpiderCave.Projectiles;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class EmpressJoro : ModNPC
	{
		int SaveDirection;

		float HeldObjectScale = 0f;

		bool IsCarryingSomething = false;

		Vector2 SavePlayerPosition;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> FrontLegsTexture;
		private static Asset<Texture2D> BackLegsTexture;
		private static Asset<Texture2D> FrontLegsCarryTexture;
		private static Asset<Texture2D> BackLegsCarryTexture;
		private static Asset<Texture2D> BabyCarryTexture;
		private static Asset<Texture2D> WebEggCarryTexture;

		public static readonly SoundStyle BombDropSound = new("Spooky/Content/Sounds/JoroBombFall", SoundType.Sound) { Volume = 0.5f, Pitch = 0.5f };

		public override void SetStaticDefaults()
        {
			Main.npcFrameCount[NPC.type] = 9;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Position = new Vector2(85f, 0f),
				PortraitPositionXOverride = 35f,
              	PortraitPositionYOverride = 0f
            };
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			//bools
			writer.Write(IsCarryingSomething);

			//floats
			writer.Write(HeldObjectScale);
			writer.Write(NPC.localAI[0]);
			writer.Write(NPC.localAI[1]);
			writer.Write(NPC.localAI[2]);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//bools
			IsCarryingSomething = reader.ReadBoolean();

			//floats
			HeldObjectScale = reader.ReadSingle();
			NPC.localAI[0] = reader.ReadSingle();
			NPC.localAI[1] = reader.ReadSingle();
			NPC.localAI[2] = reader.ReadSingle();
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 15000;
            NPC.damage = 70;
			NPC.defense = 30;
			NPC.width = 230;
			NPC.height = 110;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.HitSound = SoundID.NPCHit29 with { Pitch = -0.5f };
			NPC.DeathSound = SoundID.NPCDeath36 with { Pitch = -1.5f };
			NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
		{
			NPC.lifeMax = (int)(NPC.lifeMax * 0.75f * balance * bossAdjustment);
		}

		public override void OnSpawn(IEntitySource source)
		{
			int NewNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y - 1000, ModContent.NPCType<SpotlightFirefly>(), ai3: NPC.whoAmI);
			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.SyncNPC, number: NewNPC);
			}
		}

		public override bool CheckActive()
		{
			return !SpiderWarWorld.SpiderWarActive;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);

			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.EmpressJoro"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 3)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 9)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			NPCTexture ??= ModContent.Request<Texture2D>(Texture);
			FrontLegsTexture ??= ModContent.Request<Texture2D>(Texture + "LegsFront");
			BackLegsTexture ??= ModContent.Request<Texture2D>(Texture + "LegsBack");
			FrontLegsCarryTexture ??= ModContent.Request<Texture2D>(Texture + "LegsFrontCarry");
			BackLegsCarryTexture ??= ModContent.Request<Texture2D>(Texture + "LegsBackCarry");
			BabyCarryTexture ??= ModContent.Request<Texture2D>(Texture + "BabyCarry");
			WebEggCarryTexture ??= ModContent.Request<Texture2D>(Texture + "WebEggCarry");

			var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			//back legs texture
			Main.EntitySpriteDraw(IsCarryingSomething ? BackLegsCarryTexture.Value : BackLegsTexture.Value, NPC.Center - screenPos, NPC.frame, 
			NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			//textures for objects being carried
			if (NPC.ai[0] == 1 && NPC.localAI[0] < 180)
			{
				Main.EntitySpriteDraw(WebEggCarryTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, HeldObjectScale, effects, 0);
			}
			//textures for objects being carried
			if (NPC.ai[0] == 2 && NPC.localAI[0] < 180)
			{
				Main.EntitySpriteDraw(BabyCarryTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, HeldObjectScale, effects, 0);
			}

			//actual npc body texture
            Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			//front legs texture
			Main.EntitySpriteDraw(IsCarryingSomething ? FrontLegsCarryTexture.Value : FrontLegsTexture.Value, NPC.Center - screenPos, NPC.frame, 
			NPC.GetNPCColorTintedByBuffs(NPC.GetAlpha(drawColor)), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			NPC.spriteDirection = NPC.direction;

			bool AnotherMinibossPresent = SpiderWarWorld.EventActiveNPCCount() > 1;

			switch ((int)NPC.ai[0])
			{
				//fly around player
				case 0:
				{
					NPC.localAI[0]++;

					//move towards the player if far away
					if (NPC.localAI[1] == 0)
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 6;
           		 		NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);

						if (NPC.Distance(player.Center) <= 265f)
						{
							NPC.localAI[1]++;
						}
					}
					//if close enough, move away from the player
					else
					{
						Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * -4;
           		 		NPC.velocity.X = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20).X;

						Vector2 desiredVelocity2 = NPC.DirectionTo(player.Center) * 4;
           		 		NPC.velocity.Y = Vector2.Lerp(NPC.velocity, desiredVelocity2, 1f / 20).Y;

						if (NPC.Distance(player.Center) >= 500f)
						{
							NPC.localAI[1]--;
						}
					}

					if (NPC.localAI[0] >= 360)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;

						//spawn either fly bomb or joro baby if neither exist
						if (JoroFlyCount() <= 2 && !NPC.AnyNPCs(ModContent.NPCType<JoroBaby>()))
						{
							NPC.ai[0] = Main.rand.Next(1, 3);
						}
						//if joro baby exists but not flies, then chance to spawn flies
						else if (JoroFlyCount() <= 2 && NPC.AnyNPCs(ModContent.NPCType<JoroBaby>()))
						{
							NPC.ai[0] = Main.rand.NextBool() ? 3 : 1;
						}
						//if flies exist but not the joro baby, then chance to joro baby
						else if (JoroFlyCount() > 2 && !NPC.AnyNPCs(ModContent.NPCType<JoroBaby>()))
						{
							NPC.ai[0] = Main.rand.NextBool() ? 3 : 2;
						}
						//if both enemies exist, then always use charge attack
						else 
						{
							NPC.ai[0] = 3;
						}

						NPC.netUpdate = true;
					}
 
					break;
				}

				//drop giant web nuke filled with little joro flies
				case 1:
				{
					NPC.localAI[0]++;

					//move towards the player if far away
					if (NPC.localAI[0] <= 180)
					{
						IsCarryingSomething = true;

						if (HeldObjectScale < 1)
						{
							HeldObjectScale += 0.05f;
						}

						Vector2 GoTo = player.Center - new Vector2(0, 450);

						if (NPC.Distance(GoTo) > 70f)
						{
							Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 12;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
						else
						{
							NPC.velocity *= 0.95f;
						}
					}

					if (NPC.localAI[0] == 150)
					{
						SoundEngine.PlaySound(SoundID.NPCDeath48 with { Pitch = -1f, Volume = 0.5f }, NPC.Center);
					}

					if (NPC.localAI[0] == 180)
					{
						SoundEngine.PlaySound(BombDropSound, NPC.Center);

						NPC.velocity = Vector2.Zero;

						IsCarryingSomething = false;

						HeldObjectScale = 0f;

						NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + new Vector2(50 * NPC.direction, 40), Vector2.Zero, 
						ModContent.ProjectileType<EmpressJoroWebEgg>(), NPC.damage, 1f, ai1: NPC.spriteDirection == 1 ? 0 : 1);
					}

					if (NPC.localAI[0] >= 300)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}

				//drop joro nuke spider
				case 2:
				{
					NPC.localAI[0]++;

					//move towards the player if far away
					if (NPC.localAI[0] <= 180)
					{
						IsCarryingSomething = true;

						if (HeldObjectScale < 1)
						{
							HeldObjectScale += 0.05f;
						}

						Vector2 GoTo = player.Center - new Vector2(0, 450);

						if (NPC.Distance(GoTo) > 70f)
						{
							Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 12;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
						else
						{
							NPC.velocity *= 0.95f;
						}
					}

					if (NPC.localAI[0] == 150)
					{
						SoundEngine.PlaySound(SoundID.NPCDeath48 with { Pitch = -1f, Volume = 0.5f }, NPC.Center);
					}

					if (NPC.localAI[0] == 180)
					{
						SoundEngine.PlaySound(BombDropSound, NPC.Center);

						NPC.velocity = Vector2.Zero;

						IsCarryingSomething = false;

						HeldObjectScale = 0f;

						NPCGlobalHelper.ShootHostileProjectile(NPC, NPC.Center + new Vector2(50 * NPC.direction, 40), Vector2.Zero, 
						ModContent.ProjectileType<JoroBabyDrop>(), NPC.damage, 1f, ai1: NPC.spriteDirection == 1 ? 0 : 1);
					}

					if (NPC.localAI[0] >= 300)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}

				//charge at player really quickly
				case 3:
				{
					NPC.localAI[0]++;

					if (NPC.localAI[0] < 120)
					{
						Vector2 GoTo = new Vector2(player.Center.X - (500 * NPC.direction), player.Center.Y);

						if (NPC.Distance(GoTo) > 70f)
						{
							Vector2 desiredVelocity = NPC.DirectionTo(GoTo) * 12;
							NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
						}
						else
						{
							NPC.velocity *= 0.95f;
						}
					}

					if (NPC.localAI[0] == 100)
					{
						SoundEngine.PlaySound(SoundID.NPCDeath48 with { Pitch = -1.5f, Volume = 0.5f }, NPC.Center);

						SavePlayerPosition = player.Center;
						SaveDirection = NPC.direction;

						ZipToLocation(NPC.Center.X - (20 * NPC.direction), NPC.Center.Y);
					}

					if (NPC.localAI[0] > 120)
					{
						NPC.spriteDirection = SaveDirection;
					}

					//charge at the player
                    if (NPC.localAI[0] == 115)
                    {
						SoundEngine.PlaySound(SoundID.DD2_BetsyWindAttack, NPC.Center);
					}

					if (NPC.localAI[0] == 120)
                    {
                        Vector2 ZipTo = new Vector2(SavePlayerPosition.X + (350 * NPC.direction), SavePlayerPosition.Y);

						ZipToLocation(ZipTo.X, ZipTo.Y);
                    }

                    //slow down
                    if (NPC.localAI[0] >= 122)
                    {
                        NPC.velocity *= 0.8f;
                    }

					if (NPC.localAI[0] >= 200)
					{
						NPC.localAI[0] = 0;
						NPC.localAI[1] = 0;
						NPC.ai[0] = 0;
						NPC.netUpdate = true;
					}

					break;
				}
			}
		}

		public void ZipToLocation(float TargetPositionX, float TargetPositionY)
        {
            Vector2 GoTo = new Vector2(TargetPositionX, TargetPositionY);

            if (NPC.Distance(GoTo) >= 200f)
            { 
                GoTo -= NPC.DirectionTo(GoTo) * 100f;
            }

            Vector2 GoToVelocity = GoTo - NPC.Center;

            float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

            float velocityLength = GoToVelocity.Length();

            if (velocityLength > 18f)
            { 
                velocityLength = 18f;
            }

            NPC.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            NPC.netUpdate = true;
        }

		//get the total number of joro flies
		public static int JoroFlyCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];
				if (Enemy.active && Enemy.type == ModContent.NPCType<JoroFly>())
				{
					NpcCount++;
				}
				else
				{
					continue;
				}
			}

			return NpcCount;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            //npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpiderWarItemDropCondition(), ModContent.ItemType<SpiderWarRemote>()));
			npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.SpiderWarItemDropCondition(), ModContent.ItemType<EmpressJoroTrophyItem>()));
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0)
            {
				if (SpiderWarWorld.SpiderWarActive)
				{
					SpiderWarWorld.SpiderWarPoints++;
				}

				foreach (var npc in Main.ActiveNPCs)
				{
					if (npc.type == ModContent.NPCType<SpotlightFirefly>() && npc.ai[3] == NPC.whoAmI)
					{
						npc.ai[0]++;
					}
				}

				for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EmpressJoroGore" + numGores).Type);
                    }
                }

				for (int repeats = 1; repeats <= 2; repeats++)
                {
					for (int numGores = 1; numGores <= 2; numGores++)
					{
						if (Main.netMode != NetmodeID.Server) 
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EmpressJoroLegGore" + numGores).Type);
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/EmpressJoroWingGore" + numGores).Type);
						}
					}
				}
			}
		}
	}
}