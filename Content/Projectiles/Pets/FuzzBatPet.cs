using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class FuzzBatPet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
			Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 5)
            .WithOffset(-20f, -12f).WithSpriteDirection(-1).WithCode(CharacterPreviewCustomization);
        }

		public static void CharacterPreviewCustomization(Projectile proj, bool walking)
		{
			DelegateMethods.CharacterPreview.Float(proj, walking);
		}

        public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.ZephyrFish);
			AIType = ProjectileID.ZephyrFish;
			Projectile.width = 60;
            Projectile.height = 38;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().FuzzBatPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().FuzzBatPet)
            {
				Projectile.timeLeft = 2;
            }
		}
	}
}