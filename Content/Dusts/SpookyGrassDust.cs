using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Dusts
{
    public class SpookyGrassDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.velocity.Y *= 1f;
			dust.velocity.X *= 1f;
			dust.scale *= 1f;
			dust.noGravity = false;
			dust.noLight = true;
		}
	}
}