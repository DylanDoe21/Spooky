using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Generation;
using Spooky.Content.Tiles.Catacomb.Ambient;

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
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(52, 102, 46));
			RegisterItemDrop(ModContent.ItemType<CatacombBrick2Item>());
			DustType = ModContent.DustType<CemeteryGrassDust>();
			MineResist = 0.1f;
		}

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<CatacombBrick2>();
			}
		}

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return Flags.downedBigBone && tileTypeBeingPlaced != ModContent.TileType<CatacombBrick2Safe>();
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return true;
		}

		public override void NearbyEffects(int i, int j, bool closer)
		{
			MinPick = Flags.downedBigBone ? 0 : int.MaxValue;
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
					NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.NextBool(8))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<CatacombWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }

                //grow mushrooms
                if (Main.rand.NextBool(25))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SporeMushroom>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }

                //grow giant flowers
				if (Main.rand.NextBool(10) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
				{
					Catacombs.GrowGiantFlower(i, j, ModContent.TileType<BigFlower>());
				}
            }
        }
	}

	public class CatacombBrick2GrassArena : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick2Grass";

		public override void SetStaticDefaults()
		{
			TileID.Sets.Grass[Type] = true;
			TileID.Sets.NeedsGrassFraming[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			TileID.Sets.NeedsGrassFramingDirt[Type] = ModContent.TileType<CatacombBrick2>();
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
					NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.NextBool(8))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<CatacombWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }

                //grow mushrooms
                if (Main.rand.NextBool(25))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SporeMushroom>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }

                //grow giant flowers
                if (Main.rand.NextBool(10) && !Main.tile[i, j].LeftSlope && !Main.tile[i, j].RightSlope && !Main.tile[i, j].IsHalfBlock)
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
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(52, 102, 46));
            RegisterItemDrop(ModContent.ItemType<CatacombBrick2Item>());
            DustType = ModContent.DustType<CemeteryGrassDust>();
			MineResist = 0.1f;
		}

		public override bool CanExplode(int i, int j)
		{
			WorldGen.KillTile(i, j, false, false, true); //Makes the tile completely go away instead of reverting to dirt
			return true;
		}

		public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (!fail && !WorldGen.gen)
			{
				fail = true;
				Framing.GetTileSafely(i, j).TileType = (ushort)ModContent.TileType<CatacombBrick2Safe>();
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
					NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount <= 0 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.NextBool(8))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<CatacombWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(18) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }

                //grow mushrooms
                if (Main.rand.NextBool(25))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<SporeMushroom>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(8) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
                }
            }
        }
    }
}
