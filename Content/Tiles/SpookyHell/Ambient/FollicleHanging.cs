using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
	public class FollicleHanging1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = default(AnchorData); 
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(70, 70, 84));
			DustType = DustID.Ash;
			HitSound = SoundID.NPCHit13;
		}
	}

	public class FollicleHanging2 : FollicleHanging1
	{
	}
}