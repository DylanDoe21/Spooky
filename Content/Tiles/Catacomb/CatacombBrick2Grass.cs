using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Dusts;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.Cemetery.Ambient;
using Spooky.Content.Tiles.Cemetery.Furniture;

namespace Spooky.Content.Tiles.Catacomb
{
	public class CatacombBrick2Grass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<CatacombBrick2>();
            Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(43, 89, 49));
            DustType = ModContent.DustType<CemeteryGrassDust>();
		    MinPick = int.MaxValue;
		}

        public override bool CanExplode(int i, int j)
        {
			return false;
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
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<CatacombVines>(), true);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.NextBool(8))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<CatacombWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                }

                //grow mushrooms
                if (Main.rand.NextBool(25))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SporeMushroom>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                }

                //grow giant flowers
                if (Main.rand.NextBool(10))
                {
                    Catacombs.GrowGiantFlower(i, j, ModContent.TileType<BigFlower>());
                }
            }
        }
	}

    public class CatacombBrick2GrassSafe : ModTile
	{
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick2Grass";

        public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<CatacombBrick2>();
            Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(43, 89, 49));
            RegisterItemDrop(ModContent.ItemType<CatacombBrick2Item>());
            DustType = ModContent.DustType<CemeteryGrassDust>();
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
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<CatacombVines>(), true);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.NextBool(8))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<CatacombWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
                }

                //grow mushrooms
                if (Main.rand.NextBool(25))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SporeMushroom>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
                }
            }
        }
    }
}
