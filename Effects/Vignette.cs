using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Effects
{
	[Autoload(Side = ModSide.Client)]
	public class Vignette : ScreenShaderData
	{
		public Vignette(Effect effect, string pass) : base(new Ref<Effect>(effect), pass)
		{
		}
	}
}