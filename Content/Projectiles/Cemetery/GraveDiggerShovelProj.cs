using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class GraveDiggerShovelProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/Cemetery/GraveDiggerShovel";

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            Projectile.ai[0] = 30;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.Crit)
            {
                SoundEngine.PlaySound(SoundID.Item178, target.Center);

                SpookyPlayer.ScreenShakeAmount = 3;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<GraveDiggerShovelBonk>(), 0, 0f, Main.myPlayer);
            }

            //return to the player upon hitting an enemy
            Projectile.ai[0] = 30;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation += 0.5f * (float)Projectile.direction;

            Projectile.timeLeft = 2;

            Projectile.ai[0]++;
            
            if (Projectile.ai[0] >= 30)
            {
                Vector2 ReturnSpeed = owner.Center - Projectile.Center;
                ReturnSpeed.Normalize();
                ReturnSpeed *= 12;

                Projectile.velocity = ReturnSpeed;

                if (Projectile.Hitbox.Intersects(owner.Hitbox))
                {
                    Projectile.Kill();
                }
            }
        }
    }
}