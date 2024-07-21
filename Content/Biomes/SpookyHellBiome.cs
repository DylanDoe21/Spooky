
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Sentient;
using Spooky.Content.Tiles.Water;

namespace Spooky.Content.Biomes
{
    public class SpookyHellBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyHell");

        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

        public override int BiomeTorchItemType => ItemID.CrimsonTorch;

        public override ModWaterStyle WaterStyle => ModContent.GetInstance<BloodWaterStyle>();

        public override void Load()
        {
            On_TileLightScanner.ApplyHellLight += SpookyHellCustomLighting;
        }

        //modified vanilla hell lighting code, just makes the vanilla hell lighting a different color because the default hell lighting is ugly in the biome
        private void SpookyHellCustomLighting(On_TileLightScanner.orig_ApplyHellLight orig, TileLightScanner self, Tile tile, int x, int y, ref Vector3 lightColor)
        {
            if (ModContent.GetInstance<TileCount>().spookyHellTiles >= 500)
            {
                float Red = 0f;
                float Green = 0f;
                float Blue = 0f;
                
                float Intensity = 0.32f;

                if ((!tile.HasTile || !Main.tileNoSunLight[tile.TileType] || ((tile.Slope != 0 || tile.IsHalfBlock) && Main.tile[x, y - 1].LiquidAmount == 0 && Main.tile[x, y + 1].LiquidAmount == 0 && Main.tile[x - 1, y].LiquidAmount == 0 && Main.tile[x + 1, y].LiquidAmount == 0)) && lightColor.X < Intensity && (Main.wallLight[tile.WallType] || tile.IsWallInvisible) && tile.LiquidAmount < 200 && (!tile.IsHalfBlock || Main.tile[x, y - 1].LiquidAmount < 200))
                {
                    Red = Intensity * 1.5f;
                    Green = Intensity;
                    Blue = Intensity;
                }
                if ((!tile.HasTile || tile.IsHalfBlock || !Main.tileNoSunLight[tile.TileType]) && tile.LiquidAmount < byte.MaxValue)
                {
                    Red = Intensity * 1.5f;
                    Green = Intensity;
                    Blue = Intensity;

                    //dont provide lighting behind walls or liquids
                    if (tile.WallType > 0 || tile.LiquidAmount > 0)
                    {
                        Red *= 0f;
                        Green *= 0f;
                        Blue *= 0f;
                    }
                }

                //set each light color value
                if (lightColor.X < Red)
                {
                    lightColor.X = Red;
                }
                if (lightColor.Y < Green)
                {
                    lightColor.Y = Green;
                }
                if (lightColor.Z < Blue)
                {
                    lightColor.Z = Blue;
                }

                return;
            }

            orig(self, tile, x, y, ref lightColor);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyHellBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:EyeValleyTint", isActive, player.Center);
        }

        public override void OnInBiome(Player player)
        {
            //water in the eye valley is blood, so the player should move slower in it
            if (player.wet)
            {
                player.velocity *= 0.95f;
            }
        }

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyHellTiles >= 500 && player.ZoneUnderworldHeight;

            return BiomeCondition;
        }
    }
}