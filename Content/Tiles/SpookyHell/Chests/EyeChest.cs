using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.Audio;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.Tiles.SpookyHell.Chests
{
	[LegacyName("EyeChest2")]
	[LegacyName("EyeChest3")]
	[LegacyName("EyeChest4")]
	public class EyeChest : ModTile
	{
		public override void SetStaticDefaults() 
		{
			Main.tileSpelunker[Type] = true;
			Main.tileContainer[Type] = true;
			Main.tileShine2[Type] = true;
			Main.tileShine[Type] = 1200;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileOreFinderPriority[Type] = 500;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.BasicChest[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			AdjTiles = new int[] { TileID.Containers };
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorInvalidTiles = new int[] { TileID.MagicalIceBlock };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(Type);
			ContainerName.SetDefault("Monster Chest");
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Monster Chest");
			AddMapEntry(new Color(255, 55, 41), name, MapChestName);
			name = CreateMapEntryName(Name + "_Sleeping"); // With multiple map entries, you need unique translation keys.
			name.SetDefault("Sleeping Eye Chest");
			AddMapEntry(new Color(255, 55, 41), name, MapChestName);
			DustType = DustID.Blood;
			HitSound = SoundID.Dig;
		}

		public override ushort GetMapOption(int i, int j) => (ushort)(Main.tile[i, j].TileFrameX / 36);

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
			return true;
        }

        public override bool IsLockedChest(int i, int j) => Main.tile[i, j].TileFrameX / 36 == 1;

		public override bool UnlockChest(int i, int j, ref short frameXAdjustment, ref int dustType, ref bool manual) 
		{
			SoundEngine.PlaySound(SoundID.Item2);
			dustType = -1;
			return true;
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
				return Language.GetTextValue("LegacyChestType.0");
			}

			if (Main.chest[chest].name == "") 
			{
				return name;
			}

			return name + ": " + Main.chest[chest].name;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = 1;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
		{
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<EyeChestItem>());
			Chest.DestroyChest(i, j);
		}

		public override bool RightClick(int i, int j) 
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			Main.mouseRightRelease = false;
			int left = i;
			int top = j;
			if (tile.TileFrameX % 36 != 0) 
			{
				left--;
			}

			if (tile.TileFrameY != 0) 
			{
				top--;
			}

			if (player.sign >= 0) 
			{
				SoundEngine.PlaySound(SoundID.ChesterOpen);
				player.sign = -1;
				Main.editSign = false;
				Main.npcChatText = "";
			}

			if (Main.editChest) 
			{
				SoundEngine.PlaySound(SoundID.ChesterOpen);
				Main.editChest = false;
				Main.npcChatText = "";
			}

			if (player.editedChestName) 
			{
				NetMessage.SendData(MessageID.SyncPlayerChest, -1, -1, NetworkText.FromLiteral(Main.chest[player.chest].name), player.chest, 1f);
				player.editedChestName = false;
			}

			bool isLocked = IsLockedChest(left, top);
			if (Main.netMode == NetmodeID.MultiplayerClient && !isLocked) 
			{
				if (left == player.chestX && top == player.chestY && player.chest >= 0) 
				{
					player.chest = -1;
					Recipe.FindRecipes();
					SoundEngine.PlaySound(SoundID.ChesterOpen);
				}
				else 
				{
					NetMessage.SendData(MessageID.RequestChestOpen, -1, -1, null, left, top);
					Main.stackSplit = 600;
				}
			}
			else 
			{
				if (isLocked) 
				{
					int key = ModContent.ItemType<ChestFood>();
					if (player.ConsumeItem(key) && Chest.Unlock(left, top)) 
					{
						if (Main.netMode == NetmodeID.MultiplayerClient) 
						{
							NetMessage.SendData(MessageID.Unlock, -1, -1, null, player.whoAmI, 1f, left, top);
						}
					}
					else
					{
						Main.NewText("The chest creature won't budge", 199, 7, 49);
					}
				}
				else 
				{
					int chest = Chest.FindChest(left, top);
					if (chest >= 0) 
					{
						Main.stackSplit = 600;
						if (chest == player.chest) 
						{
							player.chest = -1;
							SoundEngine.PlaySound(SoundID.ChesterOpen);
						}
						else 
						{
							player.chest = chest;
							Main.playerInventory = true;
							Main.recBigList = false;
							player.chestX = left;
							player.chestY = top;
							SoundEngine.PlaySound(SoundID.ChesterOpen);
						}

						Recipe.FindRecipes();
					}
				}
			}

			return true;
		}

		public override void MouseOver(int i, int j) 
		{
			Player player = Main.LocalPlayer;
			Tile tile = Main.tile[i, j];
			int left = i;
			int top = j;
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
				player.cursorItemIconText = Language.GetTextValue("LegacyChestType.0");
			}
			else 
			{
				player.cursorItemIconText = Main.chest[chest].name.Length > 0 ? Main.chest[chest].name : "Eye Chest";
				if (player.cursorItemIconText == "Eye Chest") 
				{
					player.cursorItemIconID = ModContent.ItemType<EyeChestItem>();

					if (Main.tile[left, top].TileFrameX / 36 == 1) 
					{
						player.cursorItemIconID = ModContent.ItemType<ChestFood>();
					}

					player.cursorItemIconText = "";
				}
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
		}

		public override void MouseOverFar(int i, int j) 
		{
			MouseOver(i, j);
			Player player = Main.LocalPlayer;
			if (player.cursorItemIconText == "") 
			{
				player.cursorItemIconEnabled = false;
				player.cursorItemIconID = 0;
			}
		}
	}
}