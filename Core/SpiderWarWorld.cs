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

using Spooky.Content.Achievements;
using Spooky.Content.Biomes;
using Spooky.Content.NPCs.SpiderCave.SpiderWar;

namespace Spooky.Core
{
	public class SpiderWarWorld : ModSystem
	{
		public static int SpiderWarPoints = 0;
		public static int SpiderWarWave = 0;
		public static int SpiderWarMaxPoints = 1;
		
		public static float SpiderWarDisplayPoints = 0f;

		public static bool SpiderWarActive = false;

		public override void NetSend(BinaryWriter writer)
		{
			writer.Write(SpiderWarPoints);
			writer.Write(SpiderWarWave);
			writer.Write(SpiderWarMaxPoints);

			writer.WriteFlags(SpiderWarActive);
		}

		public override void NetReceive(BinaryReader reader)
		{
			SpiderWarPoints = reader.ReadInt32();
			SpiderWarWave = reader.ReadInt32();
			SpiderWarMaxPoints = reader.ReadInt32();

			reader.ReadFlags(out SpiderWarActive);
		}

		public override void OnWorldLoad()
		{
			SpiderWarPoints = 0;
			SpiderWarWave = 0;
			SpiderWarMaxPoints = 1;
			SpiderWarDisplayPoints = 0f;
			SpiderWarActive = false;
		}

		public bool AnyPlayersInBiome()
		{
			foreach (Player player in Main.ActivePlayers)
			{
				int playerInBiomeCount = 0;

				if (!player.dead && !player.ghost && player.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
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
		public static int EventActiveNPCCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];

				int[] EventNPCs = new int[] { ModContent.NPCType<CamelColonel>(), ModContent.NPCType<CorklidQueen>(), 
				ModContent.NPCType<EmperorMortar>(), ModContent.NPCType<EmpressJoro>(), ModContent.NPCType<OgreKing>() };

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

		//get the total number of active egg incursion enemies
		public static int EventActiveSpotlightCount()
		{
			int NpcCount = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC Enemy = Main.npc[i];
				if (Enemy.active && Enemy.type == ModContent.NPCType<SpotlightSpiderFloor>())
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
		public static void SpawnEnemy(Player player, int Type)
		{
			//if the player is null (which is done on multiplayer clients) then dont do anything at all
			if (player == null)
			{
				return;
			}

            if (Main.netMode != NetmodeID.MultiplayerClient)
		    {
			    NPC.SpawnOnPlayer(Player.FindClosest(player.Center - new Vector2(-50, -50), 100, 100), Type);
            }
		}

		public override void PostUpdateEverything()
		{
			if (!SpiderWarActive)
			{
				SpiderWarPoints = 0;
				SpiderWarWave = 0;
				SpiderWarMaxPoints = 1;
				SpiderWarDisplayPoints = 0f;
			}

			if (SpiderWarActive)
			{
				if (SpiderWarWave >= 10 && !Flags.downedSpiderWar)
				{
					Flags.downedSpiderWar = true;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}

				//end the event and reset everything if all players die, or if all players leave the spider grotto
				if (!AnyPlayersInBiome())
				{
					SpiderWarPoints = 0;
					SpiderWarDisplayPoints = 0f;
					SpiderWarWave = 0;
					SpiderWarActive = false;

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}
				else if (SpiderWarWave >= 10 && SpiderWarPoints >= 20)
				{
					SpiderWarPoints = 0;
					SpiderWarDisplayPoints = 0f;
					SpiderWarWave = 0;
					SpiderWarActive = false;

					ModContent.GetInstance<EventAchievementSpiderWarEnd>().SpiderWarEndCondition.Complete();

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.SendData(MessageID.WorldData);
					}
				}
				else
				{
					if (SpiderWarPoints >= SpiderWarMaxPoints)
					{
						SpiderWarPoints = SpiderWarMaxPoints;
					}

					if (SpiderWarDisplayPoints < SpiderWarPoints)
					{
						SpiderWarDisplayPoints += 0.025f;
					}
					if (SpiderWarDisplayPoints >= SpiderWarMaxPoints)
					{
						SpiderWarWave++;
						SpiderWarPoints = 0;
						SpiderWarDisplayPoints = 0f;

						Color TextColor = new Color(171, 64, 255);

						switch (SpiderWarWave)
						{
							case 0:
							{
								SpiderWarMaxPoints = 1;
								break;
							}
							case 1:
							{
								SpiderWarMaxPoints = 1;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName"), TextColor);
								break;
							}
							case 2:
							{
								SpiderWarMaxPoints = 1;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName"), TextColor);
								break;
							}
							case 3:
							{
								SpiderWarMaxPoints = 1;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.CorklidQueen.DisplayName"), TextColor);
								break;
							}
							case 4:
							{
								SpiderWarMaxPoints = 2;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName"), TextColor);
								break;
							}
							case 5:
							{
								SpiderWarMaxPoints = 2;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName"), TextColor);
								break;
							}
							case 6:
							{
								SpiderWarMaxPoints = 2;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);
								break;
							}
							case 7:
							{
								SpiderWarMaxPoints = 3;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);
								break;
							}
							case 8:
							{
								SpiderWarMaxPoints = 5;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName"), TextColor);
								break;
							}
							case 9:
							{
								SpiderWarMaxPoints = 5;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarWave") + " " + SpiderWarWave + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.CorklidQueen.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);
								break;
							}
							case 10:
							{
								SpiderWarMaxPoints = int.MaxValue;
								Main.NewText(Language.GetTextValue("Mods.Spooky.EventsAndBosses.SpiderWarFinalWave") + ": " + 
								Language.GetTextValue("Mods.Spooky.NPCs.OgreKing.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.EmperorMortar.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.CorklidQueen.DisplayName") + ", " + Language.GetTextValue("Mods.Spooky.NPCs.CamelColonel.DisplayName") + ", " +
								Language.GetTextValue("Mods.Spooky.NPCs.EmpressJoro.DisplayName"), TextColor);

								ModContent.GetInstance<EventAchievementSpiderWar>().SpiderWarCondition.Complete();
								
								break;
							}
						}
					}
				}
			}
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (SpiderWarActive && SpiderWarWave > 0 && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
			{
				int EventIndex = layers.FindIndex(layer => layer is not null && layer.Name.Equals("Vanilla: Inventory"));
				LegacyGameInterfaceLayer NewLayer = new LegacyGameInterfaceLayer("Spooky: Spider War UI",
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
			if (SpiderWarActive && Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpiderCaveBiome>()))
			{
				const float Scale = 0.875f;
				const float Alpha = 0.5f;
				const int InternalOffset = 6;
				const int OffsetX = 20;
				const int OffsetY = 20;

				Texture2D EventIcon = ModContent.Request<Texture2D>("Spooky/Content/Biomes/SpiderCaveBiomeIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
				Color descColor = new Color(130, 92, 58);
				Color waveColor = new Color(186, 214, 77);

				int width = (int)(200f * Scale);
				int height = (int)(46f * Scale);

				Rectangle waveBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 23f), new Vector2(width, height));
				Utils.DrawInvBG(spriteBatch, waveBackground, new Color(130, 92, 58, 255) * 0.785f);

				string waveText = "Wave " + SpiderWarWave + ": " + ((SpiderWarPoints * 100) / SpiderWarMaxPoints) + "%";
				if (SpiderWarWave >= 10)
				{
					waveText = "Wave " + SpiderWarWave + ": " + SpiderWarPoints + " " + "Kills";
				}

				Utils.DrawBorderString(spriteBatch, waveText, new Vector2(waveBackground.Center.X, waveBackground.Y + 5), Color.White, Scale, 0.5f, -0.1f);
				Rectangle waveProgressBar = Utils.CenteredRectangle(new Vector2(waveBackground.Center.X, waveBackground.Y + waveBackground.Height * 0.75f), TextureAssets.ColorBar.Size());

				if (SpiderWarWave < 10)
				{
					var waveProgressAmount = new Rectangle(0, 0, (int)(TextureAssets.ColorBar.Width() * 0.01f * MathHelper.Clamp(((SpiderWarDisplayPoints * 100) / SpiderWarMaxPoints), 0f, 100f)), TextureAssets.ColorBar.Height());
					var offset = new Vector2((waveProgressBar.Width - (int)(waveProgressBar.Width * Scale)) * 0.5f, (waveProgressBar.Height - (int)(waveProgressBar.Height * Scale)) * 0.5f);
					spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, null, Color.White * Alpha, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);
					spriteBatch.Draw(TextureAssets.ColorBar.Value, waveProgressBar.Location.ToVector2() + offset, waveProgressAmount, waveColor, 0f, new Vector2(0f), Scale, SpriteEffects.None, 0f);
				}

				Vector2 descSize = new Vector2(154, 40) * Scale;
				Rectangle barrierBackground = Utils.CenteredRectangle(new Vector2(Main.screenWidth - OffsetX - 100f, Main.screenHeight - OffsetY - 19f), new Vector2(width, height));
				Rectangle descBackground = Utils.CenteredRectangle(new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), descSize * 0.9f);
				Utils.DrawInvBG(spriteBatch, descBackground, descColor * Alpha);

				int descOffset = (descBackground.Height - (int)(32f * Scale)) / 2;
				var icon = new Rectangle(descBackground.X + descOffset + 7, descBackground.Y + descOffset, (int)(32 * Scale), (int)(32 * Scale));
				spriteBatch.Draw(EventIcon, icon, Color.White);
				Utils.DrawBorderString(spriteBatch, "Spider War", new Vector2(barrierBackground.Center.X, barrierBackground.Y - InternalOffset - descSize.Y * 0.5f), Color.White, 0.8f, 0.3f, 0.4f);
			}
		}
	}
}