using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class FemurFractureSkull : ModProjectile
	{
        private List<Vector2> cache;
        private Trail trail;

		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 26;
            Projectile.DamageType = DamageClass.Melee;
			Projectile.friendly = true;
			Projectile.tileCollide = true;
			Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
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

            return true;
        }

        const int TrailLength = 22;

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
            trail = trail ?? new Trail(Main.instance.GraphicsDevice, TrailLength, new TriangularTip(4), factor => 15 * factor, factor =>
            {
                return Color.Lerp(Color.Gray, Color.Lime, factor.X) * factor.X;
            });

            trail.Positions = cache.ToArray();
            trail.NextPosition = Projectile.Center + Projectile.velocity;
        }

		public override void AI()
		{
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            if (!Main.dedServ)
            {
                ManageCaches();
                ManageTrail();
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 30)
            {
                Projectile.penetrate = 1;

                int foundTarget = HomeOnTarget();
                if (foundTarget != -1)
                {
                    NPC target = Main.npc[foundTarget];
                    Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 20;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                    Projectile.tileCollide = false;
                }
                else
                {
                    Projectile.tileCollide = true;
                }
            }
		}

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 400;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public override void OnKill(int timeLeft)
		{
        	for (int numDusts = 0; numDusts < 8; numDusts++)
			{
                int dustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 0.2f);
                Main.dust[dustGore].color = Color.Lime;
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-0.5f, 0.5f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-0.5f, 0.5f);
                Main.dust[dustGore].noGravity = true;
            }
		}
	}
}