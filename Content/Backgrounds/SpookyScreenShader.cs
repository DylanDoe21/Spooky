using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Backgrounds
{
    //basically just a general screenshader for all of the mod's screen tints
    public class SpookyScreenShader : ScreenShaderData
    {
        public SpookyScreenShader(string passName) : base(passName)
        {
        }

        private void UpdateSpookyIndex()
        {
        }
        
        public override void Apply()
        {
            UpdateSpookyIndex();
            base.Apply();
        }
    }
}