using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
	public class Casket1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(93, 62, 39));
            DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
		}
	}

	public class Casket2 : Casket1
    {
	}

	public class Casket3 : Casket1
    {
	}

	public class Casket4 : Casket1
    {
	}
}