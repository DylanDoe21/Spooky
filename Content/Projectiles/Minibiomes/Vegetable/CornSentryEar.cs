using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
    public class CornSentryEar : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 20;
        }

        public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
        {
			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.65f;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item54 with { Pitch = -1.2f }, Projectile.Center);

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(Projectile.velocity.X * 2, Projectile.velocity.Y), 
            ModContent.ProjectileType<CornSentryPopcorn>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
		}
	}
}