using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Core
{
    public class ShaderLoader : IAutoload
    {
        public static Effect GlowyTrail;

        public void Load()
        {
            GlowyTrail = ModContent.Request<Effect>("Spooky/Effects/GlowyTrail", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public void Unload()
        {
            GlowyTrail = null;
        }
    }
}