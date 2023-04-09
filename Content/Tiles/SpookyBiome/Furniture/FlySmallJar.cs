using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class FlySmallJar : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1); 
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AnimationFrameHeight = 36;
			LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(203, 248, 218), name);
			DustType = DustID.Glass;
		}

        public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter > 8)
			{
				frameCounter = 0;
				frame++;

				if (frame >= 6)
				{
					frame = 0;
				}
			}
		}
    }
}