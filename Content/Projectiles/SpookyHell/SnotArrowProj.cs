using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class SnotArrowProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 40;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;
			Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            SoundEngine.PlaySound(SoundID.NPCHit13 with { Pitch = 1.25f }, Projectile.Center);

            Projectile.position = Projectile.position + Projectile.velocity;
            Projectile.velocity = -Projectile.velocity;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.NPCHit13 with { Pitch = 1.25f }, Projectile.Center);

			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			return false;
		}

        public override void AI()       
        {
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.velocity.Y += 0.25f;
        }
    }
}