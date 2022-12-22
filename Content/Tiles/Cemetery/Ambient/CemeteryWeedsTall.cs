using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Cemetery.Ambient
{
	public class CemeteryWeedsTall1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileCut[Type] = true;
			Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
			TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(31, 94, 61));
            DustType = ModContent.DustType<CemeteryGrassDust>();
			HitSound = SoundID.Grass;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            if (Main.rand.Next(20) == 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<CemeteryGrassSeeds>());
            }
        }
	}

	public class CemeteryWeedsTall2 : CemeteryWeedsTall1
	{
	}

	public class CemeteryWeedsTall3 : CemeteryWeedsTall1
	{
	}

	public class CemeteryWeedsTall4 : CemeteryWeedsTall1
	{
	}

	public class CemeteryWeedsTall5 : CemeteryWeedsTall1
	{
	}

	public class CemeteryWeedsTall6 : CemeteryWeedsTall1
	{
	}
}