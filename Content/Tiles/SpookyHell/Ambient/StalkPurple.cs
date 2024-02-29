using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
	[LegacyName("Tentacle1")]
    public class StalkPurple1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(91, 37, 132));
            DustType = ModContent.DustType<SpookyHellPurpleDust>();
            HitSound = SoundID.NPCHit13;
        }

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) 
		{
			if (i % 2 == 1) 
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
    }

	[LegacyName("Tentacle2")]
    public class StalkPurple2 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(91, 37, 132));
            DustType = ModContent.DustType<SpookyHellPurpleDust>();
            HitSound = SoundID.NPCHit13;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) 
		{
			if (i % 2 == 1) 
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
    }

	[LegacyName("Tentacle3")]
	[LegacyName("Tentacle4")]
    public class StalkPurple3 : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.Origin = new Point16(0, 4);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
            AddMapEntry(new Color(91, 37, 132));
            DustType = ModContent.DustType<SpookyHellPurpleDust>();
            HitSound = SoundID.NPCHit13;
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects) 
		{
			if (i % 2 == 1) 
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
    }
}