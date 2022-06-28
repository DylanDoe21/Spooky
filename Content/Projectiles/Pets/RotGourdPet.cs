using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class RotGourdPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rotten Gourd");
			Main.projFrames[Projectile.type] = 12;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Sapling);
			AIType = ProjectileID.Sapling;
			Projectile.width = 40;
			Projectile.height = 40;
            Projectile.timeLeft = 999999999;
            Projectile.timeLeft *= 999999999;
            Projectile.penetrate = -1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().RotGourdPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().RotGourdPet)
            {
				Projectile.timeLeft = 2;
            }
		}
	}
}