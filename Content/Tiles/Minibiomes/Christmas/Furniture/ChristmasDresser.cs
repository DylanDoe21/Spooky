using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
    public class ChristmasDresser : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileTable[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.BasicDresser[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { 127 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(143, 97, 86), this.GetLocalization("MapEntry"), MapChestName);
            DustType = DustID.WoodFurniture;
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			AdjTiles = new int[] { TileID.Dressers };
		}

        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            return this.GetLocalization("ContainerName");
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) 
        {
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Chest.DestroyChest(i, j);
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;
			if (Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY == 0)
			{
				Main.CancelClothesWindow(true);
				Main.mouseRightRelease = false;
				int left = Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameX / 18;
				left %= 3;
				left = Player.tileTargetX - left;
				int top = Player.tileTargetY - Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY / 18;
				if (player.sign > -1)
				{
					SoundEngine.PlaySound(SoundID.MenuClose);
					player.sign = -1;
					Main.editSign = false;
					Main.npcChatText = string.Empty;
				}
				if (Main.editChest)
				{
					SoundEngine.PlaySound(SoundID.MenuTick);
					Main.editChest = false;
					Main.npcChatText = string.Empty;
				}
				if (player.editedChestName)
				{
					NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f, 0f, 0f, 0, 0, 0);
					player.editedChestName = false;
				}
				if (Main.netMode == NetmodeID.MultiplayerClient)
				{
					if (left == player.chestX && top == player.chestY && player.chest != -1)
					{
						player.chest = -1;
						Recipe.FindRecipes();
						SoundEngine.PlaySound(SoundID.MenuClose);
					}
					else
					{
						NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top, 0f, 0f, 0, 0, 0);
						Main.stackSplit = 600;
					}
				}
				else
				{
					player.piggyBankProjTracker.Clear();
					player.voidLensChest.Clear();
					int num213 = Chest.FindChest(left, top);
					if (num213 != -1)
					{
						Main.stackSplit = 600;
						if (num213 == player.chest)
						{
							player.chest = -1;
							Recipe.FindRecipes();
							SoundEngine.PlaySound(SoundID.MenuClose);
						}
						else if (num213 != player.chest && player.chest == -1)
						{
							player.chest = num213;
							Main.playerInventory = true;
							Main.recBigList = false;
							SoundEngine.PlaySound(SoundID.MenuOpen);
							player.chestX = left;
							player.chestY = top;
						}
						else
						{
							player.chest = num213;
							Main.playerInventory = true;
							Main.recBigList = false;
							SoundEngine.PlaySound(SoundID.MenuTick);
							player.chestX = left;
							player.chestY = top;
						}
						Recipe.FindRecipes();
					}
				}
			}
			else
			{
				Main.playerInventory = false;
				player.chest = -1;
				Recipe.FindRecipes();
				Main.interactedDresserTopLeftX = Player.tileTargetX;
				Main.interactedDresserTopLeftY = Player.tileTargetY;
				Main.OpenClothesWindow();
			}
            return true;
		}

		public override void MouseOverFar(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			int left = Player.tileTargetX;
			int top = Player.tileTargetY;
			left -= tile.TileFrameX % 54 / 18;
			if (tile.TileFrameY % 36 != 0)
			{
				top--;
			}
			int chestIndex = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;
			if (chestIndex < 0)
			{
				player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
			}
			else
			{
				if (Main.chest[chestIndex].name != "")
				{
					player.cursorItemIconText = Main.chest[chestIndex].name;
				}
				else
				{
					player.cursorItemIconText = "Cultist Dresser";
				}
				if (player.cursorItemIconText == "Cultist Dresser")
				{
					player.cursorItemIconID = ModContent.ItemType<ChristmasDresserItem>();
					player.cursorItemIconText = "";
				}
			}
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			if (player.cursorItemIconText == "")
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			int left = Player.tileTargetX;
			int top = Player.tileTargetY;
			left -= tile.TileFrameX % 54 / 18;
			if (tile.TileFrameY % 36 != 0)
			{
				top--;
			}
			int num138 = Chest.FindChest(left, top);
			player.cursorItemIconID = -1;
			if (num138 < 0)
			{
				player.cursorItemIconText = Language.GetTextValue("LegacyDresserType.0");
			}
			else
			{
				if (Main.chest[num138].name != "")
				{
					player.cursorItemIconText = Main.chest[num138].name;
				}
				else
				{
					player.cursorItemIconText = "Cultist Dresser";
				}
				if (player.cursorItemIconText == "Cultist Dresser")
				{
					player.cursorItemIconID = ModContent.ItemType<ChristmasDresserItem>();
					player.cursorItemIconText = "";
				}
			}
            
			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			if (Main.tile[Player.tileTargetX, Player.tileTargetY].TileFrameY > 0)
			{
				player.cursorItemIconID = ItemID.FamiliarShirt;
			}
        }

        public static string MapChestName(string name, int i, int j)
        {
            int left = i;
            int top = j;
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX % 36 != 0)
            {
                left--;
            }

            if (tile.TileFrameY != 0)
            {
                top--;
            }

            int chest = Chest.FindChest(left, top);
            if (chest < 0)
            {
                return Language.GetTextValue("LegacyDresserType.0");
            }

            if (Main.chest[chest].name == "")
            {
                return name;
            }

            return name + ": " + Main.chest[chest].name;
        }
    }
}