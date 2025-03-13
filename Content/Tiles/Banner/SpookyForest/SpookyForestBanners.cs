/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.NPCs.SpookyBiome;

namespace Spooky.Content.Tiles.Banner.SpookyForest
{
	public class SpookyForestBanners : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 111;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(13, 88, 130), name);
			DustType = -1;
		}

        public override bool CanDrop(int i, int j)
        {
			return false;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			int style = frameX / 18;
			int item;
			switch (style)
			{
				case 0:
					item = ModContent.ItemType<PuttySlime1Banner>();
					break;
				case 1:
					item = ModContent.ItemType<PuttySlime2Banner>();
					break;
				case 2:
					item = ModContent.ItemType<PuttySlime3Banner>();
					break;
                case 3:
                    item = ModContent.ItemType<ZomboidBanner>();
                    break;
                case 4:
                    item = ModContent.ItemType<ZomboidWarlockBanner>();
                    break;
				case 5:
                    item = ModContent.ItemType<FluffBatSmallBanner>();
                    break;
				case 6:
                    item = ModContent.ItemType<FluffBatGiantBanner>();
                    break;
				case 7:
                    item = ModContent.ItemType<SpookyDanceBanner>();
                    break;
				case 8:
                    item = ModContent.ItemType<TinyGhostBanner>();
                    break;
				case 9:
                    item = ModContent.ItemType<TinyGhostBoofBanner>();
                    break;
				case 10:
                    item = ModContent.ItemType<TinyGhostRareBanner>();
                    break;
				case 11:
                    item = ModContent.ItemType<TinyMushroomBanner>();
                    break;
				case 12:
                    item = ModContent.ItemType<FlyBigBanner>();
                    break;
				case 13:
                    item = ModContent.ItemType<FlySmallBanner>();
                    break;
				case 14:
                    item = ModContent.ItemType<LittleSpiderBanner>();
                    break;
				case 15:
                    item = ModContent.ItemType<PuttyPumpkinBanner>();
                    break;
				case 16:
                    item = ModContent.ItemType<CandleMonsterBanner>();
                    break;
				case 17:
                    item = ModContent.ItemType<WitchBanner>();
                    break;
				default:
					return;
			}

            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 16, 48, item);
        }

		public override void NearbyEffects(int i, int j, bool closer)
		{
			if (closer)
			{
				Player player = Main.LocalPlayer;
				int style = Main.tile[i, j].TileFrameX / 18;
				int type = 0;
				int type2 = 0;
				int type3 = 0;
				int type4 = 0;
				int type5 = 0;
				switch (style)
				{
					case 0:
						type = ModContent.NPCType<PuttySlime1>();
						break;
					case 1:
						type = ModContent.NPCType<PuttySlime2>();
						break;
					case 2:
						type = ModContent.NPCType<PuttySlime3>();
						break;
					case 3:
						type = ModContent.NPCType<ZomboidThorn>();
						break;
					case 4:
						type = ModContent.NPCType<ZomboidWarlock>();
						break;
					case 5:
						type = ModContent.NPCType<FluffBatSmall1>();
						break;
					case 6:
						type = ModContent.NPCType<FluffBatBig1>();
						break;
					case 7:
						type = ModContent.NPCType<SpookyDance>();
						break;
					case 8:
						type = ModContent.NPCType<TinyGhost1>();
						break;
					case 9:
						type = ModContent.NPCType<TinyGhostBoof>();
						break;
					case 10:
						type = ModContent.NPCType<TinyGhostRare>();
						break;
					case 11:
						//type = ModContent.NPCType<TinyMushroom>();
						break;
					case 12:
						type = ModContent.NPCType<FlyBig>();
						break;
					case 13:
						type = ModContent.NPCType<FlySmall>();
						break;
					case 14:
						type = ModContent.NPCType<LittleSpider>();
						break;
					case 15:
						//type = ModContent.NPCType<PuttyPumpkin>();
						break;
					case 16:
						//type = ModContent.NPCType<CandleMonster>();
						break;
					case 17:
						//type = ModContent.NPCType<Witch>();
						break;
					default:
						return;
				}

				/TODO: implement separate variables for multiple npc types

				Main.SceneMetrics.NPCBannerBuff[type] = true;
				Main.SceneMetrics.hasBanner = true;
			}
		}

		public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
		{
			if (i % 3 == 1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
		}
	}
}
*/