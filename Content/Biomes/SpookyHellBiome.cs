using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Biomes
{
    public class SpookyHellBiome : ModBiome
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Hell");
        }

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Content/Sounds/Music/SpookyHell");

        public override SceneEffectPriority Priority => SceneEffectPriority.Event;

        public override void SpecialVisuals(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:SpookyHellTint", player.InModBiome(ModContent.GetInstance<SpookyHellBiome>()), player.Center);
        }
        public override void OnLeave(Player player)
        {
            player.ManageSpecialBiomeVisuals("Spooky:SpookyHellTint", false, player.Center);
        }

        //bestiary stuff, will change this to my own stuff when i finish the actual port
        public override string BestiaryIcon => base.BestiaryIcon;
        public override string BackgroundPath => base.BackgroundPath;
        public override Color? BackgroundColor => base.BackgroundColor;

        public override bool IsBiomeActive(Player player)
        {
            bool BiomeCondition = ModContent.GetInstance<TileCount>().spookyHellTiles >= 200 && player.ZoneUnderworldHeight;

            return BiomeCondition;
        }
    }
}