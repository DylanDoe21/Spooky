using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Tiles.SpookyHell.Ambient;

namespace Spooky.Content.Tiles.SpookyHell
{
    public class SpookyMushGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyMush>();
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(164, 29, 22));
            RegisterItemDrop(ModContent.ItemType<SpookyMushItem>());
            DustType = DustID.Blood;
            HitSound = SoundID.Dig;
            MineResist = 0.1f;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<SpookyMush>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<SpookyMush>();
		}

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * 288;
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
            Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            if (!Below.HasTile && Below.LiquidAmount <= 0 && !Tile.BottomSlope)
            {
                //grow vines
                if (Main.rand.NextBool(35)) 
                {
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<EyeVine>(), true);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.NextBool(5))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SpookyHellWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(6) * 18);
                }

                //eye stalks
                if (Main.rand.NextBool(20))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkThinShort>(), (ushort)ModContent.TileType<EyeStalkThin>(), 
                    (ushort)ModContent.TileType<EyeStalkThinTall>(), (ushort)ModContent.TileType<EyeStalkThinVeryTall>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                if (Main.rand.NextBool(25))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkSmall1>(), (ushort)ModContent.TileType<EyeStalkSmall2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                if (Main.rand.NextBool(30))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkMedium1>(), (ushort)ModContent.TileType<EyeStalkMedium2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                if (Main.rand.NextBool(35))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkBig1>(), (ushort)ModContent.TileType<EyeStalkBig2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                if (Main.rand.NextBool(35))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkGiant1>(), (ushort)ModContent.TileType<EyeStalkGiant2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //purple eye stalk
                if (Main.rand.NextBool(35))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkPurple1>(), (ushort)ModContent.TileType<EyeStalkPurple2>(), 
                    (ushort)ModContent.TileType<EyeStalkPurple3>(), (ushort)ModContent.TileType<EyeStalkPurple4>(),
                    (ushort)ModContent.TileType<EyeStalkPurple5>(), (ushort)ModContent.TileType<EyeStalkPurple6>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //ambient manhole teeth
                if (Main.rand.NextBool(35))
                {
                    WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<Tooth>(), true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, (ushort)ModContent.TileType<Tooth>(), 0, 0, -1, -1);
                }

                //exposed nerve
                if (Main.hardMode && Main.rand.NextBool(1000))
                {
                    WorldGen.PlaceObject(i, j - 1, ModContent.TileType<ExposedNerveTile>(), true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<ExposedNerveTile>(), 0, 0, -1, -1);
                }
            }

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<SpookyMush>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<SpookyMushGrass>();

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
