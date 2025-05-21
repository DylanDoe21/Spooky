using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Tiles.Catacomb
{
	public class CatacombBrick2 : ModTile
	{
		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(87, 52, 37));
			RegisterItemDrop(ModContent.ItemType<CatacombBrick2Item>());
			DustType = DustID.t_Lihzahrd;
			HitSound = SoundID.Tink;
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (WorldGen.genRand.NextBool(7) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
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
	}

	public class CatacombBrick2Arena : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick2";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(87, 52, 37));
			DustType = DustID.t_Lihzahrd;
			HitSound = SoundID.Tink;
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (WorldGen.genRand.NextBool(7) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
            }
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
	}

	public class CatacombBrick2Safe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrick2";

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
			AddMapEntry(new Color(87, 52, 37));
			DustType = DustID.t_Lihzahrd;
			HitSound = SoundID.Tink;
		}

		public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            const int HorizontalFrames = 3; //number of horizontal frames in each row of custom textures
			Tile tile = Main.tile[i, j];

            //Rng variants (done in a checkered pattern so that reframes don't cause a chain reaction)
            if (WorldGen.genRand.NextBool(7) && (i + j) % 2 == 0 && tile.TileFrameY == 18 && tile.TileFrameX >= 18 && tile.TileFrameX < 72)
            {
                Point16 CustomFrameStart = new(18 * 7, 18 * 12); //the frame for where our custom tile textures begin
				int RandomFrame = Main.rand.Next(3); //how many textures there are to choose from total

				tile.TileFrameX = (short)(CustomFrameStart.X + 18 * (RandomFrame % HorizontalFrames));
				tile.TileFrameY = (short)(CustomFrameStart.Y + 18 * (RandomFrame / HorizontalFrames));
            }
        }
    }
}
