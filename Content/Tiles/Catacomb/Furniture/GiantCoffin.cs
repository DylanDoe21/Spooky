using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.Catacomb;
using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Slingshots.Ammo;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	[LegacyName("GiantCoffin1")]
	[LegacyName("GiantCoffin2")]
	public class GiantCoffin : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
            TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
            TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
            TileObjectData.addAlternate(1);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(122, 81, 70));
			DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
		}
    }
	
	public class GiantCoffinUnsafe : ModTile
	{
		public override string Texture => "Spooky/Content/Tiles/Catacomb/Furniture/GiantCoffin";

		public static readonly SoundStyle BreakSound = new("Spooky/Content/Sounds/WoodBreaking", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(122, 81, 70));
			DustType = DustID.WoodFurniture;
			HitSound = SoundID.Dig;
		}

		public override IEnumerable<Item> GetItemDrops(int i, int j)
		{
			Tile tile = Main.tile[i, j];

			if (Main.rand.NextBool(12))
			{	
				if (tile.WallType == ModContent.WallType<CatacombBrickWall1>() || tile.WallType == ModContent.WallType<CatacombGrassWall1>())
				{
					yield return new Item(ModContent.ItemType<CatacombChestKeyUpper>());
				}
				else if (tile.WallType == ModContent.WallType<CatacombBrickWall2>() || tile.WallType == ModContent.WallType<CatacombGrassWall2>())
				{
					yield return new Item(ModContent.ItemType<CatacombChestKeyLower>());
				}
			}

			int choice = Main.rand.Next(5);
			switch (choice)
			{
				case 0:
				{
					yield return new Item(ModContent.ItemType<RustedBullet>(), Main.rand.Next(25, 76));
					break;
				}
				case 1:
				{
					yield return new Item(ModContent.ItemType<OldWoodArrow>(), Main.rand.Next(25, 76));
					break;
				}
				case 2:
				{
					yield return new Item(ModContent.ItemType<MossyPebble>(), Main.rand.Next(25, 76));
					break;
				}
				case 3:
				{
					yield return new Item(ItemID.HealingPotion, Main.rand.Next(3, 7));
					yield return new Item(ItemID.ManaPotion, Main.rand.Next(3, 7));
					break;
				}
				case 4:
				{
					yield return new Item(ItemID.SpelunkerPotion);
					break;
				}
				case 5:
				{
					if (tile.WallType == ModContent.WallType<CatacombBrickWall1>() || tile.WallType == ModContent.WallType<CatacombGrassWall1>())
					{
						yield return new Item(ModContent.ItemType<CatacombTorch1Item>(), Main.rand.Next(10, 21));
					}
					else if (tile.WallType == ModContent.WallType<CatacombBrickWall2>() || tile.WallType == ModContent.WallType<CatacombGrassWall2>())
					{
						yield return new Item(ModContent.ItemType<CatacombTorch2Item>(), Main.rand.Next(10, 21));
					}
					else
					{
						yield return new Item(ItemID.Torch, Main.rand.Next(10, 21));
					}
					break;
				}
			}
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (!WorldGen.gen)
			{
				SoundEngine.PlaySound(BreakSound, new Vector2((float)(i * 16), (float)(j * 16)));

                for (int numGores = 1; numGores <= 8; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2((float)(i * 16) + Main.rand.Next(0, 16), (float)(j * 16) + Main.rand.Next(0, 45)), default, ModContent.Find<ModGore>("Spooky/GiantCoffinPiece" + Main.rand.Next(1, 3)).Type);
                    }
                }
            }
		}
	}
}