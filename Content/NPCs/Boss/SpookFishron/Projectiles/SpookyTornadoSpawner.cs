using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.SpookFishron.Projectiles
{
    public class SpookyTornadoSpawner : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			Projectile.width = 62;
            Projectile.height = 62;
			Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            if (Main.snowMoon)
            {
                ProjTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookFishron/Projectiles/FrostMoonTextures/SpookyTornadoSpawner");
            }
            else
            {
                ProjTexture = ModContent.Request<Texture2D>(Texture);
            }

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X - Projectile.scale * 0.5f, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			for (int i = 0; i < 360; i += 90)
            {
				Color color1 = new Color(222, 133, 51, 0);
				Color color2 = new Color(206, 96, 40, 0);

				if (Main.snowMoon)
            	{
					color1 = new Color(119, 187, 217, 0);
					color2 = new Color(98, 154, 179, 0);
				}

				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lerp(color2, color1, i));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[(int)Projectile.ai[1]];

			Projectile.rotation += 1f * (float)Projectile.direction;

			if (Projectile.timeLeft > 60 && Projectile.alpha > 0)
			{
				Projectile.alpha -= 25;
			}
			if (Projectile.timeLeft < 60)
			{
				Projectile.alpha += 25;
			}

			if (Projectile.ai[0] == 0)
			{
				Projectile.tileCollide = true;

				Projectile.velocity.Y = 10f;
				Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 12;
				Projectile.velocity.X = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 7).X;
			}
			else
			{
				Projectile.tileCollide = false;

				Vector2 desiredVelocity = Projectile.DirectionTo(player.Center) * 15;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 7);
			}

			if ((Projectile.wet && Projectile.ai[0] == 0) || Projectile.Hitbox.Intersects(player.Hitbox))
			{
				Projectile.Kill();
			}
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

			float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(15f, 15f);
				Vector2 Bounds = new Vector2(3f, 3f);
				float intensity = 15f;

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int dustType = Main.snowMoon ? DustID.IceTorch : DustID.Torch;
				int num104 = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 100, default, 3f);
				Main.dust[num104].noGravity = true;
				Main.dust[num104].position = Projectile.Center + vector12;
				Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}

			int TornadoSize = Projectile.ai[0] > 0 ? 50 : 25;

			Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SpookyTornado>(), Projectile.damage, 2f, (int)Projectile.ai[1], TornadoSize, TornadoSize - 1, TornadoSize);
		}
	}
}
     
          






