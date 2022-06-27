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
	public class SpookyWeedTall1 : ModTile
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
            AddMapEntry(new Color(30, 68, 27));
			DustType = DustID.Grass;
			HitSound = SoundID.Grass;
		}
	}

	public class SpookyWeedTall2 : SpookyWeedTall1
	{
	}

	public class SpookyWeedTall3 : SpookyWeedTall1
	{
	}
}