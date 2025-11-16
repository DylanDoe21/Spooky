using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class BigBonePet : ModProjectile
	{
        bool Shake = false;

        private static Asset<Texture2D> ChainTexture1;
        private static Asset<Texture2D> ChainTexture2;

        public override void SetStaticDefaults()
		{
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 62;
            Projectile.height = 38;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            //only draw if the owner exists
            if (!player.dead)
			{
                ChainTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Pets/BigBonePetStem2");
                ChainTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Pets/BigBonePetStem3");
                
                bool flip = false;
				if (player.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture1.Height() / 2);
				Vector2 myCenter = Projectile.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(Projectile.rotation);
				Vector2 p0 = player.Center;
				Vector2 p1 = player.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), -75).RotatedBy(Projectile.rotation);
				Vector2 p3 = myCenter;

				int segments = 15;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = drawPosNext - drawPos2;
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

					Main.spriteBatch.Draw(ChainTexture1.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture1.Width(), 1), SpriteEffects.None, 0f);
				}
            }

            return true;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().BigBonePet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().BigBonePet)
            {
				Projectile.timeLeft = 2;
            }

            if (Shake)
            {
                Projectile.rotation += 0.01f;
                if (Projectile.rotation > 0.2f)
                {
                    Shake = false;
                }
            }
            else
            {
                Projectile.rotation -= 0.01f;
                if (Projectile.rotation < -0.2f)
                {
                    Shake = true;
                }
            }
            
            Projectile.spriteDirection = -player.direction;
            
            Lighting.AddLight(Projectile.Center, 1.5f, 1.5f, 1.2f);

			if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 10f;

            if (Projectile.ai[0] == 1f)
            {
                speed = 35f;
            }

            Vector2 center = Projectile.Center;
            Vector2 direction = player.Center - center;
            Projectile.ai[1] = 3600f;
            Projectile.netUpdate = true;

            direction.Y -= 70f;
            float distanceTo = direction.Length();
            if (distanceTo > 100f && speed < 30f)
            {
                speed = 30f;
            }
            if (distanceTo < 35f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 1200f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 35f)
            {
                direction.Normalize();
                direction *= speed;
                float temp = 40 / 2f;
                Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
            }
            else
            {
                Projectile.velocity *= 0.75f; //(float)Math.Pow(0.9, 40.0 / 40);
            }
		}
	}
}