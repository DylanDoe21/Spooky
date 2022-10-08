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
	public class HangingTongue : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = default(AnchorData); 
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(88, 49, 129));
			DustType = DustID.Blood;
			HitSound = SoundID.NPCHit13;
		}
	}
}