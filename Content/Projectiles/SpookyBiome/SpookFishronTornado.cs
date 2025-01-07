using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class SpookFishronTornado : ModProjectile
    {
		public override string Texture => "Spooky/Content/NPCs/Boss/SpookFishron/Projectiles/SpookyTornado";

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 162;
            Projectile.height = 42;
			Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X - Projectile.scale * 0.5f, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
			
			for (int i = 0; i < 360; i += 90)
            {
				Color color1 = new Color(222, 133, 51, 0);
				Color color2 = new Color(206, 96, 40, 0);

				Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lerp(color2, color1, Projectile.ai[1] / 20f));

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 3f), Main.rand.NextFloat(1f, 3f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return false;
        }

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];

            int TornadoHeight1 = 8;
			int TornadoHeight2 = 8;
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
			
			if (Projectile.localAI[0] == 0f && Main.myPlayer == Projectile.owner)
			{
				Projectile.localAI[0] = 1f;
				Projectile.position.X += Projectile.width / 2;
				Projectile.position.Y += Projectile.height / 2;
				Projectile.scale = ((float)(TornadoHeight1 + TornadoHeight2) - Projectile.ai[1]) * ScaleAmount / (float)(TornadoHeight2 + TornadoHeight1);
				Projectile.width = (int)((float)BaseWidth * Projectile.scale);
				Projectile.height = (int)((float)BaseHeight * Projectile.scale);
				Projectile.position.X -= Projectile.width / 2;
				Projectile.position.Y -= Projectile.height / 2;
				Projectile.width = (int)((float)BaseWidth * Projectile.scale);
				Projectile.netUpdate = true;
			}

			if (Projectile.ai[1] != -1f)
            {
                Projectile.scale = (TornadoHeight1 + TornadoHeight2 - Projectile.ai[1]) * ScaleAmount / (TornadoHeight2 + TornadoHeight1);
                Projectile.width = (int)(BaseWidth * Projectile.scale);
                Projectile.height = (int)(BaseHeight * Projectile.scale);
            }

			if (Projectile.timeLeft > 60 && Projectile.alpha > 0)
			{
				Projectile.alpha -= 35;
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
			
			if (Projectile.ai[0] == 1f && Projectile.ai[1] > 0f && Projectile.owner == Main.myPlayer)
			{
				Projectile.netUpdate = true;
				Vector2 ProjCenter = Projectile.Center;
				ProjCenter.Y -= (float)BaseHeight * Projectile.scale / 2f;
				float num540 = ((float)(TornadoHeight1 + TornadoHeight2) - Projectile.ai[1] + 1f) * ScaleAmount / (float)(TornadoHeight2 + TornadoHeight1);
				ProjCenter.Y -= (float)BaseHeight * num540 / 2f;
				ProjCenter.Y += 6f;

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), ProjCenter.X, ProjCenter.Y, Projectile.velocity.X, Projectile.velocity.Y, 
				Type, Projectile.damage, Projectile.knockBack, Projectile.owner, 10f, Projectile.ai[1] - 1f, Projectile.whoAmI);
			}

			//only segments above the bottom should follow the bottom segments velocity
			if (Projectile.ai[1] < 7f)
			{
				Projectile.velocity = Main.projectile[(int)Projectile.ai[2]].velocity;
			}

			if (Projectile.ai[0] <= 0f)
			{
				float num544 = (float)Math.PI / 30f;
				float Intensity = (float)Projectile.width / 5f;
				float PositionShift = (float)(Math.Cos(num544 * (0f - Projectile.ai[0])) - 0.5) * Intensity;
				Projectile.position.X -= PositionShift;
				Projectile.ai[0]--;
				PositionShift = (float)(Math.Cos(num544 * (0f - Projectile.ai[0])) - 0.5) * Intensity;
				Projectile.position.X += PositionShift;
			}
        }
    }
}