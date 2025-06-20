using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
    public class LockerFish : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;
            
            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 15)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCHit2 with { Pitch = 1.25f, Volume = 0.35f }, Projectile.Center);

			for (int numDusts = 0; numDusts < 5; numDusts++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Bone, 0f, -2f, 0, default, 1f);
				Main.dust[dust].position.X += Main.rand.Next(-15, 15) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-15, 15) * 0.05f - 1.5f;
				Main.dust[dust].velocity = Projectile.velocity * 0.5f;
			}
		}
    }
}