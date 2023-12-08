using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.Cemetery;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyGrass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyDirt>();
            Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(226, 116, 17));
            RegisterItemDrop(ModContent.ItemType<SpookyDirtItem>());
            DustType = ModContent.DustType<SpookyGrassDust>();
            MineResist = 0.7f;
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i - 1, j].TileType == ModContent.TileType<SpookyGrassGreen>() || Main.tile[i + 1, j].TileType == ModContent.TileType<SpookyGrassGreen>() ||
            Main.tile[i, j - 1].TileType == ModContent.TileType<SpookyGrassGreen>() || Main.tile[i, j + 1].TileType == ModContent.TileType<SpookyGrassGreen>())
            {
                Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/SpookyGrassBlend").Value;

                Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

                spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
                new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16), Lighting.GetColor(i, j));
            }
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
                if (Main.rand.Next(15) == 0)
                {
                    Above.TileType = (ushort)ModContent.TileType<SpookyWeedsOrange>();
                    Above.HasTile = true;
                    Above.TileFrameY = 0;
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
                    WorldGen.SquareTileFrame(i, j + 1, true);

                    if (Main.netMode == NetmodeID.Server) 
                    {
                        NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
                    }
                }
                
                if (Main.rand.Next(40) == 0)
                {
                    ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdSmall1>(), (ushort)ModContent.TileType<GourdSmall2>(), 
                    (ushort)ModContent.TileType<GourdMedium1>(), (ushort)ModContent.TileType<GourdMedium2>(),
                    (ushort)ModContent.TileType<GourdLarge1>(), (ushort)ModContent.TileType<GourdLarge2>(), 
                    (ushort)ModContent.TileType<GourdGiant1>(), (ushort)ModContent.TileType<GourdGiant2>() };

                    ushort newObject = Main.rand.Next(Gourds);

                    WorldGen.PlaceObject(i, j - 1, newObject, true);
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
            }

            //spread grass
            List<Point> adjacents = OpenAdjacents(i, j, ModContent.TileType<SpookyDirt>());

            if (adjacents.Count > 0)
            {
                Point tilePoint = adjacents[Main.rand.Next(adjacents.Count)];
                if (HasOpening(tilePoint.X, tilePoint.Y))
                {
                    Framing.GetTileSafely(tilePoint.X, tilePoint.Y).TileType = (ushort)ModContent.TileType<SpookyGrass>();

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

            for (int X = -1; X < 2; X++)
            {
                for (int Y = -1; Y < 2; Y++)
                {
                    if (!(X == 0 && Y == 0) && Framing.GetTileSafely(i + X, j + Y).HasTile && Framing.GetTileSafely(i + X, j + Y).TileType == type)
                    {
                        tileList.Add(new Point(i + X, j + Y));
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
