using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class Moyai : ModTile
	{
		public const int FrameWidth = 18 * 4;
		public const int FrameHeight = 18 * 6;
		public const int HorizontalFrames = 1;
		public const int VerticalFrames = 1;

		public Asset<Texture2D> RelicTexture;
		
		public virtual string RelicTextureName => "Spooky/Content/Tiles/Catacomb/Furniture/MoyaiDraw";

		public override string Texture => "Spooky/Content/Tiles/Catacomb/Furniture/Moyai";

		public override void Load() 
        {
			if (!Main.dedServ) 
            {
				RelicTexture = ModContent.Request<Texture2D>(RelicTextureName);
			}
		}

		public override void Unload() 
        {
			RelicTexture = null;
		}

		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 6;	
			TileObjectData.newTile.Origin = new Point16(2, 5);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(229, 197, 61));
			DustType = DustID.Gold;
			HitSound = SoundID.Tink;
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
				Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
			}
		}

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch) 
        {
			Vector2 offScreen = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen) 
            {
				offScreen = Vector2.Zero;
			}

			Point p = new Point(i, j);
			Tile tile = Main.tile[p.X, p.Y];

			if (tile == null || !tile.HasTile) 
            {
				return;
			}

			Texture2D texture = RelicTexture.Value;

			int frameY = tile.TileFrameX / FrameWidth;
			Rectangle frame = texture.Frame(HorizontalFrames, VerticalFrames, 0, frameY);

			Vector2 origin = frame.Size() / 2f;
			Vector2 worldPos = p.ToWorldCoordinates(33f, 75f);

			Color color = Lighting.GetColor(p.X, p.Y);

			bool direction = tile.TileFrameY / FrameHeight != 0;
			SpriteEffects effects = Main.LocalPlayer.Center.X < worldPos.X ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			const float TwoPi = (float)Math.PI * 2f;
			float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
			Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

			spriteBatch.Draw(texture, drawPos, frame, color, 0f, origin, 1f, effects, 0f);

			float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
			Color effectColor = color;
			effectColor.A = 0;
			effectColor = effectColor * 0.1f * scale;

			for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI)) 
			{
				spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), frame, effectColor, 0f, origin, 1f, effects, 0f);
			}
		}
	}
}