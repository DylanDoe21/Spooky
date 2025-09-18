using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasLantern : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.DrawYOffset = -10;
			TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(72, 88, 88), name);
            RegisterItemDrop(ModContent.ItemType<ChristmasLanternItem>());
            DustType = DustID.Stone;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.HangingLanterns };
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

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 1;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            for (int x = left; x < left + 1; x++)
            {
                for (int y = top; y < top + 2; y++)
                {
                    if (Main.tile[x, y].TileFrameX >= 18)
                    {
                        Main.tile[x, y].TileFrameX -= 18;
                    }
                    else
                    {
                        Main.tile[x, y].TileFrameX += 18;
                    }
                }
            }

            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
            }

            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                float divide = 300f;

                r = 112f / divide;
                g = 147f / divide;
                b = 158f / divide;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Christmas/Furniture/ChristmasLanternGlow");

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