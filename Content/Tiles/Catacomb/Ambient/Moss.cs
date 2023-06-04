using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
	public class Moss1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(39, 45, 0));
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

	public class Moss2 : Moss1
	{
	}

	public class Moss3 : Moss1
	{
	}

	public class Moss4 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(39, 45, 0));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }
    }

	public class Moss5 : Moss4
    {
	}
}