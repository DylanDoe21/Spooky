using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyeCandle : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(114, 13, 39), name);
            DustType = DustID.Blood;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Candles };
        }

        public override void HitWire(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX >= 18)
            {
                Main.tile[i, j].TileFrameX -= 18;
            }
            else
            {
                Main.tile[i, j].TileFrameX += 18;
            }
        }

        public override bool RightClick(int i, int j)
        {
            Main.player[Main.myPlayer].PickTile(i, j, 100);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<EyeCandleItem>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                r = 0.45f;
                g = 0.45f;
                b = 0.45f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
			Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Furniture/EyeCandleGlow").Value;
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}