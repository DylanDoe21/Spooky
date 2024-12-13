using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
	[LegacyName("CatacombVines2")]
	public class CatacombVines : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = false;
			Main.tileLavaDeath[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileBlockLight[Type] = false;
			Main.tileLighted[Type] = true;
			TileID.Sets.IsVine[Type] = true;
			TileID.Sets.VineThreads[Type] = true;
			TileID.Sets.MultiTileSway[Type] = true;
			AddMapEntry(new Color(23, 69, 29));
			DustType = ModContent.DustType<CemeteryGrassDust>();
			HitSound = SoundID.Grass;
			MineResist = 0.1f;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			if (Main.tile[i, j].TileFrameX % 18 == 0 && Main.tile[i, j].TileFrameY % 54 == 0)
			{
				Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);
			}

			return false;
		}

		public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
		{
			r = 0.4f;
			g = 0.32f;
			b = 0f;
        }

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
			int[] ValidTiles = { ModContent.TileType<CatacombVines>(), ModContent.TileType<CatacombBrick1Grass>(), ModContent.TileType<CatacombBrick1GrassSafe>(),
			ModContent.TileType<CatacombBrick2Grass>(), ModContent.TileType<CatacombBrick2GrassSafe>(),
			ModContent.TileType<CatacombBrick1GrassArena>(), ModContent.TileType<CatacombBrick2GrassArena>() };

			if (!ValidTiles.Contains(Main.tile[i, j - 1].TileType))
			{
				WorldGen.KillTile(i, j, false, false, false);
			}

            return base.TileFrame(i, j, ref resetFrame, ref noBreak);
        }

		public override void RandomUpdate(int i, int j)
		{
			int[] ValidTiles = { ModContent.TileType<CatacombBrick1Grass>(), ModContent.TileType<CatacombBrick1GrassSafe>(),
			ModContent.TileType<CatacombBrick2Grass>(), ModContent.TileType<CatacombBrick2GrassSafe>(),
			ModContent.TileType<CatacombBrick1GrassArena>(), ModContent.TileType<CatacombBrick2GrassArena>() };

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
					else if (!testTile.HasTile || !ValidTiles.Contains(testTile.TileType)) 
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
	}
}