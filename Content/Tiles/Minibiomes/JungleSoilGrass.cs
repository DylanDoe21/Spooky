using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Generation.Minibiomes;
using Spooky.Content.Tiles.Minibiomes.Ambient;
using Spooky.Content.Tiles.Minibiomes.Tree;
using Spooky.Content.Tiles.SpookyHell.Ambient;

namespace Spooky.Content.Tiles.Minibiomes
{
	public class JungleSoilGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<JungleSoil>();
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(82, 165, 76));
            RegisterItemDrop(ModContent.ItemType<JungleSoilItem>());
            DustType = DustID.Grass;
            MineResist = 0.7f;
		}

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);
			Tile Above2 = Framing.GetTileSafely(i - 1, j - 1);
			Tile Above3 = Framing.GetTileSafely(i + 1, j - 1);

			if (!Below.HasTile && Below.LiquidAmount <= 0 && !Tile.BottomSlope) 
            {
                //grow vines
                if (Main.rand.NextBool(5)) 
                {
					WorldGen.PlaceObject(i, j + 1, (ushort)ModContent.TileType<JungleVines>(), true);
				}
            }

			if (!Above.HasTile && Above.TileType != ModContent.TileType<JungleCabbageBoulder>() && Above2.TileType != ModContent.TileType<JungleCabbageBoulder>() &&
			Above3.TileType != ModContent.TileType<JungleCabbageBoulder>() && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock)
			{
				//grow weeds
				if (Main.rand.NextBool(3))
                {
					WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<JungleMossWeeds>(), true);
					Above.TileFrameX = (short)(WorldGen.genRand.Next(11) * 18);
				}

				//grow broccoli trees
				if (Main.rand.NextBool(50) && VegetableGarden.CanPlaceBroccoli(i, j) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
				{
					Broccoli.Grow(i, j - 1, 5, 9);
				}

				//cabbage boulders
				if (WorldGen.genRand.NextBool(30) && VegetableGarden.CanPlaceCabbageBoulder(i, j))
				{
					ushort newObject = (ushort)ModContent.TileType<JungleCabbageBoulder>();

					WorldGen.PlaceObject(i, j - 1, newObject, true);
					NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
				}

				//misc plants
				if (Main.rand.NextBool(15))
				{
					ushort[] LeafyPlants = new ushort[] { (ushort)ModContent.TileType<JunglePlant1>(), (ushort)ModContent.TileType<JunglePlant2>(), (ushort)ModContent.TileType<JunglePlant3>(),
					(ushort)ModContent.TileType<JunglePlant4>(), (ushort)ModContent.TileType<JunglePlant5>(), (ushort)ModContent.TileType<JunglePlant6>() };

					ushort newObject = Main.rand.Next(LeafyPlants);

					WorldGen.PlaceObject(i, j - 1, newObject, true);
					NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
				}

				//carrots
				if (Main.rand.NextBool(20))
				{
					ushort[] Carrots = new ushort[] { (ushort)ModContent.TileType<Carrot1>(), (ushort)ModContent.TileType<Carrot2>(), (ushort)ModContent.TileType<Carrot3>() };

					ushort newObject = Main.rand.Next(Carrots);

					WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
					NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
				}

				//corns
				if (Main.rand.NextBool(20))
				{
					ushort[] Corns = new ushort[] { (ushort)ModContent.TileType<Corn1>(), (ushort)ModContent.TileType<Corn2>() };

					ushort newObject = Main.rand.Next(Corns);

					WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
					NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
				}

				//garlic
				if (WorldGen.genRand.NextBool(20))
				{
					ushort newObject = (ushort)ModContent.TileType<Garlic>();

					WorldGen.PlaceObject(i, j - 1, newObject, true);
					NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
				}

				//potatos
				if (Main.rand.NextBool(20))
				{
					ushort[] Potatos = new ushort[] { (ushort)ModContent.TileType<Potato1>(), (ushort)ModContent.TileType<Potato2>(), (ushort)ModContent.TileType<Potato3>(), (ushort)ModContent.TileType<Potato4>() };

					ushort newObject = Main.rand.Next(Potatos);

					WorldGen.PlaceObject(i, j - 1, newObject, true);
					NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
				}
			}

			//spread grass
			List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<JungleSoil>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<JungleSoilGrass>();

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