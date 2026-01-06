using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Core
{
	public class EggEventWorld : ModSystem
	{
		public static int EventTimeLeft = 0;
		public static int EventTimeLeftUI = 0;
		public static int EnemySpawnTimer = 0;
		public static bool EggEventActive = false;
		public static bool HasSpawnedBiojetter1 = false;
		public static bool HasSpawnedBiojetter2 = false;
		public static bool HasSpawnedBolster1 = false;
		public static bool HasSpawnedBolster2 = false;
		public static bool HasSpawnedBolster3 = false;

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(EventTimeLeft);
			writer.Write(EventTimeLeftUI);
			writer.Write(EnemySpawnTimer);

			writer.WriteFlags(EggEventActive, HasSpawnedBiojetter1, HasSpawnedBiojetter2, HasSpawnedBolster1, HasSpawnedBolster2, HasSpawnedBolster3);
		}

		public override void NetReceive(BinaryReader reader)
		{
			EventTimeLeft = reader.ReadInt32();
			EventTimeLeftUI = reader.ReadInt32();
			EnemySpawnTimer = reader.ReadInt32();

			reader.ReadFlags(out EggEventActive, out HasSpawnedBiojetter1, out HasSpawnedBiojetter2, out HasSpawnedBolster1, out HasSpawnedBolster2, out HasSpawnedBolster3);
		}

		public override void OnWorldLoad()
		{
			EventTimeLeft = 0;
			EventTimeLeftUI = 0;
			EggEventActive = false;
			HasSpawnedBiojetter1 = false;
			HasSpawnedBiojetter2 = false;
			HasSpawnedBolster1 = false;
			HasSpawnedBolster2 = false;
			HasSpawnedBolster3 = false;
		}

		//select a random player in the event if they are in the eye valley and arent dead/inactive
		public static int GetRandomPlayerInEvent()
		{
			int[] list = new int[] { };

			foreach (Player player in Main.ActivePlayers)
			{
				if (!player.dead && !player.ghost && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
				{
					list = list.Append(player.whoAmI).ToArray();
				}
			}

			return list[Main.rand.Next(0, list.Length)];
		}

		public bool AnyPlayersInBiome()
		{
			foreach (Player player in Main.ActivePlayers)
			{
				int playerInBiomeCount = 0;

				if (!player.dead && !player.ghost && player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
				{
					playerInBiomeCount++;
				}

				if (playerInBiomeCount >= 1)
				{
					return true;
				}
			}

			return false;
		}

		//get the total number of active egg incursion enemies
		public int EventActiveNPCCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];

				int[] EventNPCs = new int[] { ModContent.NPCType<Biojetter>(), ModContent.NPCType<CoughLungs>(), ModContent.NPCType<CruxBat>(), ModContent.NPCType<EarWorm>(),
				ModContent.NPCType<ExplodingAppendix>(), ModContent.NPCType<GooSlug>(), ModContent.NPCType<HoppingHeart>(), ModContent.NPCType<HoverBrain>(), ModContent.NPCType<TongueBiter>() };

				if (Enemy.active && EventNPCs.Contains(Enemy.type))
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

		//get the total number of active ear worms since they are spawned in manuallys
		public int EarWormCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];

				if (Enemy.active && Enemy.type == ModContent.NPCType<EarWorm>())
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

		//spawn an enemy based on the type inputted
		public static void SpawnEnemy(int BiomassType, int Type, Player player)
		{
			//if the player is null (which is done on multiplayer clients) then dont do anything at all
			if (player == null)
			{
				return;
			}

			switch (BiomassType)
			{
				case 0:
				{
					//Types:
					//0 = GooSlug
					//1 = CruxBat
					//2 = Biojetter
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int Biomass = NPC.NewNPC(null, (int)(player.Center.X + Main.rand.Next(-600, 600)), 
						(int)(Flags.EggPosition.Y + Main.rand.Next(100, 150)), ModContent.NPCType<GiantBiomassPurple>(), ai2: Type);
						Main.npc[Biomass].netUpdate = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Biomass);
						}
					}

					break;
				}

				case 1:
				{
					//Types:
					//0 = HoppingHeart
					//1 = TongueBiter
					//2 = ExplodingAppendix
					//3 = CoughLungs
					//4 = HoverBrain
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int Biomass = NPC.NewNPC(null, (int)(player.Center.X + Main.rand.Next(-600, 600)), 
						(int)(Flags.EggPosition.Y + Main.rand.Next(100, 150)), ModContent.NPCType<GiantBiomassRed>(), ai2: Type);
						Main.npc[Biomass].netUpdate = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Biomass);
						}
					}

					break;
				}

				case 2:
				{
					//spawn stomach enemy
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						int Biomass = NPC.NewNPC(null, (int)(player.Center.X + Main.rand.Next(-600, 600)), 
						(int)(Flags.EggPosition.Y + Main.rand.Next(100, 150)), ModContent.NPCType<GiantBiomassOrange>());
						Main.npc[Biomass].netUpdate = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.SyncNPC, number: Biomass);
						}
					}

					break;
				}
			}
		}

		public override void PostUpdateEverything()
		{
			if (!EggEventActive)
			{
				EventTimeLeft = 0;
				EventTimeLeftUI = 0;
			}

			if (EggEventActive)
			{
				//end the event and reset everything if you die, or if you leave the valley of eyes
				if (!AnyPlayersInBiome())
				{
					EggEventActive = false;
					EventTimeLeft = 0;
					EventTimeLeftUI = 0;
					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}

				if (EventTimeLeft < 21600)
				{
					Player player = Main.netMode == NetmodeID.MultiplayerClient ? null : Main.player[GetRandomPlayerInEvent()];

					//increment both timers
					//the timer for the UI gets decreased so that the actual time displayed on the UI bar is counting down and not up
					EventTimeLeft++;
					EventTimeLeftUI--;

					//timeLeft converts the time left to actual seconds, goes up to 360 seconds (or 6 minutes)
					//60 = 1 minute in
					//120 = 2 minutes in
					//180 = 3 minutes in
					//240 = 4 minutes in
					//300 = 5 minutes in
					//360 = 6 minutes in
					float timeLeft = EventTimeLeft / 60;

					int ChanceToSpawnEnemy = 300;
					if (timeLeft >= 60) ChanceToSpawnEnemy = 300;
					if (timeLeft >= 120) ChanceToSpawnEnemy = 250;
					if (timeLeft >= 180) ChanceToSpawnEnemy = 200;
					if (timeLeft >= 240) ChanceToSpawnEnemy = 150;

					if (EventTimeLeft == 3600 || EventTimeLeft == 7200 || EventTimeLeft == 10800 || EventTimeLeft == 14400 || EventTimeLeft == 18000)
					{
						SpawnEnemy(2, 0, player);
					}

					//spawn a biojetter a little before 3 minutes and a little after 4 minutes
					if (!HasSpawnedBiojetter1 && timeLeft >= 150)
					{
						SpawnEnemy(0, 2, player);

						HasSpawnedBiojetter1 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}
					if (!HasSpawnedBiojetter2 && timeLeft >= 280)
					{
						SpawnEnemy(0, 2, player);

						HasSpawnedBiojetter2 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}

					//spawn bolsters at 3 minutes, 4 minutes, and 5 minutes in
					if (!HasSpawnedBolster1 && timeLeft >= 180)
					{
						SpawnEnemy(1, 5, player);

						HasSpawnedBolster1 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}
					if (!HasSpawnedBolster2 && timeLeft >= 240)
					{
						SpawnEnemy(1, 5, player);

						HasSpawnedBolster2 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}
					if (!HasSpawnedBolster3 && timeLeft >= 300)
					{
						SpawnEnemy(1, 5, player);

						HasSpawnedBolster3 = true;

						if (Main.netMode == NetmodeID.Server)
						{
							NetMessage.SendData(MessageID.WorldData);
						}
					}

					//if theres no enemies for too long, then manually spawn a bunch of them
					if (EventActiveNPCCount() <= 1)
					{
						EnemySpawnTimer++;

						if (EnemySpawnTimer >= 240)
						{
							for (int numEnemies = 0; numEnemies <= 5; numEnemies++)
							{
								if (timeLeft < 60)
								{
									int BiomassType = Main.rand.Next(0, 2);
									SpawnEnemy(BiomassType, BiomassType == 0 ? 0 : Main.rand.Next(0, 2), player);
								}
								if (timeLeft >= 60 && timeLeft < 180)
								{
									int BiomassType = Main.rand.Next(0, 2);
									SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 2) : Main.rand.Next(0, 3), player);
								}
								if (timeLeft >= 180)
								{
									//chance to spawn an ear worm manually
									if (Main.rand.NextBool(7) && EarWormCount() < 4)
									{
										Vector2 center = new Vector2(player.Center.X, player.Center.Y - 100);

										center.X += Main.rand.Next(-500, 500);

										int numtries = 0;
										int x = (int)(center.X / 16);
										int y = (int)(center.Y / 16);

										while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
										{
											y++;
											center.Y = y * 16;
										}
										while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10)
										{
											numtries++;
											y--;
											center.Y = y * 16;
										}

										if (Main.netMode != NetmodeID.MultiplayerClient)
										{
											int EarWorm = NPC.NewNPC(null, (int)center.X, (int)center.Y + 20, ModContent.NPCType<EarWormBase>());

											if (Main.netMode == NetmodeID.Server)
											{
												NetMessage.SendData(MessageID.SyncNPC, number: EarWorm);
											}
										}
									}
									else
									{
										int BiomassType = Main.rand.Next(0, 2);
										SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 2) : Main.rand.Next(0, 5), player);
									}
								}
							}

							EnemySpawnTimer = -60;
						}
					}

					//randomly spawn enemies throughout the event
					if (EventActiveNPCCount() < 20 && Main.rand.NextBool(ChanceToSpawnEnemy))
					{
						if (timeLeft < 60)
						{
							int BiomassType = Main.rand.Next(0, 2);
							SpawnEnemy(BiomassType, BiomassType == 0 ? 0 : Main.rand.Next(0, 2), player);
						}
						if (timeLeft >= 60 && timeLeft < 180)
						{
							int BiomassType = Main.rand.Next(0, 2);
							SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 2) : Main.rand.Next(0, 3), player);
						}
						if (timeLeft >= 180)
						{
							//chance to spawn an ear worm manually
							if (Main.rand.NextBool(8) && EarWormCount() < 4)
							{
								Vector2 center = new Vector2(player.Center.X, player.Center.Y - 100);

								center.X += Main.rand.Next(-500, 500);

								int numtries = 0;
								int x = (int)(center.X / 16);
								int y = (int)(center.Y / 16);

								while (y < Main.maxTilesY - 10 && Main.tile[x, y] != null && !WorldGen.SolidTile2(x, y) && Main.tile[x - 1, y] != null && !WorldGen.SolidTile2(x - 1, y) && Main.tile[x + 1, y] != null && !WorldGen.SolidTile2(x + 1, y))
								{
									y++;
									center.Y = y * 16;
								}
								while ((WorldGen.SolidOrSlopedTile(x, y) || WorldGen.SolidTile2(x, y)) && numtries < 10)
								{
									numtries++;
									y--;
									center.Y = y * 16;
								}

								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									int EarWorm = NPC.NewNPC(null, (int)center.X, (int)center.Y + 20, ModContent.NPCType<EarWormBase>());

									if (Main.netMode == NetmodeID.Server)
									{
										NetMessage.SendData(MessageID.SyncNPC, number: EarWorm);
									}
								}
							}
							else
							{
								int BiomassType = Main.rand.Next(0, 2);
								SpawnEnemy(BiomassType, BiomassType == 0 ? Main.rand.Next(0, 2) : Main.rand.Next(0, 5), player);
							}
						}
					}
				}
				else
				{
					EventTimeLeft = 21600;
					EventTimeLeftUI = 0;
				}
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (EggEventActive && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				int EventIndex = layers.FindIndex(layer => layer is not null && layer.Name.Equals("Vanilla: Inventory"));
				LegacyGameInterfaceLayer NewLayer = new LegacyGameInterfaceLayer("Spooky: Egg Event UI",
				delegate
				{
					DrawEventUI(Main.spriteBatch);
					return true;
				},
				InterfaceScaleType.UI);

				layers.Insert(EventIndex, NewLayer);
			}
		}

		public void DrawEventUI(SpriteBatch spriteBatch)
		{
			if (EggEventActive && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>()))
			{
				const float Scale = 0.875f;
				const float Alpha = 0.65f;
				const int InternalOffset = 6;
				const int OffsetX = 20;
				const int OffsetY = 20;

				Texture2D EventIcon = ModContent.Request<Texture2D>("Spooky/Content/Items/BossSummon/StrangeCyst", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Color NameBoxColor = new Color(113, 35, 206);
				Color ProgressBoxColor = new Color(113, 35, 206);
				Color ProgressBarColor1 = new Color(54, 35, 35);
				Color ProgressBarColor2 = new Color(199, 7, 49);

				int width = (int)(200f * Scale);
				int height = (int)(46f * Scale);

				Rectangle ProgressBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
				Utils.DrawInvBG(spriteBatch, ProgressBackground, ProgressBoxColor * Alpha);

				float divide = 3.6f;

				float timeLeft = EventTimeLeft / 60;
				float timeLeftUI = EventTimeLeftUI / 60;

				TimeSpan time = TimeSpan.FromSeconds(timeLeftUI);
				string actualTime = string.Format("{0:D1}:{1:D2}", time.Minutes, time.Seconds);

				string ProgressText = Language.GetTextValue("Mods.Spooky.UI.EggEvent.EggEventBarProgress") + " " + actualTime;

				Utils.DrawBorderString(spriteBatch, ProgressText, new Vector2(ProgressBackground.Center.X, ProgressBackground.Y), Color.White, Scale, 0.5f, -0.1f);
				Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(ProgressBackground.Center.X, ProgressBackground.Y + ProgressBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

				var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp((timeLeft / divide), 0f, 100f)), TextureAssets.ColorBar.Height());
				var offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * Scale)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * Scale)) * 0.5f);
				spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, null, ProgressBarColor1 * Alpha, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);
				spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, ProgressBarColor2, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);

				Vector2 descSize = new Vector2(175, 40) * Scale;
				Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 19f), new Vector2(width, height));
				Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), descSize * 0.9f);
				Utils.DrawInvBG(spriteBatch, descBackground, NameBoxColor * Alpha);

				int descOffset = (descBackground.Height - (int)(32f * Scale)) / 2;
				var icon = new Rectangle(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * Scale), (int)(32 * Scale));
				spriteBatch.Draw(EventIcon, icon, Color.White);
				Utils.DrawBorderString(spriteBatch, Language.GetTextValue("Mods.Spooky.UI.EggEvent.EggEventBarDisplayName"), new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
			}
		}
	}
}