using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class CultistChandelier : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.DrawYOffset = -10;
			TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(98, 67, 82), name);
            DustType = DustID.Stone;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Chandeliers };
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			if ((Framing.GetTileSafely(i, j - 1).HasTile && TileID.Sets.Platforms[Framing.GetTileSafely(i, j - 1).TileType]) ||
            (Framing.GetTileSafely(i, j - 2).HasTile && TileID.Sets.Platforms[Framing.GetTileSafely(i, j - 2).TileType]) ||
            (Framing.GetTileSafely(i, j - 3).HasTile && TileID.Sets.Platforms[Framing.GetTileSafely(i, j - 3).TileType]))
			{
				offsetY -= 8;
			}
		}

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<CultistChandelierItem>());
        }

        public override void HitWire(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int width = 3;
			int height = 3;
			int x = i - tile.TileFrameX / 18 % width;
			int y = j - tile.TileFrameY / 18 % height;

			for (int l = x; l < x + width; l++)
			{
				for (int m = y; m < y + height; m++)
				{
					Tile checkTile = Framing.GetTileSafely(l, m);
					if (checkTile.HasTile && checkTile.TileType == Type)
					{
						if (checkTile.TileFrameX != 108)
						{
							checkTile.TileFrameX += 54;
						}
						if (checkTile.TileFrameX >= 108)
						{
							checkTile.TileFrameX -= 108;
						}
					}

					if (Wiring.running)
					{
						Wiring.SkipWire(l, m);
					}
				}
			}

			int w2 = width / 2;
			int h2 = height / 2;
			NetMessage.SendTileSquare(-1, x + w2 - 1, y + h2, 1 + w2 + h2);
		}

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 36)
            {
                float divide = 250f;

                r = 138f / divide;
                g = 234f / divide;
                b = 108f / divide;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/NoseTemple/Furniture/CultistChandelierGlow");

            Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

			if (Main.drawToScreen) 
			{
				zero = Vector2.Zero;
			}

			Tile tile = Main.tile[i, j];
			int width = 16;
			int offsetY = 0;
			int height = 16;
			short frameX = tile.TileFrameX;
			short frameY = tile.TileFrameY;

			TileLoader.SetDrawPositions(i, j, ref width, ref offsetY, ref height, ref frameX, ref frameY);

            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero, 
            new Rectangle(frameX, frameY, width, height), Color.White, 0f, default, 1f, SpriteEffects.None, 0f);
        }
    }
}