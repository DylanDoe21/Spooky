using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Generation;
using Spooky.Content.Tiles.SpookyBiome.Ambient;

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
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
            Main.tileLighted[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(136, 96, 213));
            RegisterItemDrop(ModContent.ItemType<SpookyStoneItem>());
            DustType = DustID.Stone;
			HitSound = SoundID.Tink;
            MineResist = 0.1f;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<SpookyStone>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<SpookyStone>();
		}

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.35f;
            g = 0.2f;
            b = 0.35f;
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
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<SpookyFungusVines>(), true);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow mushrooms
                if (Main.rand.NextBool(5))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SpookyMushroom>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(4) * 18);
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
                    SpookyForest.GrowGiantMushroom(i, j, 6, 10);
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
