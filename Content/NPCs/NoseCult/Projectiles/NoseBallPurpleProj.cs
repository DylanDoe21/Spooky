using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.NoseCult.Projectiles
{
	public class NoseBallPurpleProj : ModProjectile
	{
		private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
		{
			Projectile.width = 52;
			Projectile.height = 52;
			Projectile.hostile = true;
			Projectile.tileCollide = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 60;
			Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);
			Color glowColor = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.White);

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.3f, SpriteEffects.None, 0);

			return true;
		}

		public override bool CanHitPlayer(Player target)
		{
			return false;
		}

		public override void AI()
		{
			Projectile.rotation += 0.12f * (float)Projectile.direction;

			Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;

			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= 10;
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                Projectile.velocity.X = -oldVelocity.X * 0.95f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
            }

			return false;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item16, Projectile.Center);

			ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings
			{
				PositionInWorld = Projectile.Center
			});

			int NoseBall = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y + Projectile.height / 2, ModContent.NPCType<NoseBallPurple>());
			Main.npc[NoseBall].velocity = Projectile.velocity;
			Main.npc[NoseBall].rotation = Projectile.rotation;

			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.SyncNPC, number: NoseBall);
			}
		}
	}

	public class NoseBallRedProj : NoseBallPurpleProj
	{
		private static Asset<Texture2D> ProjTexture;

		public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);
			Color glowColor = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.White);

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.3f, SpriteEffects.None, 0);

			return true;
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item16, Projectile.Center);

			ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings
			{
				PositionInWorld = Projectile.Center
			});

			int NoseBall = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y + Projectile.height / 2, ModContent.NPCType<NoseBallRed>());
			Main.npc[NoseBall].velocity = Projectile.velocity;
			Main.npc[NoseBall].rotation = Projectile.rotation;

			if (Main.netMode == NetmodeID.Server)
			{
				NetMessage.SendData(MessageID.SyncNPC, number: NoseBall);
			}
		}
	}
}