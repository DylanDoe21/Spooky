using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
	[LegacyName("CatacombVines2")]
	public class CatacombVines : ModTile
	{
        private static Asset<Texture2D> GlowTexture;

        public override void SetStaticDefaults()
		{
			Main.tileLighted[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoFail[Type] = true;
			Main.tileNoAttach[Type] = true;
			AddMapEntry(new Color(23, 69, 29));
			DustType = ModContent.DustType<SpookyGrassDustGreen>();
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.4f;
			g = 0.15f;
			b = 0f;
        }

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
			int[] ValidTiles = { ModContent.TileType<CatacombVines>(), ModContent.TileType<CatacombBrick1Grass>(), ModContent.TileType<CatacombBrick1GrassSafe>(),
			ModContent.TileType<CatacombBrick2Grass>(), ModContent.TileType<CatacombBrick2GrassSafe>() };

			if (!ValidTiles.Contains(Main.tile[i, j - 1].TileType))
			{
				WorldGen.KillTile(i, j, false, false, false);
			}

            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }

		public override void RandomUpdate(int i, int j)
		{
			Tile tileBelow = Framing.GetTileSafely(i, j + 1);
			if (Main.rand.NextBool(5) && !tileBelow.HasTile && tileBelow.LiquidType != LiquidID.Lava)
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
					else if (!testTile.HasTile || testTile.TileType != ModContent.TileType<CatacombBrick1Grass>()) 
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
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Catacomb/Ambient/CatacombVinesGlow");

            Tile tile = Framing.GetTileSafely(i, j);
			Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);
			spriteBatch.Draw(GlowTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), Color.White * 0.5f);
		}
	}
}