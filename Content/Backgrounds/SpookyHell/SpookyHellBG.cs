using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Biomes;

namespace Spooky.Content.Backgrounds.SpookyHell
{
    public class SpookyHellBG : HellBGType
    {
        public override string TexturePath => "Spooky/Content/Backgrounds/SpookyHell/SpookyHellBG";

        public override bool IsActive()
        {
            return Main.LocalPlayer.InModBiome(ModContent.GetInstance<SpookyHellBiome>());
        }

        public override Color DrawColor => Color.Gray;
    }
}