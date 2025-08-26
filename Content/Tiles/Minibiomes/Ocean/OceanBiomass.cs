using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
	public class OceanBiomass : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(55, 98, 58));
			RegisterItemDrop(ModContent.ItemType<OceanBiomassItem>());
            DustType = DustID.Blood;
			HitSound = SoundID.NPCDeath1;
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (WorldGen.genRand.NextBool(4) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
            }
        }
	}

	public class OceanBiomassSafe : OceanBiomass
	{
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Ocean/OceanBiomass";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(55, 98, 58));
            DustType = DustID.Blood;
			HitSound = SoundID.NPCDeath1;
		}
	}
}
