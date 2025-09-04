using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
	public class TarBombProj : ModProjectile
	{
        public override void SetDefaults() 
        {
			Projectile.width = 18;
			Projectile.height = 42;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 1000;
            Projectile.penetrate = 1;
		}

		public override bool? CanDamage()
        {
            return Projectile.ai[0] >= 30;
        }

		public override void AI() 
        {
			Player player = Main.player[Projectile.owner];

			Projectile.tileCollide = Projectile.Center.Y > player.Center.Y;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

			Projectile.ai[0]++;
			if (Projectile.ai[0] >= 30)
			{
				Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Asphalt, -Projectile.velocity * 0.5f, default, default, 1f);
				dust.noGravity = true;

				Projectile.velocity.Y = Projectile.velocity.Y + 0.65f;

				if (Projectile.owner == Main.myPlayer)
                {
					Vector2 desiredVelocity = Projectile.DirectionTo(Main.MouseWorld) * 20;
					Projectile.velocity.X = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20).X;
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_KoboldExplosion with { Pitch = 1.5f }, Projectile.Center);

			//shoot tar blob projectiles
			for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
            {
                Vector2 ProjectilePosition = Projectile.Center + new Vector2(0, 15).RotatedByRandom(360);

                Vector2 ShootSpeed = ProjectilePosition - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 2f;

                Projectile.NewProjectile(Projectile.GetSource_Death(), ProjectilePosition, ShootSpeed - new Vector2(0, 5), ModContent.ProjectileType<TarBombBlob>(), Projectile.damage / 2, 0, Projectile.owner);
            }

			float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(Main.rand.NextFloat(4f, 9f), Main.rand.NextFloat(4f, 9f));
				Vector2 Bounds = new Vector2(5f, 5f);
				float intensity = Main.rand.NextFloat(3f, 8f);

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Asphalt, 0f, 0f, 100, default, 2f);
				Main.dust[num104].noGravity = true;
				Main.dust[num104].position = Projectile.Center + vector12;
				Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
			
			for (int numGores = 1; numGores <= 3; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.Find<ModGore>("Spooky/TarBombGore" + Main.rand.Next(1, 7)).Type);
				}
			}
		}
	}
}