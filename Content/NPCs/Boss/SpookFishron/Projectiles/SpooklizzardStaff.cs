using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Spooky.Content.NPCs.Boss.SpookFishron.Projectiles
{
    public class SpooklizzardStaff : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
			writer.Write(Projectile.localAI[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			//floats
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
			Projectile.localAI[2] = reader.ReadSingle();
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

				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/SpooklizzardStaff").Type);
				}
			}

            if (Parent.active && Parent.type == ModContent.NPCType<SpookFishron>() && Parent.ai[0] != 6)
            {
                Projectile.Kill();

				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/SpooklizzardStaff").Type);
				}
            }

			Projectile.timeLeft = 5;

			Projectile.spriteDirection = player.Center.X < Projectile.Center.X ? -1 : 1;

			Vector2 pos = new Vector2(125, 0).RotatedBy(Parent.rotation + MathHelper.PiOver2);
			Projectile.Center = pos + new Vector2(Parent.Center.X, Parent.Center.Y - 2);
			Projectile.rotation = Parent.rotation;

			//shoot out icicles
			if (Parent.ai[1] <= 2 && Parent.localAI[0] % 10 == 0)
			{
				int num33 = 2;
				for (int num34 = 0; num34 < num33; num34++)
				{
					Vector2 pointPoisition = new Vector2(player.position.X + (float)player.width * 0.5f + (float)(Main.rand.Next(201) * -player.direction), player.MountedCenter.Y - 600f);
					pointPoisition.X = (pointPoisition.X + player.Center.X) / 2f + (float)Main.rand.Next(-200, 201);
					pointPoisition.Y -= 100 * num34;
					float num2 = player.MountedCenter.X- pointPoisition.X;
					float num3 = player.MountedCenter.Y - pointPoisition.Y;
					if (player.gravDir == -1f)
					{
						num3 = (float)Main.screenHeight - player.MountedCenter.Y - pointPoisition.Y;
					}
					if (num3 < 0f)
					{
						num3 *= -1f;
					}
					if (num3 < 20f)
					{
						num3 = 20f;
					}
					float num4 = (float)Math.Sqrt(num2 * num2 + num3 * num3);
					num4 = 55f / num4;
					num2 *= num4;
					num3 *= num4;
					float speedX5 = num2 + (float)Main.rand.Next(-40, 41) * 0.02f;
					float speedY7 = num3 + (float)Main.rand.Next(-40, 41) * 0.02f;
					Projectile.NewProjectile(Projectile.GetSource_FromAI(), pointPoisition.X, pointPoisition.Y, speedX5, speedY7, ProjectileID.FrostShard, Projectile.damage, Projectile.knockBack, Projectile.owner, 0f);
				}
			}

			if (Parent.ai[1] > 2 && Parent.localAI[0] == 420)
			{
				Vector2 ShootSpeed = player.Center - Projectile.Center;
				ShootSpeed.Normalize();
				ShootSpeed *= 55f;

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<SpooklizzardStaffSpin>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);

				Projectile.netUpdate = true;

				Projectile.Kill();
			}
		}
	}
}
     
          






