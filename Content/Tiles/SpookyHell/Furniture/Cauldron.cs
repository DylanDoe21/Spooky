using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpookyHell;
using Spooky.Content.Items.SpookyHell.Sentient;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class Cauldron : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(2, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AnimationFrameHeight = 72;
            AddMapEntry(new Color(0, 128, 0));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override bool CanKillTile(int i, int j, ref bool blockDamaged)
        {
			return false;
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter > 8)
			{
				frameCounter = 0;
				frame++;

				if (frame > 3)
				{
					frame = 0;
				}
			}
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			noBreak = true;
			return true;
		}
    }
}