using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.SpookFishron.Projectiles
{
    public class SpookyTornado : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 162;
            Projectile.height = 42;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
			if (Main.snowMoon)
            {
                ProjTexture = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/SpookFishron/Projectiles/FrostMoonTextures/SpookyTornado");
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

				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lerp(color2, color1, Projectile.ai[1] / 20f));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

		public override bool? CanDamage()
		{
			return Projectile.alpha <= 0;
		}

        public override void AI()
        {
			int TornadoHeight1 = (int)Projectile.ai[2];
			int TornadoHeight2 = (int)Projectile.ai[2];
			float ScaleAmount = 1.5f;
			int BaseWidth = 162;
			int BaseHeight = 42;

			Projectile.frameCounter++;
			if (Projectile.frameCounter > 2)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame >= 6)
			{
				Projectile.frame = 0;
			}
			
			if (Projectile.localAI[0] == 0f)
			{
				Projectile.position.X += Projectile.width / 2;
				Projectile.position.Y += Projectile.height / 2;
				Projectile.scale = ((float)(TornadoHeight1 + TornadoHeight2) - Projectile.ai[1]) * ScaleAmount / (float)(TornadoHeight2 + TornadoHeight1);
				Projectile.width = (int)((float)BaseWidth * Projectile.scale);
				Projectile.height = (int)((float)BaseHeight * Projectile.scale);
				Projectile.position.X -= Projectile.width / 2;
				Projectile.position.Y -= Projectile.height / 2;
				Projectile.width = (int)((float)BaseWidth * Projectile.scale);
				Projectile.localAI[0] = 1f;
				Projectile.netUpdate = true;
			}

			if (Projectile.timeLeft > 60 && Projectile.alpha > 0)
			{
				Projectile.alpha -= 10;
			}
			if (Projectile.timeLeft < 60)
			{
				Projectile.alpha += 5;

				if (Projectile.alpha >= 255)
				{
					Projectile.Kill();
				}
			}
			
			if (Projectile.ai[0] > 0f)
			{
				Projectile.ai[0]--;
			}
			
			if (Projectile.ai[0] == 1f && Projectile.ai[1] > 0f)
			{
				Projectile.netUpdate = true;
				Vector2 center4 = Projectile.Center;
				center4.Y -= (float)BaseHeight * Projectile.scale / 2f;
				float num540 = ((float)(TornadoHeight1 + TornadoHeight2) - Projectile.ai[1] + 1f) * ScaleAmount / (float)(TornadoHeight2 + TornadoHeight1);
				center4.Y -= (float)BaseHeight * num540 / 2f;
				center4.Y += 2f;

				Projectile.NewProjectile(Projectile.GetSource_FromAI(), center4.X, center4.Y, Projectile.velocity.X, Projectile.velocity.Y, 
				Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 10f, Projectile.ai[1] - 1f, Projectile.ai[2]);

				if ((int)Projectile.ai[1] % 3 == 0 && Projectile.ai[1] != 0f)
				{
					//spawn a sharkron
					int Sharkron = NPC.NewNPC(Projectile.GetSource_FromAI(), (int)center4.X + 18, (int)center4.Y + 20, ModContent.NPCType<SpookSharkron>());
					Main.npc[Sharkron].velocity = Projectile.velocity;
					Main.npc[Sharkron].netUpdate = true;
					Main.npc[Sharkron].alpha = 255;
					Main.npc[Sharkron].ai[2] = Projectile.width;
				}
			}

			if (Projectile.ai[0] <= 0f)
			{
				float num544 = (float)Math.PI / 30f;
				float num545 = (float)Projectile.width / 5f;
				float num546 = (float)(Math.Cos(num544 * (0f - Projectile.ai[0])) - 0.5) * num545;
				Projectile.position.X -= num546 * (float)(-Projectile.direction);
				Projectile.ai[0]--;
				num546 = (float)(Math.Cos(num544 * (0f - Projectile.ai[0])) - 0.5) * num545;
				Projectile.position.X += num546 * (float)(-Projectile.direction);
			}
        }
    }
}