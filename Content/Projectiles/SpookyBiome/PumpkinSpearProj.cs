using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class PumpkinSpearProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pumpkin Poker");
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