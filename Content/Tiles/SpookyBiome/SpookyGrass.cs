using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Gourds;

namespace Spooky.Content.Tiles.SpookyBiome
{
	public class SpookyGrass : ModTile
	{
        private static Asset<Texture2D> MergeTexture;

		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<SpookyDirt>();
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(226, 116, 17));
            RegisterItemDrop(ModContent.ItemType<SpookyDirtItem>());
            DustType = ModContent.DustType<SpookyGrassDust>();
            MineResist = 0.1f;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<SpookyDirt>();
			}
		}

        public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return tileTypeBeingPlaced != ModContent.TileType<SpookyDirt>();
		}

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            if (Main.tile[i - 1, j].TileType == ModContent.TileType<SpookyGrassGreen>() || Main.tile[i + 1, j].TileType == ModContent.TileType<SpookyGrassGreen>() ||
            Main.tile[i, j - 1].TileType == ModContent.TileType<SpookyGrassGreen>() || Main.tile[i, j + 1].TileType == ModContent.TileType<SpookyGrassGreen>())
            {
                if (!Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
                {
                    MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/SpookyBiome/SpookyGrassBlend");

                    Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange);

                    spriteBatch.Draw(MergeTexture.Value, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, 
                    new Rectangle(Main.tile[i, j].TileFrameX, Main.tile[i, j].TileFrameY, 16, 16), Lighting.GetColor(i, j));
                }
            }
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
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<SpookyVines>(), true);
					NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
				}
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow small weeds
                if (Main.rand.NextBool(15))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SpookyWeedsOrange>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(10) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
				}
                
                //grow colored gourds
                if (Main.rand.NextBool(40) && SpookyForest.CanGrowGourd(i, j))
                {
                    ushort[] Gourds = new ushort[] { (ushort)ModContent.TileType<GourdGreen>(), (ushort)ModContent.TileType<GourdLime>(), 
                    (ushort)ModContent.TileType<GourdLimeOrange>(), (ushort)ModContent.TileType<GourdOrange>(), (ushort)ModContent.TileType<GourdRed>(), 
                    (ushort)ModContent.TileType<GourdWhite>(), (ushort)ModContent.TileType<GourdYellow>(), (ushort)ModContent.TileType<GourdYellowGreen>() };

                    ushort newObject = Main.rand.Next(Gourds);

                    WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //grow rotten gourd
                if (Main.rand.NextBool(55) && SpookyForest.CanGrowRottenGourd(i, j))
                {
                    WorldGen.PlaceObject(i, j - 1, (ushort)ModContent.TileType<GourdRotten>(), true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, (ushort)ModContent.TileType<GourdRotten>(), 0, 0, -1, -1);
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
