using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
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
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(199, 7, 49));
            RegisterItemDrop(ModContent.ItemType<SpookyMushItem>());
            DustType = DustID.Blood;
            HitSound = SoundID.Dig;
            MineResist = 0.7f;
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

            if (!Below.HasTile && Below.LiquidType <= 0 && !Tile.BottomSlope)
            {
                //eye vines
                if (Main.rand.Next(35) == 0)
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

            if (!Above.HasTile && Above.LiquidType <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //eye stalks
                if (Main.rand.Next(35) == 0)
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkThin>(), (ushort)ModContent.TileType<EyeStalkThinTall>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                if (Main.rand.NextBool(35))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkSmall1>(), (ushort)ModContent.TileType<EyeStalkSmall2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                if (Main.rand.NextBool(40))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkMedium1>(), (ushort)ModContent.TileType<EyeStalkMedium2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                if (Main.rand.NextBool(45))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkBig1>(), (ushort)ModContent.TileType<EyeStalkBig2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
                if (Main.rand.NextBool(50))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<EyeStalkGiant1>(), (ushort)ModContent.TileType<EyeStalkGiant2>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //purple stalks
                if (Main.rand.NextBool(45))
                {
                    ushort[] Stalks = new ushort[] { (ushort)ModContent.TileType<StalkRed1>(), (ushort)ModContent.TileType<StalkRed2>(), (ushort)ModContent.TileType<StalkRed3>() };

                    ushort newObject = Main.rand.Next(Stalks);

                    WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //ambient manhole teeth
                if (Main.rand.NextBool(35))
                {
                    WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<Tooth>(), true, Main.rand.Next(0, 2));
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
