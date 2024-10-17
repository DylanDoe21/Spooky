using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.GameContent;
using Terraria.GameContent.ObjectInteractions;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class CultistSofa : ModTile
	{
		public const int Height = 38;
		public const int NextStyleWidth = 54;

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.CanBeSatOnForPlayers[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(98, 67, 82), name);
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
			DustType = DustID.Stone;
			AdjTiles = new int[] { TileID.Benches };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = fail ? 1 : 3;
		}

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
		{
			return settings.player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance);
		}

		public override void ModifySittingTargetInfo(int i, int j, ref TileRestingInfo info)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			info.TargetDirection = info.RestingEntity.direction;
			info.DirectionOffset = 0;
			Vector2 leftOffset = new Vector2(-4f, 2f);
			Vector2 rightOffset = new Vector2(4f, 2f);
			Vector2 centerOffset = new Vector2(0f, 2f);
			centerOffset.Y = leftOffset.Y = rightOffset.Y = 1f;

			Vector2 bonusOffset = Vector2.Zero;
			bonusOffset.X = 1f;

			info.FinalOffset.X = -1f;

			info.AnchorTilePosition.X = i;
			info.AnchorTilePosition.Y = j;

			if (tile.TileFrameY % Height == 0)
			{
				info.AnchorTilePosition.Y++;
			}

			if ((tile.TileFrameX % NextStyleWidth == 0 && info.TargetDirection == -1) || (tile.TileFrameX % NextStyleWidth == 36 && info.TargetDirection == 1))
			{
				info.VisualOffset = leftOffset;
			}
			else if ((tile.TileFrameX % NextStyleWidth == 0 && info.TargetDirection == 1) || (tile.TileFrameX % NextStyleWidth == 36 && info.TargetDirection == -1))
			{
				info.VisualOffset = rightOffset;
			}
			else
			{
				info.VisualOffset = centerOffset;
			}

			info.VisualOffset += bonusOffset;
		}

		public override bool RightClick(int i, int j)
		{
			Player player = Main.LocalPlayer;

			if (player.IsWithinSnappngRangeToTile(i, j, PlayerSittingHelper.ChairSittingMaxDistance))
			{
				//Avoid being able to trigger it from long range
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
				//Match condition in RightClick. Interaction should only show if clicking it does something
				return;
			}

			player.noThrow = 2;
			player.cursorItemIconEnabled = true;

			Tile tile = Main.tile[i, j];
			int style = tile.TileFrameX / NextStyleWidth;
			int item = TileLoader.GetItemDropFromTypeAndStyle(tile.TileType, style);
			if (item > 0)
			{
				player.cursorItemIconID = item;
			}
		}
	}
}