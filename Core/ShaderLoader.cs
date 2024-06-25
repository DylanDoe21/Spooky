using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Core
{
    public static class ShaderLoader
    {
        public static Effect GlowyTrail;

        public static Asset<Texture2D> GlobTrail;
        public static Asset<Texture2D> GlowTrail;
        public static Asset<Texture2D> MagicTrail;
        public static Asset<Texture2D> ShadowTrail;
        public static Asset<Texture2D> ShadowTrailBig;

        public static void Load()
        {
            GlowyTrail = ModContent.Request<Effect>("Spooky/Effects/GlowyTrail", AssetRequestMode.ImmediateLoad).Value;

            GlobTrail ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/GlobTrail");
            GlowTrail ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/GlowTrail");
            MagicTrail ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/MagicTrail");
            ShadowTrail ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/ShadowTrail");
            ShadowTrailBig ??= ModContent.Request<Texture2D>("Spooky/ShaderAssets/ShadowTrailBig");
        }

        public static void Unload()
        {
            GlowyTrail = null;
        }
    }
}