using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class PeptoBubble : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (Main.rand.NextBool(10))
			{
				target.AddBuff(ModContent.BuffType<PeptoDebuff>(), int.MaxValue);
			}
		}

		public override void AI()
        {   
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 2)
                {
                    Projectile.frame = 0;
                }
            }

            Projectile.rotation += 0.35f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y - 0.02f;
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Item54, Projectile.Center);

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CauldronBubble>(), 0f, -2f, 0, default, 1f);
                Main.dust[newDust].color = Color.DeepPink;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                Main.dust[newDust].noGravity = true;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 1.2f;
				}
			}
		}
    }
}