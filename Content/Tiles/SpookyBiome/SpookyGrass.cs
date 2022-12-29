using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.JungleSpecial[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyDirt>();
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(226, 116, 17));
			ItemDrop = ModContent.ItemType<SpookyDirtItem>();
            DustType = ModContent.DustType<SpookyGrassDust>();
		}

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                if (Main.rand.Next(8) == 0) 
                {
                    Below.TileType = (ushort)ModContent.TileType<SpookyVines>();
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
                //grow small weeds
                if (Main.rand.Next(5) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<SpookyWeedsOrange>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(7) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);

                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
				}

                if (Main.rand.Next(8) == 0) 
                {
                    ushort[] TallWeed = new ushort[] { (ushort)ModContent.TileType<SpookyWeedsTallOrange1>(), 
                    (ushort)ModContent.TileType<SpookyWeedsTallOrange2>(),(ushort)ModContent.TileType<SpookyWeedsTallOrange3>() };

                    ushort newObject = Main.rand.Next(TallWeed);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                if (Main.rand.Next(15) == 0) 
                {
                    ushort[] BigWeeds = new ushort[] { (ushort)ModContent.TileType<SpookyWeedBig1>(), (ushort)ModContent.TileType<SpookyWeedBig2>() };

                    ushort newObject = Main.rand.Next(BigWeeds);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                
                if (Main.rand.Next(35) == 0) 
                {
                    ushort[] Pumpkins = new ushort[] { (ushort)ModContent.TileType<SpookyPumpkin1>(), 
                    (ushort)ModContent.TileType<SpookyPumpkin2>(), (ushort)ModContent.TileType<SpookyPumpkin3>() };

                    ushort newObject = Main.rand.Next(Pumpkins);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacment(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
            }
        }
	}
}
