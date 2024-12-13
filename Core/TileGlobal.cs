using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.NPCs.Minibiomes.TarPits.Projectiles;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.NoseTemple.Furniture;
using Spooky.Content.Tiles.Pylon;
using Spooky.Content.Tiles.SpookyHell.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Core
{
    public class TileGlobal : GlobalTile
    {
		public static Vector2 TileOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		public static Vector2 TileCustomPosition(int i, int j, Vector2 off = default) => (new Vector2(i, j) * 16) - Main.screenPosition - off + TileOffset;

		public override void Load()
		{
			On_ShimmerTransforms.IsItemTransformLocked += CustomShimmerLockConditions;
		}

		public override bool Slope(int i, int j, int type)
        {
            Tile tileAbove = Main.tile[i, j - 1];

            //dont allow sloping under specific spooky mod tiles
            if (tileAbove.TileType == ModContent.TileType<Cauldron>() || tileAbove.TileType == ModContent.TileType<NoseShrine>() || tileAbove.TileType == ModContent.TileType<MocoIdolPedestal>() ||
            tileAbove.TileType == ModContent.TileType<CemeteryPylon>() || tileAbove.TileType == ModContent.TileType<SpookyBiomePylon>() || tileAbove.TileType == ModContent.TileType<SpookyHellPylon>())
            {
                return false;
            }

            return base.Slope(i, j, type);
        }

		public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
		{
			//create bubbles while in the tar pits biome
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

		//use for items in spooky mod that should be locked from shimmer with custom conditions, return true if it should be locked
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

			return orig(type);
		}

		public bool IsProtected(int x, int y)
        {
            if (!Main.gameMenu || Main.dedServ)
            {
                Tile tile = Framing.GetTileSafely(x, y);

                if (tile.WallType == ModContent.WallType<CatacombBrickWall1>() || tile.WallType == ModContent.WallType<CatacombBrickWall2>() ||
                tile.WallType == ModContent.WallType<CatacombGrassWall1>() || tile.WallType == ModContent.WallType<CatacombGrassWall2>())
                {
                    return true;
                }
            }

            return false;
        }

        private Vector2 DontAllowTeleportation(On_Player.orig_CheckForGoodTeleportationSpot orig, Player self, ref bool canSpawn, int teleportStartX, int teleportRangeX, int teleportStartY, int teleportRangeY, Player.RandomTeleportationAttemptSettings settings)
        {
            Vector2 result = orig(self, ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);

            if (IsProtected((int)result.X, (int)result.Y))
            {
                settings.attemptsBeforeGivingUp--;
                result = self.CheckForGoodTeleportationSpot(ref canSpawn, teleportStartX, teleportRangeX, teleportStartY, teleportRangeY, settings);
            }

            return result;
        }
    }
}