using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class EyeWizardEnergy : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/TrailSquare";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[12];

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.aiStyle = -1;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

				if (trailLength[k] == Vector2.Zero)
				{
					return false;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

                    for (int j = 0; j < 360; j += 90)
                    {
                        Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

                        Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 2.5f), 0).RotatedBy(MathHelper.ToRadians(j));

                        Main.spriteBatch.Draw(ProjTexture.Value, drawPos + circular, null, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0f);
                    }
				}

				previousPosition = currentPos;
			}

			return false;
		}

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

		public override void AI()
        {
            NPC Parent = Main.npc[(int)Projectile.ai[0]];

			//kill this npc if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<EyeWizard>())
			{
                Projectile.active = false;
			}

			if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}

            Projectile.ai[1]++;
            if (Projectile.ai[1] < 3)
            {
                Projectile.scale -= 0.5f;
            }
            if (Projectile.ai[1] >= 3)
            {
                Projectile.scale += 0.5f;
            }
            
            if (Projectile.ai[1] > 6)
            {
                Projectile.ai[1] = 0;
                Projectile.scale = 1f;
            }

			Projectile.ai[2]++;
            if (Projectile.ai[2] > 30)
            {
				Vector2 Speed = Parent.Center - Projectile.Center;
				Speed.Normalize();
				Speed *= 25;

				Projectile.velocity = Speed;

				if (Projectile.Hitbox.Intersects(Parent.Hitbox))
				{
					Parent.ai[0] = 1;

					Projectile.Kill();
				}
			}
        }

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item104 with { Volume = 0.5f }, Projectile.Center);

        	float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(2f, 2f);
				Vector2 Bounds = new Vector2(2f, 2f);
				float intensity = 3f;

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.RedTorch, 0f, 0f, 100, default, 2f);
				Main.dust[num104].noGravity = true;
				Main.dust[num104].position = Projectile.Center + vector12;
				Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
		}
    }
}