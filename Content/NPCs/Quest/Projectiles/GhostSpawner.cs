using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class GhostSpawner1 : ModProjectile
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
            Projectile.timeLeft = 35;
            Projectile.aiStyle = -1;
			Projectile.alpha = 255;
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
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					//gives the projectile after images a shaking effect
					float x = Main.rand.Next(-2, 3) * scale;
					float y = Main.rand.Next(-2, 3) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, Color.Red, Projectile.rotation, drawOrigin, scale * 2f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

		public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

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
        }

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_OgreHurt, Projectile.Center);

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int Bandit = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y + 35, ModContent.NPCType<BanditBruiser>(), ai0: Projectile.ai[2]);

				NetMessage.SendData(MessageID.SyncNPC, number: Bandit);
			}

        	float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(5f, 5f);
				Vector2 Bounds = new Vector2(3f, 3f);
				float intensity = 5f;

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

    public class GhostSpawner2 : ModProjectile
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
            Projectile.timeLeft = 35;
            Projectile.aiStyle = -1;
			Projectile.alpha = 255;
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
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					//gives the projectile after images a shaking effect
					float x = Main.rand.Next(-2, 3) * scale;
					float y = Main.rand.Next(-2, 3) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, Color.Lime, Projectile.rotation, drawOrigin, scale * 2f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

		public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

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
        }

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_GoblinScream with { Pitch = -0.25f }, Projectile.Center);

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int Bandit = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y + 35, ModContent.NPCType<BanditWizard>(), ai0: Projectile.ai[2]);

				NetMessage.SendData(MessageID.SyncNPC, number: Bandit);
			}

        	float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(5f, 5f);
				Vector2 Bounds = new Vector2(3f, 3f);
				float intensity = 5f;

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GreenTorch, 0f, 0f, 100, default, 2f);
				Main.dust[num104].noGravity = true;
				Main.dust[num104].position = Projectile.Center + vector12;
				Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
		}
    }

    public class GhostSpawner3 : ModProjectile
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
            Projectile.timeLeft = 35;
            Projectile.aiStyle = -1;
			Projectile.alpha = 255;
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
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					//gives the projectile after images a shaking effect
					float x = Main.rand.Next(-2, 3) * scale;
					float y = Main.rand.Next(-2, 3) * scale;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos + new Vector2(x, y), null, Color.Cyan, Projectile.rotation, drawOrigin, scale * 2f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

		public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

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
        }

        public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_DarkMageHurt with { Pitch = 1f }, Projectile.Center);

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
            	int Bandit = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y + 35, ModContent.NPCType<BanditPriest>(), ai0: Projectile.ai[2]);

                NetMessage.SendData(MessageID.SyncNPC, number: Bandit);
			}

        	float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(5f, 5f);
				Vector2 Bounds = new Vector2(3f, 3f);
				float intensity = 5f;

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.BlueTorch, 0f, 0f, 100, default, 2f);
				Main.dust[num104].noGravity = true;
				Main.dust[num104].position = Projectile.Center + vector12;
				Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
		}
    }
}