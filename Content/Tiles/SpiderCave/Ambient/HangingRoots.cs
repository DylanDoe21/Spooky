using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
	public class HangingRoots1 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default(AnchorData);
			TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(84, 61, 42));
			DustType = DustID.Dirt;
			HitSound = SoundID.Dig;
		}
	}

	public class HangingRoots2 : HangingRoots1
	{
	}

	public class HangingRoots3 : HangingRoots1
	{
	}

	public class HangingRoots4 : HangingRoots1
	{
	}
}