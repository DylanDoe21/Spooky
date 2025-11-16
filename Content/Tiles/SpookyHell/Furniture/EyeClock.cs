using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using Terraria.GameContent.ObjectInteractions;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
    public class EyeClock : ModTile
	{
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.Clock[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(0, 4);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(114, 13, 39), Lang.GetItemName(ItemID.GrandfatherClock));
            DustType = DustID.Blood;
			AdjTiles = new int[] { TileID.GrandfatherClocks };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

		public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

		public override bool RightClick(int x, int y)
		{
			string text = "AM";
			//Get current weird time
			double time = Main.time;
			if (!Main.dayTime)
			{
				//if it's night add this number
				time += 54000.0;
			}

			//Divide by seconds in a day * 24
			time = time / 86400.0 * 24.0;
			//Dunno why we're taking 19.5. Something about hour formatting
			time = time - 7.5 - 12.0;
			//Format in readable time
			if (time < 0.0)
			{
				time += 24.0;
			}

			if (time >= 12.0)
			{
				text = "PM";
			}

			int intTime = (int)time;
			//Get the decimal points of time.
			double deltaTime = time - intTime;
			//multiply them by 60. Minutes, probably
			deltaTime = (int)(deltaTime * 60.0);
			//This could easily be replaced by deltaTime.ToString()
			string text2 = string.Concat(deltaTime);
			if (deltaTime < 10.0)
			{
				//if deltaTime is eg "1" (which would cause time to display as HH:M instead of HH:MM)
				text2 = "0" + text2;
			}

			if (intTime > 12)
			{
				//This is for AM/PM time rather than 24hour time
				intTime -= 12;
			}

			if (intTime == 0)
			{
				//0AM = 12AM
				intTime = 12;
			}

			//Whack it all together to get a HH:MM format
			Main.NewText($"Time: {intTime}:{text2} {text}", 255, 240, 20);
			return true;
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyHell/Furniture/EyeClockGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
        }
    }
}