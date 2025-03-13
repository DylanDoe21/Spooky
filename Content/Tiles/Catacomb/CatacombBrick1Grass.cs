using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Tiles.Catacomb.Ambient;

namespace Spooky.Content.Tiles.Catacomb
{
	public class CatacombBrick1Grass : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(52, 102, 46));
			RegisterItemDrop(ModContent.ItemType<CatacombBrick1Item>());
			DustType = ModContent.DustType<CemeteryGrassDust>();
			MineResist = 0.1f;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<CatacombBrick1>();
			}
		}

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return Flags.downedDaffodil && tileTypeBeingPlaced != ModContent.TileType<CatacombBrick1Safe>();
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return true;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			MinPick = Flags.downedDaffodil ? 0 : int.MaxValue;
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
            }
        }
	}

	public class CatacombBrick1GrassArena : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick1Grass";

		public override void SetStaticDefaults()
		{
			TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(52, 102, 46));
			DustType = ModContent.DustType<CemeteryGrassDust>();
		}

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return false;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return false;
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
			}
		}
	}

	public class CatacombBrick1GrassSafe : ModTile
	{
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick1Grass";

        public override void SetStaticDefaults()
		{
            TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(52, 102, 46));
            RegisterItemDrop(ModContent.ItemType<CatacombBrick1Item>());
            DustType = ModContent.DustType<CemeteryGrassDust>();
			MineResist = 0.1f;
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<CatacombBrick1Safe>();
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
