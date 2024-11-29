using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;

using Spooky.Content.NPCs.Minibiomes.TarPits.Projectiles;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Pylon;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.Cemetery.Ambient;
using Spooky.Content.Tiles.NoseTemple.Furniture;
using Spooky.Content.Tiles.SpiderCave.Ambient;
using Spooky.Content.Tiles.SpookyBiome.Ambient;
using Spooky.Content.Tiles.SpookyHell.Furniture;
using Spooky.Content.Tiles.Water;

namespace Spooky.Core
{
    public class TileGlobal : GlobalTile
    {
        public static bool LightingEssentialsActive() => ModLoader.TryGetMod("LightingEssentials", out _);

		public static Vector2 TileOffset => Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
		public static Vector2 TileCustomPosition(int i, int j, Vector2 off = default) => (new Vector2(i, j) * 16) - Main.screenPosition - off + TileOffset;

		public override void SetStaticDefaults()
        {
            //do not run any lighting essentials checking if the mod is not enabled
            if (LightingEssentialsActive())
            {
                //set tileLighted to true for all ambient grasses in spooky mod
                //using reflection, get the lighting essentials config and make sure the "Light Environment" option is turned on
                var LightingEssentialsConfig = ModContent.Find<ModConfig>("LightingEssentials/Config");
                bool LightEnvironmentOn = (bool)LightingEssentialsConfig.GetType().GetField("LightEnvironment", BindingFlags.Public | BindingFlags.Instance).GetValue(LightingEssentialsConfig);

                if (LightEnvironmentOn)
                {
                    Main.tileLighted[ModContent.TileType<SpookyWeedsOrange>()] = true;
                    Main.tileLighted[ModContent.TileType<SpookyWeedsGreen>()] = true;
                    Main.tileLighted[ModContent.TileType<CemeteryWeeds>()] = true;
                    Main.tileLighted[ModContent.TileType<CatacombWeeds>()] = true;
                    Main.tileLighted[ModContent.TileType<SpiderCaveWeeds>()] = true;
                }
            }
        }

        public override void ModifyLight(int i, int j, int type, ref float r, ref float g, ref float b)
        {
            //do not run any lighting essentials checking if the mod is not enabled
            if (LightingEssentialsActive())
            {
                //add the actual lighting for all the ambient grasses in spooky mod
                //like above, use reflection get the lighting essentials config and make sure the "Light Environment" option is turned on
                var LightingEssentialsConfig = ModContent.Find<ModConfig>("LightingEssentials/Config");
                bool LightEnvironmentOn = (bool)LightingEssentialsConfig.GetType().GetField("LightEnvironment", BindingFlags.Public | BindingFlags.Instance).GetValue(LightingEssentialsConfig);

                if (LightEnvironmentOn)
                {
                    if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWeedsOrange>())
                    {
                        r = 175f / 400f;
                        g = 102f / 400f;
                        b = 36f / 400f;
                    }

                    if (Main.tile[i, j].TileType == ModContent.TileType<SpookyWeedsGreen>())
                    {
                        r = 78f / 400f;
                        g = 120f / 400f;
                        b = 48f / 400f;
                    }

                    if (Main.tile[i, j].TileType == ModContent.TileType<CemeteryWeeds>())
                    {
                        r = 38f / 400f;
                        g = 77f / 400f;
                        b = 53f / 400f;
                    }

                    if (Main.tile[i, j].TileType == ModContent.TileType<CatacombWeeds>())
                    {
                        r = 56f / 400f;
                        g = 109f / 400f;
                        b = 62f / 400f;
                    }

                    if (Main.tile[i, j].TileType == ModContent.TileType<SpiderCaveWeeds>())
                    {
                        r = 70f / 400f;
                        g = 120f / 400f;
                        b = 40f / 400f;
                    }
                }
            }
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