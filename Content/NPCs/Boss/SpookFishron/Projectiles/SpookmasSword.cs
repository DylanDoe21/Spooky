using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.SpookFishron.Projectiles
{
    public class SpookmasSword : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			Projectile.width = 46;
            Projectile.height = 46;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
			Projectile.aiStyle = -1;
		}

		public override bool PreDraw(ref Color lightColor)
        {
			NPC Parent = Main.npc[(int)Projectile.ai[0]];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

			if ((Parent.ai[1] <= 2 && Parent.localAI[0] > 285 && Parent.localAI[0] < 345) || (Parent.ai[1] > 2 && Parent.localAI[0] >= 330))
			{
				for (int i = 0; i < 360; i += 60)
				{
					Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lerp(Color.OrangeRed, Color.Orange, i / 30));

					Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), 0).RotatedBy(MathHelper.ToRadians(i));
					
					Vector2 SparkleOffset = new Vector2(152, 0).RotatedBy(Parent.rotation + MathHelper.PiOver2);

					Vector2 SparklePos = SparkleOffset + Parent.Center + circular - Main.screenPosition;
					float time = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 0.5f / 2.5f * 150f)) / 2f + 0.5f;
					DrawPrettyStarSparkle(Projectile.Opacity, SpriteEffects.None, SparklePos, Color.Gold, color, 0.5f, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(5f * time, 5f * time), new Vector2(5, 5));
				}
			}

			return false;
        }

		private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness)
		{
			Texture2D Texture = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = Texture.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float Intensity = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * Intensity;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * Intensity;
			color *= Intensity;
			color2 *= Intensity;
			Main.EntitySpriteDraw(Texture, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color, 0f + rotation, origin, vector2 * 0.6f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.3f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.3f, dir);
		}

		public override void AI()
		{
			NPC Parent = Main.npc[(int)Projectile.ai[0]];
			Player player = Main.player[Projectile.owner];

			if (!Parent.active || Parent.type != ModContent.NPCType<SpookFishron>())
			{
				Projectile.Kill();
			}

            if (Parent.active && Parent.type == ModContent.NPCType<SpookFishron>() && Parent.ai[0] != 6)
            {
                Projectile.Kill();
            }

			Projectile.timeLeft = 5;

			Vector2 pos = new Vector2(125, 0).RotatedBy(Parent.rotation + MathHelper.PiOver2);
			Projectile.Center = pos + new Vector2(Parent.Center.X, Parent.Center.Y);
			Projectile.rotation = Parent.rotation;

			if (Parent.ai[1] <= 2 && Parent.localAI[0] == 345)
			{
				Projectile.localAI[2]++;

				SoundEngine.PlaySound(SoundID.Item29, Projectile.Center);

				Vector2 position = new Vector2(152, 0).RotatedBy(Parent.rotation + MathHelper.PiOver2) + Parent.Center;

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, Vector2.Zero, ModContent.ProjectileType<SpookmasSwordStar>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			
				Projectile.netUpdate = true;
			}

			if (Parent.ai[1] > 2 && Parent.localAI[0] == 420)
			{
				Vector2 ShootSpeed = player.Center - Projectile.Center;
				ShootSpeed.Normalize();
				ShootSpeed *= 55f;

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<SpookmasSwordSpin>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);

				Projectile.netUpdate = true;

				Projectile.Kill();
			}
		}
	}
}
     
          






