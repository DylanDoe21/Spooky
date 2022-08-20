using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria.ModLoader;

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