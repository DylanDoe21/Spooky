using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
	public class SpookyWeedsTallOrange1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(175, 102, 36));
            DustType = ModContent.DustType<HalloweenGrassDust>();
			HitSound = SoundID.Grass;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
	}

	public class SpookyWeedsTallOrange2 : SpookyWeedsTallOrange1
	{
	}

	public class SpookyWeedsTallOrange3 : SpookyWeedsTallOrange1
	{
	}

	public class SpookyWeedsTallGreen1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(62, 95, 38));
			DustType = DustID.Grass;
			HitSound = SoundID.Grass;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }
	}

	public class SpookyWeedsTallGreen2 : ModTile
	{
	}

	public class SpookyWeedsTallGreen3 : ModTile
	{
	}
}