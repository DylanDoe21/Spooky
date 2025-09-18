using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class GlowshroomChandelier : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.DrawYOffset = -10;
			TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(122, 72, 203), name);
            DustType = DustID.Slush;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Chandeliers };
        }

        public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
		{
			if ((Framing.GetTileSafely(i, j - 1).HasTile && TileID.Sets.Platforms[Framing.GetTileSafely(i, j - 1).TileType]) ||
            (Framing.GetTileSafely(i, j - 2).HasTile && TileID.Sets.Platforms[Framing.GetTileSafely(i, j - 2).TileType]) ||
            (Framing.GetTileSafely(i, j - 3).HasTile && TileID.Sets.Platforms[Framing.GetTileSafely(i, j - 3).TileType]))
			{
				offsetY -= 8;
			}
		}

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<GlowshroomChandelierItem>());
        }

        public override void HitWire(int i, int j)
		{
			Tile tile = Main.tile[i, j];
			int width = 3;
			int height = 3;
			int x = i - tile.TileFrameX / 18 % width;
			int y = j - tile.TileFrameY / 18 % height;

			for (int l = x; l < x + width; l++)
			{
				for (int m = y; m < y + height; m++)
				{
					Tile checkTile = Framing.GetTileSafely(l, m);
					if (checkTile.HasTile && checkTile.TileType == Type)
					{
						if (checkTile.TileFrameX != 108)
						{
							checkTile.TileFrameX += 54;
						}
						if (checkTile.TileFrameX >= 108)
						{
							checkTile.TileFrameX -= 108;
						}
					}

					if (Wiring.running)
					{
						Wiring.SkipWire(l, m);
					}
				}
			}

			int w2 = width / 2;
			int h2 = height / 2;
			NetMessage.SendTileSquare(-1, x + w2 - 1, y + h2, 1 + w2 + h2);
		}

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 36)
            {
                float divide = 250f;

                r = 122f / divide;
                g = 72f / divide;
                b = 203f / divide;
            }
        }
    }

    public class GlowshroomYellowChandelier : GlowshroomChandelier
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.DrawYOffset = -2;
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom | AnchorType.PlanterBox, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.Platform, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.DrawYOffset = -10;
			TileObjectData.addAlternate(0);
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(179, 128, 50), name);
            DustType = DustID.Slush;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            AdjTiles = new int[] { TileID.Chandeliers };
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            yield return new Item(ModContent.ItemType<GlowshroomYellowChandelierItem>());
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 36)
            {
                float divide = 250f;

                r = 204f / divide;
                g = 164f / divide;
                b = 63f / divide;
            }
        }
    }
}