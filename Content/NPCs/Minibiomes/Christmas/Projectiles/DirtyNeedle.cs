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
    public class DirtyNeedle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
			Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
        }

		public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
		{
			target.AddBuff(BuffID.Bleeding, 300);

			switch (Main.rand.Next(3))
			{
				case 0:
				{
					target.AddBuff(BuffID.Weak, 600);
					break;
				}
				case 1:
				{
					target.AddBuff(BuffID.Poisoned, 600);
					break;
				}
				case 2:
				{
					target.AddBuff(BuffID.Confused, 600);
					break;
				}
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.velocity.Y <= 0)
			{
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
			else
			{
				return true;
			}
		}

        public override void AI()
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Shatter with { Volume = 0.45f }, Projectile.Center);
		}
    }
}