using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Furniture
{
    public class LabScreen : ModTile
    {
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
        {
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(156, 154, 121));
            DustType = -1;
            HitSound = SoundID.NPCHit53 with { Volume = 0.5f };
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
            float divide = 700f;

			r = 255f / divide;
			g = 196f / divide;
			b = 0f / divide;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch) 
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/Furniture/LabScreenGlow");

            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
            int yOffset = TileObjectData.GetTileData(tile).DrawYOffset;
            spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16 + yOffset) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White);
		}
    }
}