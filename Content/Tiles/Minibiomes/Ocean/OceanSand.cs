using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Biomes;
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
