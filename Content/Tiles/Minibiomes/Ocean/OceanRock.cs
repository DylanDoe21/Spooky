using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Tiles.Minibiomes.Ocean
{
	public class OceanRock : ModTile
	{
		private static Asset<Texture2D> MergeTexture;

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(132, 93, 52));
			RegisterItemDrop(ModContent.ItemType<OceanRockItem>());
            DustType = DustID.YellowStarfish;
			HitSound = SoundID.Tink;
			MinPick = 110;
		}

		public override bool CanExplode(int i, int j)
		{
			return false;
		}
		
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/OceanRockSandMerge");

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);
			Color color = TileGlobal.GetWallColorWithPaint(i, j, Lighting.GetColor(i, j));

			//down wall merge
			if (Main.tile[i, j + 1].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 0, 0, 16, 16), color);
			}
			//left wall merge
			if (Main.tile[i - 1, j].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 1, 0, 16, 16), color);
			}
			//up wall merge
			if (Main.tile[i, j - 1].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 2, 0, 16, 16), color);
			}
			//right wall merge
			if (Main.tile[i + 1, j].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 3, 0, 16, 16), color);
			}
		}
	}

	public class OceanRockSafe : OceanRock
	{
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Ocean/OceanRock";

		private static Asset<Texture2D> MergeTexture;

		public override void SetStaticDefaults()
		{
			TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
			Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(132, 93, 52));
            DustType = DustID.Stone;
			HitSound = SoundID.Tink;
			MinPick = 110;
		}
		
		public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MergeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Tiles/Minibiomes/Ocean/OceanRockSandMerge");

			Vector2 pos = TileGlobal.TileCustomPosition(i, j);
			Color color = TileGlobal.GetWallColorWithPaint(i, j, Lighting.GetColor(i, j));

			//down wall merge
			if (Main.tile[i, j + 1].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 0, 0, 16, 16), color);
			}
			//left wall merge
			if (Main.tile[i - 1, j].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 1, 0, 16, 16), color);
			}
			//up wall merge
			if (Main.tile[i, j - 1].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 2, 0, 16, 16), color);
			}
			//right wall merge
			if (Main.tile[i + 1, j].TileType == ModContent.TileType<OceanSand>())
			{
				spriteBatch.Draw(MergeTexture.Value, pos, new Rectangle(18 * 3, 0, 16, 16), color);
			}
		}
	}
}
