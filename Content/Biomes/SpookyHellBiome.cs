using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Light;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class SpookyHellBiome : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyHell");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void Load()
        {
            On_TileLightScanner.ApplyHellLight += SpookyHellCustomLighting;
        }

        private void SpookyHellCustomLighting(On_TileLightScanner.orig_ApplyHellLight orig, TileLightScanner self, Tile tile, int x, int y, ref Vector3 lightColor)
        {
            orig.Invoke(self, tile, x, y, ref lightColor);

            //copied vanilla hell lighting code, just makes the vanilla hell lighting completely white while in the eye valley to make it not look ugly
            if (ModContent.GetInstance<TileCount>().spookyHellTiles >= 500)
            {
                float Red = 0f;
                float Green = 0f;
                float Blue = 0f;
                
                float Intensity = 0.55f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 2f) * 0.08f;

                if ((!tile.HasTile || !Main.tileNoSunLight[tile.TileType] || ((tile.Slope != 0 || tile.IsHalfBlock) && Main.tile[x, y - 1].LiquidAmount == 0 && Main.tile[x, y + 1].LiquidAmount == 0 && Main.tile[x - 1, y].LiquidAmount == 0 && Main.tile[x + 1, y].LiquidAmount == 0)) && lightColor.X < Intensity && (Main.wallLight[tile.WallType] || tile.IsWallInvisible) && tile.LiquidAmount < 200 && (!tile.IsHalfBlock || Main.tile[x, y - 1].LiquidAmount < 200))
                {
                    Red = Intensity;
                    Green = Intensity;
                    Blue = Intensity;
                }
                if ((!tile.HasTile || tile.IsHalfBlock || !Main.tileNoSunLight[tile.TileType]) && tile.LiquidAmount < byte.MaxValue)
                {
                    Red = Intensity;
                    Green = Intensity;
                    Blue = Intensity;

                    //reduce lighting behind walls
                    if (tile.WallType > 0)
                    {
                        Red *= 0.5f;
                        Green *= 0.5f;
                        Blue *= 0.5f;
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
            }
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("Spooky:EyeValleyTint", isActive, player.Center);
        }

        //bestiary stuff
        public override string BestiaryIcon => "Spooky/Content/Biomes/SpookyHellBiomeIcon";
        public override string MapBackground => BackgroundPath;
		public override string BackgroundPath => base.BackgroundPath;
		public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyHellTiles >= 500 && player.ZoneUnderworldHeight;

            return BiomeCondition;
        }
    }
}