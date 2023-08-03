using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyBiome
{
	public class SkullWisp : ModProjectile
	{
		private List<Vector2> cache;
        private Trail trail;

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Raven);
            Projectile.width = 20;
			Projectile.height = 28;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.netImportant = true;
			Projectile.timeLeft = 2;
			Projectile.penetrate = -1;
			Projectile.minionSlots = 1;
			AIType = ProjectileID.Raven;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = ShaderLoader.GlowyTrail;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Spooky/ShaderAssets/ShadowTrail").Value); //trails texture image
            effect.Parameters["time"].SetValue((float)Main.timeForVisualEffects * 0.05f); //this affects something?
            effect.Parameters["repeats"].SetValue(1); //this is how many times the trail is drawn

            trail?.Render(effect);

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

			//draw aura
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SpookyBiome/SkullWispAura").Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
				float shakeX = Main.rand.Next(-2, 2);
			    float shakeY = Main.rand.Next(-2, 2);

                Color color = Color.Lerp(Color.Purple, Color.OrangeRed, numEffect);

                Vector2 vector = new Vector2(Projectile.Center.X - 1 + shakeX, Projectile.Center.Y + shakeY) + (numEffect / 4 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY + 2) * numEffect;
                Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
				Main.EntitySpriteDraw(tex, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.035f, effects, 0);
            }

            return true;
        }

		const int TrailLength = 8;

        private void ManageCaches()
        {
            if (cache == null)
            {
                cache = new List<Vector2>();
                for (int i = 0; i < TrailLength; i++)
                {
                    cache.Add(Projectile.Center);
                }
            }

            cache.Add(Projectile.Center);

            while (cache.Count > TrailLength)
            {
                cache.RemoveAt(0);
            }
        }

        private void ManageTrail()
        {
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 5 * factor, factor =>
            {
                return Color.Lerp(Color.Purple, Color.OrangeRed, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

		public override bool MinionContactDamage()
		{
			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Lighting.AddLight(Projectile.Center, 0.25f, 0.12f, 0f);

			if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

			if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().SkullWisp = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().SkullWisp)
			{
				Projectile.timeLeft = 2;
			}

			//prevent projectiles clumping together
			for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.08f;
					if (Projectile.position.X < other.position.X)
					{
						Projectile.velocity.X -= pushAway;
					}
					else
					{
						Projectile.velocity.X += pushAway;
					}
					if (Projectile.position.Y < other.position.Y)
					{
						Projectile.velocity.Y -= pushAway;
					}
					else
					{
						Projectile.velocity.Y += pushAway;
					}
				}
			}
		}
	}
}