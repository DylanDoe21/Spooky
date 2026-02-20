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
	public class ValleyNautilusPet : ModProjectile
	{
        private static Asset<Texture2D> GlowTexture;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 3;
			Main.projPet[Projectile.type] = true;

			ProjectileID.Sets.CharacterPreviewAnimations[Projectile.type] = ProjectileID.Sets.SimpleLoop(0, Main.projFrames[Projectile.type], 7)
			.WithOffset(-30f, 10f).WithSpriteDirection(1).WhenNotSelected(0, 0);
		}

        public override void SetDefaults()
		{
			Projectile.width = 76;
            Projectile.height = 60;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

        public override void PostDraw(Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

			GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Vector2 drawOrigin = new(GlowTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, GlowTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, GlowTexture.Width(), GlowTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(GlowTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().ValleyNautilusPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().ValleyNautilusPet)
            {
				Projectile.timeLeft = 2;
            }

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 7)
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 3)
				{
					Projectile.frame = 0;
				}
			}

			Projectile.spriteDirection = Projectile.Center.X > player.Center.X ? -1 : 1;

			Vector2 RotateTowards = player.Center - Projectile.Center;

            float RotateDirection = (float)Math.Atan2(RotateTowards.Y, RotateTowards.X);
            float RotateSpeed = 0.05f;

			Projectile.rotation = Projectile.rotation.AngleTowards(RotateDirection - (Projectile.spriteDirection == -1 ? MathHelper.Pi : MathHelper.TwoPi), RotateSpeed);

			if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 12f;
            if (Projectile.ai[0] == 1f)
            {
                speed = 18f;
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
            direction.X -= (10 + num * 70) * player.direction;
            direction.Y -= 65f;
            float distanceTo = direction.Length();
            if (distanceTo > 200f && speed < 15f)
            {
                speed = 15f;
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
                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
            }
		}
	}
}