using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Spooky.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Spooky.Content.Tiles.Relic
{
	public class DunkleosteusRelic : ModTile
	{
        public override string Texture => "Spooky/Content/Tiles/Relic/RelicPedestal";

        public const int FrameWidth = 18 * 3;
		public const int FrameHeight = 18 * 4;
		public const int HorizontalFrames = 1;
		public const int VerticalFrames = 1;

        private Asset<Texture2D> RelicTexture1;
		private Asset<Texture2D> RelicTexture2;
		private Asset<Texture2D> RelicTexture3;

		private Asset<Texture2D> RelicBottomTexture1;
		private Asset<Texture2D> RelicBottomTexture2;
		private Asset<Texture2D> RelicBottomTexture3;

		public static readonly SoundStyle PlaceSound = new("Spooky/Content/Sounds/Dunkleosteus/DunkleosteusVictory", SoundType.Sound);

		public override void SetStaticDefaults() 
        {
			Main.tileShine[Type] = 400;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.InteractibleByNPCs[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2; 
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = false;
			TileObjectData.newTile.StyleWrapLimitVisualOverride = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.styleLineSkipVisualOverride = 0;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
		}

		public override void PlaceInWorld(int i, int j, Item item)
		{
			int maxConfetti = 7;
			float soundPitch = 0f;

			if (Main.expertMode && !Main.masterMode)
			{
				maxConfetti = 15;
				soundPitch = 0.5f;
			}
			if (Main.masterMode)
			{
				maxConfetti = 30;
				soundPitch = 1f;
			}

			SoundEngine.PlaySound(PlaceSound with { Pitch = soundPitch }, new Vector2(i * 16, j * 16));
			SoundEngine.PlaySound(SoundID.ResearchComplete with { Pitch = 1f }, new Vector2(i * 16, j * 16));

			for (int numGores = 1; numGores <= maxConfetti; numGores++)
			{
				if (Main.netMode != NetmodeID.Server)
				{
					Gore.NewGore(null, new Vector2(i * 16, j * 16), new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -2)), Main.rand.Next(276, 283));
				}
			}
		}

		public override bool CreateDust(int i, int j, ref int type) 
        {
			return false;
		}

		public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY) 
        {
			tileFrameX %= FrameWidth; 
			tileFrameY %= FrameHeight * 2; 
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData) 
		{
			if (drawData.tileFrameX % FrameWidth == 0 && drawData.tileFrameY % FrameHeight == 0) 
			{
				Main.instance.TilesRenderer.AddSpecialPoint(i, j, Terraria.GameContent.Drawing.TileDrawing.TileCounterType.CustomNonSolid);
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
			RelicBottomTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/RelicPedestalCopper");
			RelicBottomTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/RelicPedestalSilver");
			RelicBottomTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/RelicPedestal");

			Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;

			if (!Main.expertMode)
			{
				spriteBatch.Draw(RelicBottomTexture1.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(new Point(i, j)));
			}
			else if (Main.expertMode && !Main.masterMode)
			{
				spriteBatch.Draw(RelicBottomTexture2.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(new Point(i, j)));
			}
			else if (Main.masterMode)
			{
				spriteBatch.Draw(RelicBottomTexture3.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, 
				new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Lighting.GetColor(new Point(i, j)));
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
		{
			// Take the tile, check if it actually exists
			Point p = new Point(i, j);
			Tile tile = Main.tile[p.X, p.Y];
			if (!tile.HasTile) 
			{
				return;
			}

			// Get the initial draw parameters
			RelicTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicCopper");
			RelicTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicSilver");
			RelicTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Relic/DunkleosteusRelicGold");

			int frameY = tile.TileFrameX / FrameWidth; // Picks the frame on the sheet based on the placeStyle of the item
			Rectangle frame = RelicTexture1.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

			Vector2 origin = frame.Size() / 2f;
			Vector2 worldPos = p.ToWorldCoordinates(24f, 64f);

			Color color = Lighting.GetColor(p.X, p.Y);

			bool direction = tile.TileFrameY / FrameHeight != 0; // This is related to the alternate tile data we registered before
			SpriteEffects effects = direction ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			// Some math magic to make it smoothly move up and down over time
			const float TwoPi = (float)Math.PI * 2f;
			float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
			Vector2 drawPos = worldPos - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

			// Draw the main texture
			if (!Main.expertMode)
			{
				spriteBatch.Draw(RelicTexture1.Value, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
			}
			else if (Main.expertMode && !Main.masterMode)
			{
				spriteBatch.Draw(RelicTexture2.Value, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
			}
			else if (Main.masterMode)
			{
				spriteBatch.Draw(RelicTexture3.Value, drawPos, frame, color, 0f, origin, 1f, effects, 0f);
			}

			// Draw the periodic glow effect
			float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
			Color effectColor = color;
			effectColor.A = 0;
			effectColor = effectColor * 0.1f * scale;
			for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI)) 
			{
				if (!Main.expertMode)
				{
					spriteBatch.Draw(RelicTexture1.Value, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
				}
				else if (Main.expertMode && !Main.masterMode)
				{
					spriteBatch.Draw(RelicTexture2.Value, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
				}
				else if (Main.masterMode)
				{
					spriteBatch.Draw(RelicTexture3.Value, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
				}
			}
		}
	}
}