using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Tiles.SpookyHell.Ambient;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class EyeBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMerge[Type][ModContent.TileType<SpookyMush>()] = true;
			Main.tileMerge[Type][ModContent.TileType<Carapace>()] = true;
            Main.tileMerge[Type][TileID.Ash] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(138, 31, 40));
            ItemDrop = ModContent.ItemType<EyeBlockItem>();
			DustType = DustID.Blood;
            HitSound = SoundID.NPCHit20;
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                if (Main.rand.Next(15) == 0) 
                {
                    Below.TileType = (ushort)ModContent.TileType<EyeVine>();
                    Below.HasTile = true;
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
