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
			Main.projFrames[Projectile.type] = 10;
			Main.projPet[Projectile.type] = true;

			ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, 6, 5)
            .WithOffset(-8f, 0f).WithSpriteDirection(-1).WhenNotSelected(0, 0);
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.PetLizard);
			AIType = ProjectileID.PetLizard;
			Projectile.width = 34;
			Projectile.height = 40;
            Projectile.timeLeft = 2;
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