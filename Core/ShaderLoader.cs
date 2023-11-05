using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Core
{
    public static class ShaderLoader
    {
        public static Effect GlowyTrail;

        public static void Load()
        {
            GlowyTrail = ModContent.Request<Effect>("Spooky/Effects/GlowyTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public static void Unload()
        {
            GlowyTrail = null;
        }
    }
}