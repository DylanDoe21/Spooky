using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
	public class SporeBallProj : ModProjectile
	{
        public override string Texture => "Spooky/Content/Items/SpiderCave/SporeBall";

        float addedStretch = 0f;
		float stretchRecoil = 0f;

        private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 40;
            Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 360;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}
        
        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			float stretch = 0f;

			stretch = Math.Abs(stretch) - addedStretch;

			//limit how much it can stretch
			if (stretch > 0.2f)
			{
				stretch = 0.2f;
			}

			//limit how much it can squish
			if (stretch < -0.2f)
			{
				stretch = -0.2f;
			}

			Vector2 scaleStretch = new Vector2(1f - stretch, 1f + stretch);

            if (Projectile.velocity.Y <= 0)
			{
				scaleStretch = new Vector2(1f + stretch, 1f - stretch);
			}
			if (Projectile.velocity.Y > 0)
			{
				scaleStretch = new Vector2(1f - stretch, 1f + stretch);
			}

            int height = ProjTexture.Height() / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, ProjTexture.Width(), height);

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(ProjTexture.Width() / 2f, height / 2f), scaleStretch, SpriteEffects.None, 0);

			return false;
		}

		public override void AI()
		{
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.velocity *= 0.98f;

            //stretch stuff
			if (stretchRecoil > 0)
			{
				stretchRecoil -= 0.05f;
			}
			else
			{
				stretchRecoil = 0;
			}

			addedStretch = -stretchRecoil;

            Projectile.ai[0]++;
            if (Projectile.ai[0] % 20 == 0)
            {
                stretchRecoil = 0.65f;

                float RandomRotation = MathHelper.ToRadians(Main.rand.NextFloat(0f, 360f));

                Vector2 velocity = new Vector2(3f, 3f);
                Vector2 Bounds = new Vector2(1f, 1f);
                float intensity = 3f;

                Vector2 vector12 = Vector2.UnitX * 0f;
                vector12 += -Vector2.UnitY.RotatedBy((double)(6f), default) * Bounds;
                vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                Vector2 ShootVelocity = (velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity).RotatedBy(RandomRotation);

                Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, ShootVelocity, ModContent.ProjectileType<SporeCloud>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, ai0: Main.rand.Next(0, 2));
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.8f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
            }

			return false;
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.Center);

            for (int numDusts = 0; numDusts < 18; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 288, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;

				if (Main.dust[dust].position != Projectile.Center)
                {
				    Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                }
			}
        }
	}
}