using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Generation;
using Spooky.Content.Tiles.Minibiomes.Vegetable.Ambient;
using Spooky.Content.Tiles.Minibiomes.Vegetable.Tree;

namespace Spooky.Content.Tiles.Minibiomes.Vegetable
{
	public class JungleMoss : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(82, 165, 76));
            DustType = DustID.Grass;
			HitSound = SoundID.Grass;
			MineResist = 0.65f;
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (Main.rand.NextBool(3) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
            }
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
                    NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
				}

                //radish
				if (Main.rand.NextBool(35))
                {
                    WorldGen.PlaceObject(i, j + 1, (ushort)ModContent.TileType<RadishHanging>(), true, WorldGen.genRand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j + 1, (ushort)ModContent.TileType<RadishHanging>(), 0, 0, -1, -1);
                }
            }

			if (!Above.HasTile && Above.TileType != ModContent.TileType<JungleCabbageBoulder>() && Above2.TileType != ModContent.TileType<JungleCabbageBoulder>() &&
			Above3.TileType != ModContent.TileType<JungleCabbageBoulder>() && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock)
			{
				//grow weeds
				if (Main.rand.NextBool(3))
                {
					WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<JungleMossWeeds>(), true);
					Above.TileFrameX = (short)(Main.rand.Next(11) * 18);
                    NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
				}

                //grow broccoli trees
                if (Main.rand.NextBool(50) && VegetableGarden.CanPlaceBroccoli(i, j) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
                {
                    Broccoli.Grow(i, j - 1, 5, 9);
                }

                //cabbage boulders
                if (Main.rand.NextBool(30) && VegetableGarden.CanPlaceCabbageBoulder(i, j))
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

					WorldGen.PlaceObject(i, j - 1, newObject, true);
					NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
				}

				//garlic
				if (Main.rand.NextBool(20))
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

                //radish
				if (Main.rand.NextBool(35))
                {
                    ushort[] Radishes = new ushort[] { (ushort)ModContent.TileType<Radish1>(), (ushort)ModContent.TileType<Radish2>() };

                    ushort newObject = Main.rand.Next(Radishes);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
            }
		}
	}
}
