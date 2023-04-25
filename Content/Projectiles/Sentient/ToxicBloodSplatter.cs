using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class ToxicBloodSplatter : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Projectile.velocity.Y = Projectile.velocity.Y + 0.25f;

            for (int numDusts = 0; numDusts < 2; numDusts++)
			{
                Vector2 dustPosition = Projectile.position;
                dustPosition -= Projectile.velocity * ((float)numDusts * 0.25f);
                int dust = Dust.NewDust(dustPosition, 1, 1, DustID.Blood, 0f, 0f, 0, default, 1f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position = dustPosition;
                Main.dust[dust].velocity *= 0.2f;
            }
        }
    }
}