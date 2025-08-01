using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class ControllableEyeBig : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
			Projectile.height = 14;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 200;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Blue) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return Projectile.ai[0] > 0;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += 0.35f * (float)Projectile.direction;

            if (player.channel && Projectile.ai[0] == 0) 
            {
                Projectile.timeLeft = 200;

                if (ItemGlobal.ActiveItem(player).type != ModContent.ItemType<LivingFleshStaff>())
                {
                    Projectile.Kill();
                }

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(Main.MouseWorld) * 32;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
            }
            else
            {
                Projectile.ai[0]++;

                if (Projectile.ai[0] == 1)
                {
                    if (player.ownedProjectileCounts[ModContent.ProjectileType<ControllableEyeBig>()] >= 8)
                    {
                        Projectile.Kill();
                    }
                    else
                    {
                        Projectile.damage *= 2;
                    }
                }

                if (Projectile.ai[0] >= 25)
                {
                    Projectile.velocity.X = Projectile.velocity.X * 0.97f;
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;
                }
            }

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.45f;
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.X += pushAway;
                    }
                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.Y += pushAway;
                    }
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.LocalPlayer.ownedProjectileCounts[ModContent.ProjectileType<ControllableEyeBig>()] >= 8)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.Center);

                for (int numDusts = 0; numDusts < 15; numDusts++)
                {                                                                                  
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, 0f, -2f, 0, default, Main.rand.NextFloat(1f, 2f));
                    Main.dust[dust].velocity.X *= Main.rand.NextFloat(-18f, 18f);
                    Main.dust[dust].velocity.Y *= Main.rand.NextFloat(-18f, 18f);
                    Main.dust[dust].noGravity = true;
                }

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<ControllableEyeBigExplosion>(), Projectile.damage * 5, Projectile.knockBack, Main.myPlayer);
            }
            else
            {
                for (int numDusts = 0; numDusts < 10; numDusts++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[dust].velocity *= 1.5f;
                    Main.dust[dust].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dust].scale = 0.5f;
                        Main.dust[dust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
	}
}