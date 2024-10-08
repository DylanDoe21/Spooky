using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
	public class SpookyVinesGreen : ModTile
	{
        private Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = false;
			TileID.Sets.IsVine[Type] = true;
            TileID.Sets.VineThreads[Type] = true;
			AddMapEntry(new Color(62, 95, 38));
			DustType = ModContent.DustType<SpookyGrassDustGreen>();
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.2f;
			g = 0.1f;
			b = 0.01f;
        }

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
			int[] ValidTiles = { ModContent.TileType<SpookyVinesGreen>(), ModContent.TileType<SpookyGrassGreen>() };

			if (!ValidTiles.Contains(Main.tile[i, j - 1].TileType))
			{
				WorldGen.KillTile(i, j, false, false, false);
			}
			
            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }

		public override void RandomUpdate(int i, int j)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (Main.rand.NextBool(12) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
            {
				bool PlaceVine = false;
				int Test = j;
				while (Test > j - 10) 
                {
					Tile testTile = Framing.GetTileSafely(i, Test);
					if (testTile.BottomSlope) 
                    {
						break;
					}
					else if (!testTile.HasTile || testTile.TileType != ModContent.TileType<SpookyGrassGreen>()) 
                    {
						Test--;
						continue;
					}
					PlaceVine = true;
					break;
				}
				
				if (PlaceVine) 
                {
					tileBelow.TileType = Type;
					tileBelow.HasTile = true;
					WorldGen.SquareTileFrame(i, j + 1, true);
					if (Main.netMode == NetmodeID.Server) 
                    {
						NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
					}
				}
			}
		}

		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
		{
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/Ambient/SpookyVinesGreenGlow");

            Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

			spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.5f);
		}
	}
}