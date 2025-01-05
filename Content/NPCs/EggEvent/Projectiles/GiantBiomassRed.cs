using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class GiantBiomassRed : ModProjectile
    {
        int ScaleTimerLimit = 10;
        float RotateSpeed = 0.2f;
        float ScaleAmount = 0.05f;

		private static Asset<Texture2D> GlowTexture;

		public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode2", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

		public override void SendExtraAI(BinaryWriter writer)
		{
			//ints
			writer.Write(ScaleTimerLimit);

			//floats
			writer.Write(RotateSpeed);
			writer.Write(ScaleAmount);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//ints
			ScaleTimerLimit = reader.ReadInt32();

			//floats
			RotateSpeed = reader.ReadSingle();
			ScaleAmount = reader.ReadSingle();
		}

		public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 70;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2000;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
        }

		public override void PostDraw(Color lightColor)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/GiantBiomassGlow");

			Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

			Vector2 drawPos = Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
			Rectangle rectangle = new(0, (GlowTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, GlowTexture.Width(), GlowTexture.Height() / Main.projFrames[Projectile.type]);
			Main.EntitySpriteDraw(GlowTexture.Value, drawPos, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
		}

		public override void AI()
        {
            Projectile.rotation += RotateSpeed * Projectile.direction;

            Projectile.alpha -= 3;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 90 || Projectile.ai[0] == 110 || Projectile.ai[0] == 130)
            {
                ScaleAmount += 0.025f;
                ScaleTimerLimit -= 2;
            }

            if (Projectile.ai[0] >= 70)
            {
                Projectile.velocity *= 0.95f;

                if (RotateSpeed >= 0)
                {
                    RotateSpeed -= 0.01f;
                }
                else
                {
                    RotateSpeed = 0f;
                }

                Projectile.ai[1]++;
                if (Projectile.ai[1] < ScaleTimerLimit)
                {
                    Projectile.scale -= ScaleAmount;
                }
                if (Projectile.ai[1] >= ScaleTimerLimit)
                {
                    Projectile.scale += ScaleAmount;
                }

                if (Projectile.ai[1] > ScaleTimerLimit * 2)
                {
                    Projectile.ai[1] = 0;
                    Projectile.scale = 1.5f;
                }
            }

            if (Projectile.ai[2] == 5 && Projectile.alpha <= 0)
            {
                Projectile.velocity.Y += 0.4f;
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[0] >= 150 && Projectile.ai[2] != 5)
            {
				Projectile.netUpdate = true;
				Projectile.Kill();
			}
        }

        public void NewNPC(int Type)
        {
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int NewEnemy = NPC.NewNPC(Projectile.GetSource_Death(), (int)Projectile.Center.X, (int)Projectile.Center.Y + Projectile.height / 2, Type);

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: NewEnemy);
				}
			}
		}

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(ExplosionSound, Projectile.Center);

            //spawn blood splatter
            for (int i = 0; i < 8; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-8, 9), Main.rand.Next(-8, 5), ModContent.ProjectileType<RedSplatter>(), 0, 0);
                }
            }

            //spawn blood explosion clouds
            for (int numExplosion = 0; numExplosion < 8; numExplosion++)
            {
                int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.65f, Main.rand.NextFloat(1f, 1.2f));
                Main.dust[DustGore].velocity.X *= Main.rand.NextFloat(-3f, 3f);
                Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(-3f, 0f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            //spawn vanilla blood dust
            for (int numDust = 0; numDust < 75; numDust++)
            {
                int newDust = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, DustID.Blood, 0f, 0f, 100, default, 1f);
                Main.dust[newDust].velocity.X *= Main.rand.Next(-12, 12);
                Main.dust[newDust].velocity.Y *= Main.rand.Next(-12, 12);
                Main.dust[newDust].scale *= Main.rand.NextFloat(1.8f, 2.5f);
                Main.dust[newDust].noGravity = true;

                if (Main.rand.NextBool(2))
                {
                    Main.dust[newDust].scale = 0.5f;
                    Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            //spawn enemy depending on what ai[2] is set to
            switch ((int)Projectile.ai[2])
            {
                case 0:
                {
                    NewNPC(ModContent.NPCType<HoppingHeart>());
                    break;
                }

                case 1:
                {
                    NewNPC(ModContent.NPCType<TongueBiter>());
                    break;
                }

                case 2:
                {
                    NewNPC(ModContent.NPCType<ExplodingAppendix>());
                    break;
                }

                case 3:
                {
                    NewNPC(ModContent.NPCType<CoughLungs>());
                    break;
                }

                case 4:
                {
                    NewNPC(ModContent.NPCType<HoverBrain>());
                    break;
                }

                case 5:
                {
                    NewNPC(ModContent.NPCType<FleshBolster>());
                    break;
                }
            }
		}
    }
}