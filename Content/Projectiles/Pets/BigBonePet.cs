using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class BigBonePet : ModProjectile
	{
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
                Vector2 rootPosition = player.Center;

                Vector2[] bezierPoints = { rootPosition, rootPosition + new Vector2(0, -30), Projectile.Center + new Vector2(-30 * Projectile.direction, 0).RotatedBy(Projectile.rotation), Projectile.Center + new Vector2(-7 * Projectile.direction, 0).RotatedBy(Projectile.rotation) };
                float bezierProgress = 0;
                float bezierIncrement = 8;

                Texture2D texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Pets/BigBonePetChain").Value;
                Vector2 textureCenter = Projectile.spriteDirection == -1 ? new Vector2(2, 2) : new Vector2(2, 2);

                float rotation;

                while (bezierProgress < 1)
                {
                    //draw stuff
                    Vector2 oldPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);

                    //increment progress
                    while ((oldPos - BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress)).Length() < bezierIncrement)
                    {
                        bezierProgress += 0.1f / BezierCurveUtil.BezierCurveDerivative(bezierPoints, bezierProgress).Length();
                    }

                    Vector2 newPos = BezierCurveUtil.BezierCurve(bezierPoints, bezierProgress);
                    rotation = (newPos - oldPos).ToRotation() + MathHelper.Pi;

                    Main.spriteBatch.Draw(texture, (oldPos + newPos) / 2 - Main.screenPosition, texture.Frame(), lightColor, rotation, textureCenter, Projectile.scale, SpriteEffects.None, 0f);
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

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
                return;
            }
		}
	}
}