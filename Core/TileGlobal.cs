using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Achievements;
using Spooky.Content.Dusts;
using Spooky.Content.Gores.Misc;
using Spooky.Content.Items.Minibiomes.Desert;
using Spooky.Content.NPCs.Minibiomes.Desert.Projectiles;
using Spooky.Content.Tiles.Blooms;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.NoseTemple.Furniture;
using Spooky.Content.Tiles.Pylon;
using Spooky.Content.Tiles.SpiderCave;
using Spooky.Content.Tiles.SpookyHell.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Core
{
    public class TileGlobal : GlobalTile
    {
		public static List<ushort> Blooms = new()
		{
			(ushort)ModContent.TileType<CemeteryBloomPlant>(),
			(ushort)ModContent.TileType<DandelionBloomPlant>(),
			(ushort)ModContent.TileType<DragonfruitBloomPlant>(),
            (ushort)ModContent.TileType<FallBloomPlant>(),
            (ushort)ModContent.TileType<FossilBloomPlant>(),
            (ushort)ModContent.TileType<FungusBloomPlant>(),
            (ushort)ModContent.TileType<SeaBloomPlant>(),
            (ushort)ModContent.TileType<SpringBloomPlant>(),
            (ushort)ModContent.TileType<SummerBloomPlant>(),
            (ushort)ModContent.TileType<VegetableBloomPlant>(),
            (ushort)ModContent.TileType<WinterBloomPlant>(),
		};

		public static Vector2 TileOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		public static Vector2 TileCustomPosition(int i, int j, Vector2 off = default) => (new Vector2(i, j) * 16) - Main.screenPosition - off + TileOffset;

		public override void Load()
		{
			On_ShimmerTransforms.IsItemTransformLocked += CustomShimmerLockConditions;
		}

		public override void PlaceInWorld(int i, int j, int type, Item item)
		{
			if (Blooms.Contains((ushort)type))
			{
				if (type == Blooms[0])
				{
					Flags.BloomsPlanted[0] = true;
				}
				if (type == Blooms[1])
				{
					Flags.BloomsPlanted[1] = true;
				}
				if (type == Blooms[2])
				{
					Flags.BloomsPlanted[2] = true;
				}
				if (type == Blooms[3])
				{
					Flags.BloomsPlanted[3] = true;
				}
				if (type == Blooms[4])
				{
					Flags.BloomsPlanted[4] = true;
				}
				if (type == Blooms[5])
				{
					Flags.BloomsPlanted[5] = true;
				}
				if (type == Blooms[6])
				{
					Flags.BloomsPlanted[6] = true;
				}
				if (type == Blooms[7])
				{
					Flags.BloomsPlanted[7] = true;
				}
				if (type == Blooms[8])
				{
					Flags.BloomsPlanted[8] = true;
				}
				if (type == Blooms[9])
				{
					Flags.BloomsPlanted[9] = true;
				}
				if (type == Blooms[10])
				{
					Flags.BloomsPlanted[10] = true;
				}

				bool CanGetAchievement = true;
				foreach (bool var in Flags.BloomsPlanted)
				{
					if (var == false)
					{
						CanGetAchievement = false;
						break;
					}
				}

				if (CanGetAchievement)
				{
					ModContent.GetInstance<MiscAchievementBloomSeeds>().BloomSeedsCondition.Complete();
				}
			}
		}

		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			if (Blooms.Contains(Main.tile[i, j].TileType) && Main.tile[i, j].TileFrameX >= 216)
			{
				ModContent.GetInstance<MiscAchievementBloomHarvest>().BloomHarvestCondition.Complete();
			}

			if (Main.tile[i, j].TileType == TileID.Stone && Main.rand.NextBool(40) && ItemGlobal.ActiveItem(Main.LocalPlayer).type == ModContent.ItemType<GoldrushPickaxe>())
			{
				int[] Items = new int[] { ItemID.CopperOre, ItemID.TinOre, ItemID.IronOre, ItemID.LeadOre, ItemID.SilverOre, ItemID.TungstenOre, ItemID.GoldOre, ItemID.PlatinumOre,
				ItemID.Amethyst, ItemID.Topaz, ItemID.Sapphire, ItemID.Ruby, ItemID.Amber, ItemID.Diamond };

				int OreDrop = Item.NewItem(new EntitySource_TileBreak(i, j), (new Vector2(i, j) * 16), Main.rand.Next(Items), 1);

				if (Main.netMode == NetmodeID.MultiplayerClient && OreDrop >= 0)
				{
					NetMessage.SendData(MessageID.SyncItem, -1, -1, null, OreDrop, 1f);
				}
			}
		}

		public override bool Slope(int i, int j, int type)
        {
            Tile tileAbove = Main.tile[i, j - 1];

            //dont allow sloping under specific spooky mod tiles
            if (tileAbove.TileType == ModContent.TileType<Cauldron>() || tileAbove.TileType == ModContent.TileType<NoseShrine>() || tileAbove.TileType == ModContent.TileType<MocoIdolPedestal>() ||
            tileAbove.TileType == ModContent.TileType<CemeteryPylon>() || tileAbove.TileType == ModContent.TileType<SpiderCavePylon>() || 
			tileAbove.TileType == ModContent.TileType<SpookyBiomePylon>() || tileAbove.TileType == ModContent.TileType<SpookyHellPylon>())
            {
                return false;
            }

            return base.Slope(i, j, type);
        }

		public override void NearbyEffects(int i, int j, int type, bool closer)
		{
			//tree stuff
			if (!Main.gamePaused && Main.instance.IsActive && closer)
			{
				Tile tile = Framing.GetTileSafely(i, j);

				//spawn autumn leaves from spider grotto trees
				if (Main.rand.NextBool(50))
				{
					if (tile.TileType == TileID.Trees && GetTileTreeIsOn(i, j) == ModContent.TileType<DampGrass>() && tile.TileFrameX >= 22 && tile.TileFrameX < 44 && tile.TileFrameY >= 198)
					{
						int[] Leaves = new int[] { ModContent.GoreType<LeafGrottoRed>(), ModContent.GoreType<LeafGrottoOrange>(), ModContent.GoreType<LeafGrottoYellow>() };

						int LeafGore = Gore.NewGore(null, new Vector2((i * 16) + Main.rand.Next(-50, 55), (j * 16) + Main.rand.Next(-75, -25)), Vector2.Zero, Main.rand.Next(Leaves), 1f);
						Main.gore[LeafGore].velocity.X = Main.rand.NextFloat(-3.5f, 3.5f);
						Main.gore[LeafGore].velocity.Y = Main.rand.NextFloat(0.05f, 1.2f);
					}
				}

				//grotto mushroom trees produce red light at the top
				if (tile.TileType == TileID.Trees && GetTileTreeIsOn(i, j) == ModContent.TileType<DampMushroomGrass>() && tile.TileFrameX >= 22 && tile.TileFrameY >= 198)
				{
					float divide = 300f;

					float r = 255f / divide;
					float g = 111f / divide;
					float b = 111f / divide;

					Lighting.AddLight(new Vector2(i, j) * 16, new Vector3(r, g, b));
				}
			}
		}

		public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			//create bubbles in water while in the tar pits biome
			if (Main.waterStyle == ModContent.GetInstance<TarWaterStyle>().Slot)
			{
				if (Main.rand.NextBool(750) && !Main.gamePaused && Main.instance.IsActive)
				{
					if (Main.tile[i, j - 1] != null)
					{
						if (!Main.tile[i, j - 1].HasTile && !Main.tile[i, j - 2].HasTile && !Main.tile[i, j - 3].HasTile && !Main.tile[i, j - 4].HasTile)
						{
							int YPos = j - 1;
							bool ShouldSpawnBubble = false;

							for (int yCheck = YPos; yCheck > 0; yCheck--)
							{
								if (Main.tile[i, yCheck].LiquidAmount > 0 && (Main.tile[i, yCheck - 1].HasTile || Main.tile[i, yCheck - 2].HasTile))
								{
									break;
								}

								if (Main.tile[i, yCheck].LiquidAmount >= 255 && Main.tile[i, yCheck - 1].LiquidAmount > 0 && Main.tile[i, yCheck - 2].LiquidAmount <= 0 &&
								!Main.tile[i - 1, yCheck].HasTile && !Main.tile[i - 1, yCheck - 1].HasTile && !Main.tile[i + 1, yCheck].HasTile && !Main.tile[i + 1, yCheck - 1].HasTile)
								{
									YPos = yCheck + 1;
									ShouldSpawnBubble = true;
									break;
								}
							}

							if (ShouldSpawnBubble && Main.netMode != NetmodeID.MultiplayerClient)
							{
								Projectile.NewProjectile(new EntitySource_WorldEvent(), (float)(i * 16), (float)(YPos * 16 - 20), 0, 0, ModContent.ProjectileType<TarBubble>(), 0, 0f);
							}
						}
					}
				}
			}
		}

		public int GetTileTreeIsOn(int i, int j)
		{
			int x = i;
			int y = j;

			if (Framing.GetTileSafely(x - 1, y).TileType == TileID.Trees && Framing.GetTileSafely(x, y + 1).TileType != TileID.Trees && Framing.GetTileSafely(x, y - 1).TileType != TileID.Trees)
			{
				x--;
			}
			if (Framing.GetTileSafely(x + 1, y).TileType == TileID.Trees && Framing.GetTileSafely(x, y + 1).TileType != TileID.Trees && Framing.GetTileSafely(x, y - 1).TileType != TileID.Trees)
			{
				x++;
			}

			while (WorldGen.InWorld(x, y) && Framing.GetTileSafely(x, y).TileType == TileID.Trees && Framing.GetTileSafely(x, y).HasTile)
			{
   				y++;
			}

			if (!Framing.GetTileSafely(x, y).HasTile)
			{
				return -1;
			}

			return Framing.GetTileSafely(x, y).TileType;
		}

		//use for items in spooky mod that should be locked from shimmer with custom conditions, return true if it should be locked
		//note to self so i dont forget: type refers to the item being thrown into the shimmer, not the item that results from the transformation
		private bool CustomShimmerLockConditions(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
		{
			//dont allow catacomb brick walls to be shimmered into their unsafe variants if that respective layers boss hasnt been defeated yet
			if (type == ModContent.ItemType<CatacombBrickWall1Item>() && !Flags.downedDaffodil)
			{
				return true;
			}
			if (type == ModContent.ItemType<CatacombBrickWall2Item>() && !Flags.downedBigBone)
			{
				return true;
			}

			//orig MUST be returned by default in order for vanillas own shimmer locking conditions to apply
			return orig(type);
		}

		public static Color GetTileColorWithPaint(int i, int j, Color color)
        {
			int colType = Main.tile[i, j].TileColor;
			Color paintCol = WorldGen.paintColor(colType);
			if (colType < 13)
			{
				paintCol.R = (byte)((paintCol.R / 2f) + 128);
				paintCol.G = (byte)((paintCol.G / 2f) + 128);
				paintCol.B = (byte)((paintCol.B / 2f) + 128);
				paintCol.A = (byte)((paintCol.A / 2f) + 128);
			}
			if (colType == 29)
			{
				paintCol = Color.Black;
			}
			color.R = (byte)(paintCol.R / 255f * color.R);
			color.G = (byte)(paintCol.G / 255f * color.G);
			color.B = (byte)(paintCol.B / 255f * color.B);
			color.A = (byte)(paintCol.A / 255f * color.A);
			return color;
        }

		public static Color GetWallColorWithPaint(int i, int j, Color color)
        {
            int colType = Main.tile[i, j].WallColor;
            Color paintCol = WorldGen.paintColor(colType);
            if (colType < 13)
            {
                paintCol.R = (byte)((paintCol.R / 2f) + 128);
                paintCol.G = (byte)((paintCol.G / 2f) + 128);
                paintCol.B = (byte)((paintCol.B / 2f) + 128);
				paintCol.A = (byte)((paintCol.A / 2f) + 128);
			}
            if (colType == 29)
            {
                paintCol = Color.Black;
            }
            color.R = (byte)(paintCol.R / 255f * color.R);
            color.G = (byte)(paintCol.G / 255f * color.G);
            color.B = (byte)(paintCol.B / 255f * color.B);
			color.A = (byte)(paintCol.A / 255f * color.A);
			return color;
        }

		//custom copied version of vanilla SolidCollision but with a list of specific tiles
		public static bool SolidCollisionWithSpecificTiles(Vector2 Position, int Width, int Height, int[] TileTypes)
		{
			int value = (int)(Position.X / 16f) - 1;
			int value2 = (int)((Position.X + (float)Width) / 16f) + 2;
			int value3 = (int)(Position.Y / 16f) - 1;
			int value4 = (int)((Position.Y + (float)Height) / 16f) + 2;
			int num = Utils.Clamp(value, 0, Main.maxTilesX - 1);
			value2 = Utils.Clamp(value2, 0, Main.maxTilesX - 1);
			value3 = Utils.Clamp(value3, 0, Main.maxTilesY - 1);
			value4 = Utils.Clamp(value4, 0, Main.maxTilesY - 1);
			Vector2 vector = default(Vector2);
			for (int i = num; i < value2; i++)
			{
				for (int j = value3; j < value4; j++)
				{
					Tile tile = Main.tile[i, j];
					if (tile == null || !tile.HasTile || !TileTypes.Contains(tile.TileType))
					{
						continue;
					}
					
					bool IsTileValid = Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType];

					if (IsTileValid)
					{
						vector.X = i * 16;
						vector.Y = j * 16;
						int num2 = 16;
						if (tile.IsHalfBlock)
						{
							vector.Y += 8f;
							num2 -= 8;
						}
						if (Position.X + (float)Width > vector.X && Position.X < vector.X + 16f && Position.Y + (float)Height > vector.Y && Position.Y < vector.Y + (float)num2)
						{
							return true;
						}
					}
				}
			}
			
			return false;
		}
    }
}