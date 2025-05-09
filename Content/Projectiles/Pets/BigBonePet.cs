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
        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }

		public override void SetDefaults()
		{
			Projectile.width = 46;
            Projectile.height = 40;
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
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Pets/BigBonePetChain");
                
                bool flip = false;
				if (player.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = Projectile.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(Projectile.rotation);
				Vector2 p0 = player.Center;
				Vector2 p1 = player.Center;
				Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(Projectile.rotation);
				Vector2 p3 = myCenter;

				int segments = 32;

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

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
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

            Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6) 
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 4) 
				{
					Projectile.frame = 0;
				}
			}

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            
            Lighting.AddLight(Projectile.Center, 1.5f, 0.8f, 0f);

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
            int num = 1;
            for (int k = 0; k < Projectile.whoAmI; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
                {
                    num++;
                }
            }
            direction.X += ((10 + num * 40) * player.direction);
            direction.Y -= 70f;
            float distanceTo = direction.Length();
            if (distanceTo > 200f && speed < 30f)
            {
                speed = 30f;
            }
            if (distanceTo < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 48f)
            {
                direction.Normalize();
                direction *= speed;
                float temp = 40 / 2f;
                Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
            }
            else
            {
                Projectile.direction = Main.player[Projectile.owner].direction;
                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
            }

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
                return;
            }
		}
	}
}