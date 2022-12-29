using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpookyHell.Ambient;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class SpookyMushGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.JungleSpecial[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyMush>();
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(199, 7, 49));
            ItemDrop = ModContent.ItemType<SpookyMushItem>();
            DustType = ModContent.DustType<SpookyHellGrassDust>();
            HitSound = SoundID.Dig;
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
                    Below.TileType = (ushort)ModContent.TileType<TendrilVine>();
                    Below.HasTile = true;
                    WorldGen.SquareTileFrame(i, j + 1, true);
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, i, j + 1, 3, TileChangeType.None);
                    }
                }
            }

            if (!Above.HasTile && Above.LiquidType <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //exposed nerve
                if (Main.rand.Next(100) == 0)
                {
                    WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ExposedNerveTile>(), true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, ModContent.TileType<ExposedNerveTile>(), 0, 0, -1, -1);
                }
            }
        }
    }
}
