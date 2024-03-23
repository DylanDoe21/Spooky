using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Mushrooms;
using Spooky.Content.Tiles.SpookyBiome.Tree;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class MushroomMoss : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.GeneralPlacementTiles[Type] = false;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyStone>();
            Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(136, 96, 213));
            RegisterItemDrop(ModContent.ItemType<SpookyStoneItem>());
            DustType = DustID.Stone;
			HitSound = SoundID.Tink;
            MineResist = 0.8f;
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.25f;
            g = 0.15f;
            b = 0.25f;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope) 
            {
                if (Main.rand.NextBool(10))
                {
                    Below.TileType = (ushort)ModContent.TileType<SpookyFungusVines>();
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
                //grow mushrooms
                if (Main.rand.NextBool(5))
                {
                    Above.TileType = (ushort)ModContent.TileType<SpookyMushroom>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(Main.rand.Next(4) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);

                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
				}

                //grow big mushrooms
                if (Main.rand.NextBool(18))
                {
                    ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroom1>(), (ushort)ModContent.TileType<GiantShroom2>(),
                    (ushort)ModContent.TileType<GiantShroom3>(), (ushort)ModContent.TileType<GiantShroom4>() };

                    ushort newObject = Main.rand.Next(Shrooms);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //grow big yellow mushrooms
                if (Main.rand.NextBool(30))
                {
                    ushort[] Shrooms = new ushort[] { (ushort)ModContent.TileType<GiantShroomYellow1>(), (ushort)ModContent.TileType<GiantShroomYellow2>(),
                    (ushort)ModContent.TileType<GiantShroomYellow3>(), (ushort)ModContent.TileType<GiantShroomYellow4>() };

                    ushort newObject = Main.rand.Next(Shrooms);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //grow mushroom trees very rarely
                if (Main.rand.NextBool(35))
                {
                    SpookyForest.GrowGiantMushroom(i, j, ModContent.TileType<GiantShroom>(), 6, 10);
                }
            }

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<SpookyStone>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<MushroomMoss>();

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
