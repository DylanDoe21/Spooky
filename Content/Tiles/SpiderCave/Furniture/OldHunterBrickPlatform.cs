using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class OldHunterBrickPlatform : ModTile
	{
		public override void SetStaticDefaults() 
        {
			Main.tileFrameImportant[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileSolid[Type] = false;
			Main.tileNoAttach[Type] = true;
			TileID.Sets.Platforms[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 27;
			TileObjectData.newTile.StyleWrapLimit = 27;
			TileObjectData.newTile.DrawYOffset = 0;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(62, 54, 59));
			DustType = DustID.Stone;
			HitSound = SoundID.Tink;
			MinPick = int.MaxValue;
			AdjTiles = new int[] { TileID.Platforms };
		}

		public override void PostSetDefaults() 
		{
			Main.tileNoSunLight[Type] = false;
		}

		public override bool CanReplace(int i, int j, int tileTypeBeingPlaced)
		{
			return false;
		}

		public override bool CanKillTile(int i, int j, ref bool blockDamaged)
		{
			return false;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}

		public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
		{
			return false;
		}
	}
}