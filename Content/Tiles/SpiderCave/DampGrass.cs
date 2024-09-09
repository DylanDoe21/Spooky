using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpiderCave.Tree;

namespace Spooky.Content.Tiles.SpiderCave
{
	public class DampGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<DampSoil>();
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(43, 100, 38));
            RegisterItemDrop(ModContent.ItemType<DampSoilItem>());
            DustType = ModContent.DustType<DampGrassDust>();
			MineResist = 0.65f;
		}

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

			if (!Below.HasTile && Below.LiquidAmount <= 0 && !Tile.BottomSlope) 
            {
                //grow vines
                if (Main.rand.NextBool(15)) 
                {
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<DampVines>(), true);
                }
            }

			if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.NextBool(8))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SpiderCaveWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(16) * 18);
                }

                //mushrooms 
                if (Main.rand.NextBool(18))
                {
                    ushort[] Mushrooms = new ushort[] { (ushort)ModContent.TileType<MushroomBlue1>(), (ushort)ModContent.TileType<MushroomBlue2>(), 
                    (ushort)ModContent.TileType<MushroomGreen1>(), (ushort)ModContent.TileType<MushroomGreen2>(), 
                    (ushort)ModContent.TileType<MushroomRed1>(), (ushort)ModContent.TileType<MushroomRed2>(), 
                    (ushort)ModContent.TileType<MushroomYellow1>(), (ushort)ModContent.TileType<MushroomYellow2>() };

                    ushort newObject = Main.rand.Next(Mushrooms);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //grow tall mushrooms
                if (!Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
                {
                    if (Main.rand.NextBool(10))
                    {
                        Content.Generation.SpiderCave.CanGrowTallMushroom(i, j, ModContent.TileType<TallMushroom>(), 2, 5);
                    }
                }
			}

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<DampSoil>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<DampGrass>();

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tilePoint.X, tilePoint.Y, 1, TileChangeType.None);
                    }
                }
            }
		}

        private List<Point> OpenAdjacents(int i, int j, int type)
        {
            var tileList = new List<Point>();

            for (int k = -1; k < 2; ++k)
            {
                for (int l = -1; l < 2; ++l)
                {
                    if (!(l == 0 && k == 0) && Framing.GetTileSafely(i + k, j + l).HasTile && Framing.GetTileSafely(i + k, j + l).TileType == type)
                    {
                        tileList.Add(new Point(i + k, j + l));
                    }
                }
            }

            return tileList;
        }

        private bool HasOpening(int i, int j)
        {
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    if (!Framing.GetTileSafely(i + k, j + l).HasTile)
                    {
                        return true;
                    }
                }
            }
                    
            return false;
        }
	}
}
