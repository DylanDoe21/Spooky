using Terraria.Graphics.Shaders;

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
