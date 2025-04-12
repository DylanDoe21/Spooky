using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.NPCs.Boss.SpookySpirit;
using Spooky.Content.NPCs.Boss.SpookySpirit.Projectiles;
using Spooky.Content.NPCs.Cemetery;
using Spooky.Content.NPCs.Cemetery.Projectiles;
using Spooky.Content.NPCs.PandoraBox;
using Spooky.Content.Tiles.Cemetery.Furniture;

namespace Spooky.Content.Items.BossSummon
{
    public class EMFReader : ModItem
    {
		public static readonly SoundStyle BeepSound1 = new("Spooky/Content/Sounds/EMFNoGhost", SoundType.Sound);
		public static readonly SoundStyle BeepSound2 = new("Spooky/Content/Sounds/EMFGhost", SoundType.Sound);
		public static readonly SoundStyle BeepSound3 = new("Spooky/Content/Sounds/EMFGhostPowerful", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Type] = 12;
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 30;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

		public override bool? UseItem(Player player)
		{
			CheckTilesAroundPlayer(player);
			return true;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<EMFReaderBroke>())
            .AddRecipeGroup("SpookyMod:DemoniteBars", 5)
            .AddRecipeGroup(RecipeGroupID.IronBar, 2)
            .AddTile(TileID.Anvils)
            .Register();
        }

		public void CheckTilesAroundPlayer(Player player)
		{
			Vector2 playerTileCenter = new Vector2(player.Center.X / 16, player.Center.Y / 16);

			//used so spooky spirit altar has priority over gravestones
			bool CanSpawnMistGhost = true;
			bool FoundValidGrave = false;
			bool FoundValidAltar = false;

			if (NPC.AnyNPCs(ModContent.NPCType<SpookySpirit>()) || PandoraBoxWorld.PandoraEventActive)
			{
				SoundEngine.PlaySound(BeepSound3, player.Center);
				CombatText.NewText(player.getRect(), Color.Crimson, Language.GetTextValue("Mods.Spooky.EventsAndBosses.EMFReaderSpiritExists"));
				return;
			}

			foreach (var npc in Main.ActiveNPCs)
			{
				if (npc.type == ModContent.NPCType<PandoraBox>() && npc.Distance(player.Center) <= 200f)
				{
					SoundEngine.PlaySound(BeepSound3, player.Center);
					CombatText.NewText(player.getRect(), Color.Cyan, Language.GetTextValue("Mods.Spooky.EventsAndBosses.EMFReaderPandoraBox"));

					npc.ai[2] = 1;
					npc.netUpdate = true;
					return;
				}
			}

			for (int i = (int)playerTileCenter.X - 8; i <= (int)playerTileCenter.X + 8; i++)
			{
				if (i == (int)playerTileCenter.X + 8 && FoundValidAltar)
				{
					SoundEngine.PlaySound(BeepSound3, player.Center);
					CombatText.NewText(player.getRect(), Color.MediumPurple, Language.GetTextValue("Mods.Spooky.EventsAndBosses.EMFReaderSpookySpirit"));
				}

				for (int j = (int)playerTileCenter.Y - 5; j <= (int)playerTileCenter.Y + 5; j++)
				{
					if (Main.tile[i, j].TileType == ModContent.TileType<SpiritAltar>())
					{
						FoundValidAltar = true;
						CanSpawnMistGhost = false;

						Tile tile = Framing.GetTileSafely(i, j);

						int left = i - tile.TileFrameX / 18 % 3;
						int top = j - tile.TileFrameY / 18 % 5;

						bool CanSpawn = true;

						if (NPC.AnyNPCs(ModContent.NPCType<SpookySpirit>()) || Main.dayTime)
						{
							CanSpawn = false;
						}

						foreach (var Proj in Main.ActiveProjectiles)
						{
							if (Proj.type == ModContent.ProjectileType<SpookySpiritSpawn>())
							{
								CanSpawn = false;
								break;
							}
						}

						if (CanSpawn)
						{
							int SpawnX = (left * 16) + 25;
							int SpawnY = (top * 16) + 35;

							Projectile.NewProjectile(new EntitySource_TileInteraction(player, SpawnX, SpawnY), new Vector2(SpawnX, SpawnY), new Vector2(0, -1),
							ModContent.ProjectileType<SpookySpiritSpawn>(), 0, 0);
						}
					}
				}
			}

			if (CanSpawnMistGhost)
			{
				for (int i = (int)playerTileCenter.X - 8; i <= (int)playerTileCenter.X + 8; i++)
				{
					if (i == (int)playerTileCenter.X + 8 && !FoundValidGrave)
					{
						SoundEngine.PlaySound(BeepSound1, player.Center);
						CombatText.NewText(player.getRect(), Color.Lime, Language.GetTextValue("Mods.Spooky.EventsAndBosses.EMFReaderNoGhost"));
					}

					for (int j = (int)playerTileCenter.Y - 5; j <= (int)playerTileCenter.Y + 5; j++)
					{
						if (Main.tile[i, j].TileType == ModContent.TileType<MysteriousTombstone>())
						{
							FoundValidGrave = true;

							Tile tile = Framing.GetTileSafely(i, j);

							int left = i - tile.TileFrameX / 18 % 2;
							int top = j - tile.TileFrameY / 18 % 3;

							bool CanSpawn = true;

							foreach (var Proj in Main.ActiveProjectiles)
							{
								for (int x = left; x < left + 2; x++)
								{
									for (int y = top; y < top + 3; y++)
									{
										if (Proj.type == ModContent.ProjectileType<MistGhostSpawn>() && Proj.Hitbox.Intersects(new Rectangle(x * 16, y * 16, 16, 16)))
										{
											CanSpawn = false;
											break;
										}
									}
								}
							}

							if (CanSpawn && !NPC.AnyNPCs(ModContent.NPCType<MistGhost>()) && !NPC.AnyNPCs(ModContent.NPCType<MistGhostFaces>()) && !NPC.AnyNPCs(ModContent.NPCType<MistGhostWiggle>()))
							{
								SoundEngine.PlaySound(BeepSound2, player.Center);
								CombatText.NewText(player.getRect(), Color.Orange, Language.GetTextValue("Mods.Spooky.EventsAndBosses.EMFReaderGhost"));

								int SpawnX = (left * 16) + 16;
								int SpawnY = (top * 16) + 20;

								//spawn a mist ghost ambush
								Projectile.NewProjectile(new EntitySource_TileInteraction(player, SpawnX, SpawnY), new Vector2(SpawnX, SpawnY),
								Vector2.Zero, ModContent.ProjectileType<MistGhostSpawn>(), 0, 0);
								return;
							}
						}
					}
				}
			}
		}
    }
}