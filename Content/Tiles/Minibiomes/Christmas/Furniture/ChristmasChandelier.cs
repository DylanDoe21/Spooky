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

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasChandelier : ModTile
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
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(72, 88, 88), name);
            RegisterItemDrop(ModContent.ItemType<ChristmasLampItem>());
            DustType = DustID.Stone;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Chandeliers };
        }

        /*
        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<ChristmasChandelierItem>());
        }
        */

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

                r = 112f / divide;
                g = 147f / divide;
                b = 158f / divide;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/Furniture/ChristmasChandelierGlow");

            SpriteEffects effects = SpriteEffects.None;

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

			ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (long)(uint)i); // Don't remove any casts.

			// We can support different flames for different styles here: int style = Main.tile[j, i].frameY / 54;
			for (int c = 0; c < 7; c++) 
			{
				float shakeX = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
				float shakeY = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
				spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f + shakeX, j * 16 - (int)Main.screenPosition.Y + offsetY + shakeY) + zero, new Rectangle(frameX, frameY, width, height), new Color(100, 100, 100, 0), 0f, default, 1f, effects, 0f);
			}
        }
    }
}