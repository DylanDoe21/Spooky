using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
	public class Follicle1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(70, 70, 84));
			DustType = DustID.Ash;
			HitSound = SoundID.Dig;
		}
    }

	public class Follicle2 : Follicle1
	{
	}
}