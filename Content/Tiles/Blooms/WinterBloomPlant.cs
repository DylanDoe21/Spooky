using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;
using Spooky.Content.Items.Blooms;

namespace Spooky.Content.Tiles.Blooms
{
	public class WinterBloomPlant : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Blooms/BloomPlantTestTexture";

		private Asset<Texture2D> PlantTexture;

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.WaterDeath = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.AnchorValidTiles = new[] { ModContent.TileType<BloomSoil>() };
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(204, 223, 216), name);
			RegisterItemDrop(ModContent.ItemType<WinterSeed>());
			DustType = DustID.Grass;
			HitSound = SoundID.Grass;
		}

		public static void DrawPlant(int i, int j, Texture2D tex, Rectangle? source, Vector2? offset = null, Vector2? origin = null)
		{
			Vector2 drawPos = new Vector2(i, j).ToWorldCoordinates() - Main.screenPosition + (offset ?? new Vector2(0, -2));

			Main.spriteBatch.Draw(tex, drawPos, source, Lighting.GetColor(i, j), 0, origin ?? source.Value.Size() / 3f, 1f, SpriteEffects.None, 0f);
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			//do not draw the tile texture itself
			return false;
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			PlantTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Blooms/WinterBloomPlant");

			Tile tile = Framing.GetTileSafely(i, j);

			//draw the tile only on the bottom center of each tiles y-frame
			if (tile.TileFrameY == 36 || tile.TileFrameY == 90 || tile.TileFrameY == 144 || tile.TileFrameY == 198)
			{
				//also only draw the bloom tile on the middle x-frame
				if (tile.TileFrameX == 18 || tile.TileFrameX == 72 || tile.TileFrameX == 126 || tile.TileFrameX == 180 || tile.TileFrameX == 234)
				{
					//reminder: offset negative numbers are right and down, while positive is left and up
					Vector2 offset = new Vector2((PlantTexture.Width() / 2) - 1, (PlantTexture.Height() / 8) - 12);

					int frame = 0;

					//growth stage frames
					if (tile.TileFrameX == 18) frame = 0;
					if (tile.TileFrameX == 72) frame = 1;
					if (tile.TileFrameX == 126) frame = 2;
					if (tile.TileFrameX == 180) frame = 3;

					//fully grown frames
					if (tile.TileFrameX == 234)
					{
						if (tile.TileFrameY == 36) frame = 4;
						if (tile.TileFrameY == 90) frame = 5;
						if (tile.TileFrameY == 144) frame = 6;
						if (tile.TileFrameY == 198) frame = 7;
					}

					DrawPlant(i, j, PlantTexture.Value, new Rectangle(0, 58 * frame, 56, 56), TileGlobal.TileOffset, offset);
				}
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			if (frameX == 216)
			{
				bool ShouldDropExtra = Main.LocalPlayer.GetModPlayer<BloomBuffsPlayer>().FallWaterGourd && Main.rand.NextBool(7);

				if (ShouldDropExtra)
				{
					Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<WinterSeed>());
				}

				if (frameY == 0) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<WinterBlackberry>(), ShouldDropExtra ? 2 : 1);
				if (frameY == 54) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<WinterBlueberry>(), ShouldDropExtra ? 2 : 1);
				if (frameY == 108) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<WinterGooseberry>(), ShouldDropExtra ? 2 : 1);
				if (frameY == 162) Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i + 1, j) * 16, ModContent.ItemType<WinterStrawberry>(), ShouldDropExtra ? 2 : 1);
			}
		}

		public override void RandomUpdate(int i, int j)
		{
			Tile tile = Framing.GetTileSafely(i, j);

			bool ShouldGrowFaster = Main.LocalPlayer.GetModPlayer<BloomBuffsPlayer>().SummerSunflower;

			if (tile.TileFrameX < 216 && Main.rand.NextBool(ShouldGrowFaster ? 12 : 20))
			{
				int left = i - tile.TileFrameX / 18 % 3;
				int top = j - tile.TileFrameY / 18 % 3;

				for (int x = left; x < left + 3; x++)
				{
					for (int y = top; y < top + 3; y++)
					{
						Tile CheckTile = Framing.GetTileSafely(x, y);
						CheckTile.TileFrameX += 54;
					}
				}

				if (Main.netMode != NetmodeID.SinglePlayer)
				{
					NetMessage.SendTileSquare(-1, left, top, 6);
				}
			}
		}
	}
}