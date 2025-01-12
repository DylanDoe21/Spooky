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
    public class StakeLauncher : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
			Projectile.width = 22;
            Projectile.height = 22;
			Projectile.hostile = true;
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 5;
			Projectile.aiStyle = -1;
		}

		public override bool PreDraw(ref Color lightColor)
        {
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			return false;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)Projectile.ai[0]];
			Player player = Main.player[Parent.target];

			if (!Parent.active || Parent.type != ModContent.NPCType<SpookFishron>())
			{
				Projectile.Kill();
			}

            if (Parent.active && Parent.type == ModContent.NPCType<SpookFishron>() && Parent.ai[0] != 6)
            {
                Projectile.Kill();
            }

			Projectile.timeLeft = 5;

			Projectile.spriteDirection = player.Center.X < Projectile.Center.X ? -1 : 1;

			Vector2 pos = new Vector2(125, 0).RotatedBy(Parent.rotation + MathHelper.PiOver2);
			Projectile.Center = pos + new Vector2(Parent.Center.X, Parent.Center.Y - 5);
			Projectile.rotation = Parent.rotation;

			//shoot out stake
			if (Parent.ai[1] <= 4 && Parent.localAI[0] == 300)
			{
				SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);

				Vector2 ShootSpeed = player.Center - Projectile.Center;
				ShootSpeed.Normalize();
				ShootSpeed *= 55f;

				Vector2 Offset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 62f;
				Vector2 position = new Vector2(Projectile.Center.X, Projectile.Center.Y);

				if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
				{
					position += Offset;
				}

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, ShootSpeed, ModContent.ProjectileType<Stake>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
			}

			if (Parent.ai[1] > 4 && Parent.localAI[0] == 420)
			{
				Vector2 ShootSpeed = player.Center - Projectile.Center;
				ShootSpeed.Normalize();
				ShootSpeed *= 55f;

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<StakeLauncherSpin>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);

				Projectile.Kill();
			}
		}
	}
}
     
          






