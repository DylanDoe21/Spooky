using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class SpookyChair : ModTile
	{
		public const int NextStyleHeight = 40;

		public override void SetStaticDefaults() 
        {
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			TileID.Sets.CanBeSatOnForNPCs[Type] = true;
			TileID.Sets.CanBeSatOnForPlayers[Type] = true; 
			TileID.Sets.DisableSmartCursor[Type] = true;
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
			AdjTiles = new int[] { TileID.Chairs };
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
            ModTranslation name = CreateMapEntryName();
			name.SetDefault("Chair");
			AddMapEntry(new Color(65, 52, 32), name);
            DustType = DustID.WoodFurniture;
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
        {
			num = fail ? 1 : 3;
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
			Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 32, ModContent.ItemType<SpookyChairItem>());
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings) 
        {
			return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
		}

		public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info) 
        {
			Tile tile = Framing.GetTileSafely(i, j);

			info.TargetDirection = -1;
			if (tile.TileFrameX != 0) 
            {
				info.TargetDirection = 1; 
			}

			info.AnchorTilePosition.X = i;
			info.AnchorTilePosition.Y = j;

			if (tile.TileFrameY % NextStyleHeight == 0) 
            {
				info.AnchorTilePosition.Y++;
			}
		}

		public override bool RightClick(int i, int j) 
        {
			Player player = Main.LocalPlayer;

			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance)) 
            { 
				player.GamepadEnableGrappleCooldown();
				player.sitting.SitDown(player, i, j);
			}

			return true;
		}

		public override void MouseOver(int i, int j) 
        {
			Player player = Main.LocalPlayer;

			if (!player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance)) 
            {
				return;
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;
			player.cursorItemIconID = ModContent.ItemType<SpookyChairItem>();

			if (Main.tile[i, j].TileFrameX / 18 < 1) 
            {
				player.cursorItemIconReversed = true;
			}
		}
	}
}