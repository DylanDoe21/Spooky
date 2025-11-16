using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
	public class DampStalactite : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = default(AnchorData);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(50, 46, 43));
			DustType = DustID.Stone;
			HitSound = SoundID.Dig;
		}
	}
}