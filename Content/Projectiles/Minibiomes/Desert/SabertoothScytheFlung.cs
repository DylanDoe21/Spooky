using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
    public class SabertoothScytheFlung : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
			Projectile.height = 24;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 200;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return Projectile.ai[0] > 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.45f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 25)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
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