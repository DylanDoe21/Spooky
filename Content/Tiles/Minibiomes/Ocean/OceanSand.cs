using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Biomes;
using Spooky.Content.Tiles.Minibiomes.Ocean.Ambient;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
	public class OceanSand : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(222, 192, 102));
			RegisterItemDrop(ModContent.ItemType<OceanSandItem>());
            DustType = DustID.Sand;
			MineResist = 0.65f;
		}

		public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Main.dedServ && Main.LocalPlayer.InModBiome(ModContent.GetInstance<ZombieOceanBiome>()))
			{
                Main.SceneMetrics.ActiveFountainColor = ModContent.GetInstance<ZombieWaterStyle>().Slot;
			}
        }

		public override void RandomUpdate(int i, int j)
        {
            Tile Tile = Framing.GetTileSafely(i, j);
			Tile Below = Framing.GetTileSafely(i, j + 1);
            Tile Above = Framing.GetTileSafely(i, j - 1);

            // Place sunken kelp
            if (WorldGen.genRand.NextBool(3) && !Above.HasTile && Above.LiquidAmount > 0 && !Tile.LeftSlope && !Tile.RightSlope && !Tile.IsHalfBlock)
            {
                WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<OceanKelp>(), true);
                NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
            }

			if (!Below.HasTile && Below.LiquidAmount >= 255 && !Tile.BottomSlope) 
            {
                //grow vines
                if (Main.rand.NextBool(15))
                {
                    WorldGen.PlaceTile(i, j + 1, (ushort)ModContent.TileType<OceanVines>(), true);
                    NetMessage.SendTileSquare(-1, i, j + 1, 1, TileChangeType.None);
                }
            }

            if (!Above.HasTile && Above.LiquidAmount >= 255 && !Tile.BottomSlope && !Tile.TopSlope && !Tile.IsHalfBlock) 
            {
                //grow weeds
                if (Main.rand.NextBool(15))
                {
                    WorldGen.PlaceTile(i, j - 1, (ushort)ModContent.TileType<OceanWeeds>(), true);
                    Above.TileFrameX = (short)(WorldGen.genRand.Next(12) * 18);
					NetMessage.SendTileSquare(-1, i, j - 1, 1, TileChangeType.None);
				}

				//grow light plants
                if (Main.rand.NextBool(20))
                {
                    ushort[] LightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlant1>(), (ushort)ModContent.TileType<LightPlant2>(), (ushort)ModContent.TileType<LightPlant3>() };

                    ushort newObject = Main.rand.Next(LightPlants);

                    WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

				//grow light plants
                if (Main.rand.NextBool(25))
                {
                    ushort[] BigLightPlants = new ushort[] { (ushort)ModContent.TileType<LightPlantBig1>(), (ushort)ModContent.TileType<LightPlantBig2>(), 
					(ushort)ModContent.TileType<LightPlantBig3>(), (ushort)ModContent.TileType<LightPlantBig4>() };

                    ushort newObject = Main.rand.Next(BigLightPlants);

                    WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }

                //grow corals
                if (Main.rand.NextBool(22))
                {
                    ushort[] Corals = new ushort[] { (ushort)ModContent.TileType<CoralGreen1>(), (ushort)ModContent.TileType<CoralGreen2>(), (ushort)ModContent.TileType<CoralGreen3>(),
                    (ushort)ModContent.TileType<CoralPurple1>(), (ushort)ModContent.TileType<CoralPurple2>(), (ushort)ModContent.TileType<CoralPurple3>(),
                    (ushort)ModContent.TileType<CoralRed1>(), (ushort)ModContent.TileType<CoralRed2>(), (ushort)ModContent.TileType<CoralRed3>(),
                    (ushort)ModContent.TileType<CoralYellow1>(), (ushort)ModContent.TileType<CoralYellow2>(), (ushort)ModContent.TileType<CoralYellow3>(),
                    (ushort)ModContent.TileType<TubeCoralBlue1>(), (ushort)ModContent.TileType<TubeCoralBlue2>(), (ushort)ModContent.TileType<TubeCoralBlue3>(),
                    (ushort)ModContent.TileType<TubeCoralLime1>(), (ushort)ModContent.TileType<TubeCoralLime2>(), (ushort)ModContent.TileType<TubeCoralLime3>(),
                    (ushort)ModContent.TileType<TubeCoralPurple1>(), (ushort)ModContent.TileType<TubeCoralPurple2>(), (ushort)ModContent.TileType<TubeCoralPurple3>(),
                    (ushort)ModContent.TileType<TubeCoralTeal1>(), (ushort)ModContent.TileType<TubeCoralTeal2>(), (ushort)ModContent.TileType<TubeCoralTeal3>() };

                    ushort newObject = Main.rand.Next(Corals);

                    WorldGen.PlaceObject(i, j - 1, newObject, true, Main.rand.Next(0, 2));
                    NetMessage.SendObjectPlacement(-1, i, j - 1, newObject, 0, 0, -1, -1);
                }
            }
		}
	}

	public class OceanSandSafe : OceanSand
	{
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Ocean/OceanSand";

		public override void SetStaticDefaults()
		{
			TileID.Sets.CanBeDugByShovel[Type] = true;
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(222, 192, 102));
            DustType = DustID.Sand;
			MineResist = 0.65f;
		}

        public override void NearbyEffects(int i, int j, bool closer)
        {
            if (!Main.dedServ && Main.LocalPlayer.InModBiome(ModContent.GetInstance<ZombieOceanBiome>()))
			{
                Main.SceneMetrics.ActiveFountainColor = ModContent.GetInstance<ZombieWaterStyle>().Slot;
			}
        }
	}
}
