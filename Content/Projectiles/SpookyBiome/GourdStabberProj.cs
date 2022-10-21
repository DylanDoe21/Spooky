using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class GourdStabberProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gourd Stabber");
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Trident);
			Projectile.DamageType = DamageClass.Melee;
			Projectile.height = 20;
			Projectile.width = 20;
			AIType = ProjectileID.Trident;
		}
	}
}