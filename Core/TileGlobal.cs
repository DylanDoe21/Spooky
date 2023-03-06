using Terraria;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Pylon;
using Spooky.Content.Tiles.SpookyHell.Furniture;

namespace Spooky.Core
{
    public class TileGlobal : GlobalTile
    {
        public override bool Slope(int i, int j, int type)
        {
            Tile tileAbove = Main.tile[i, j - 1];

            if (tileAbove.TileType == ModContent.TileType<Cauldron>() || 
            tileAbove.TileType == ModContent.TileType<NoseShrine>() ||
            tileAbove.TileType == ModContent.TileType<OrroboroEgg>() ||
            tileAbove.TileType == ModContent.TileType<CemeteryPylon>() ||
            tileAbove.TileType == ModContent.TileType<SpookyBiomePylon>() ||
            tileAbove.TileType == ModContent.TileType<SpookyHellPylon>())
            {
                return false;
            }

            return true;
        }
    }
}