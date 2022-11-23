using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
	public class FingerHanging1 : ModTile
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
            AddMapEntry(new Color(4, 60, 36));
			DustType = ModContent.DustType<SpookyHellGrassDust>();
			HitSound = SoundID.NPCHit13;
		}
	}

	public class FingerHanging2 : FingerHanging1
	{
	}
}