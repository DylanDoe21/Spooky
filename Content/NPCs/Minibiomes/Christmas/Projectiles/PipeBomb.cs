using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{
    public class PipeBomb : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 16;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

		public override bool? CanDamage()
        {
            return false;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.45f }, Projectile.Center);

			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X * 0.85f;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y * 0.85f;
			}

			return false;
		}

        public override void AI()
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14 with { Volume = 0.45f }, Projectile.Center);

			//flame dusts
			for (int numDust = 0; numDust < 45; numDust++)
			{
				int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, -2f, 0, default, 1.5f);
				Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-7f, 8f);
				Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-4f, 5f);
				Main.dust[dustGore].noGravity = true;
			}

			//explosion smoke
			for (int numExplosion = 0; numExplosion < 5; numExplosion++)
			{
				int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, new Color(146, 75, 19) * 0.5f, 0.45f);
				Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-1f, 2f);
				Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-1f, 2f);
				Main.dust[DustGore].noGravity = true;
			}

			foreach (Player player in Main.ActivePlayers)
			{
				if (!player.dead && player.Distance(Projectile.Center) <= 135f)
				{
					player.Hurt(PlayerDeathReason.ByCustomReason(Language.GetText("Mods.Spooky.DeathReasons.ToyRobotExplosion").ToNetworkText(player.name)), Projectile.damage + Main.rand.Next(0, 30), 0);
				}
			}
		}
    }
}