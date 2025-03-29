using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.NPCs.Catacomb;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
    public class FlamingWisp : ModProjectile
    {
        int Offset = Main.rand.Next(-100, 100);

        private static Asset<Texture2D> AfterImageTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
			Projectile.height = 26;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 360;
            Projectile.alpha = 255;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            AfterImageTexture ??= TextureAssets.Extra[98];
            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Color color1 = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.OrangeRed);
            Color color2 = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

            float TrailRotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			TrailRotation += 0f * Projectile.direction;

			for (int oldPos = 1; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                Color newColor = Color.Lerp(color1, color2, oldPos / (float)Projectile.oldPos.Length) * 0.65f * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                newColor = Projectile.GetAlpha(newColor);
                newColor *= 1f;

				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                
                for (int repeats = 0; repeats < 2; repeats++)
                {
                    Main.EntitySpriteDraw(AfterImageTexture.Value, drawPos, null, newColor, TrailRotation, AfterImageTexture.Size() / 2f, scale, SpriteEffects.None);
                }
            }

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

        public override void AI()
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;

            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0f);

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 8;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 75)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
            }

            if (Projectile.ai[0] < 75)
            {
                NPC Parent = Main.npc[(int)Projectile.ai[1]];

                if (Parent.active && (Parent.type == ModContent.NPCType<BigBone>() || Parent.type == ModContent.NPCType<CatacombGuardian>()))
                {
                    float goToX = Parent.Center.X + Offset - Projectile.Center.X;
                    float goToY = Parent.Center.Y + Offset - Projectile.Center.Y;
                    float speed = 0.12f;

                    if (Projectile.velocity.X < goToX)
                    {
                        Projectile.velocity.X = Projectile.velocity.X + speed;
                        if (Projectile.velocity.X < 0f && goToX > 0f)
                        {
                            Projectile.velocity.X = Projectile.velocity.X + speed;
                        }
                    }
                    else if (Projectile.velocity.X > goToX)
                    {
                        Projectile.velocity.X = Projectile.velocity.X - speed;
                        if (Projectile.velocity.X > 0f && goToX < 0f)
                        {
                            Projectile.velocity.X = Projectile.velocity.X - speed;
                        }
                    }
                    if (Projectile.velocity.Y < goToY)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y + speed;
                        if (Projectile.velocity.Y < 0f && goToY > 0f)
                        {
                            Projectile.velocity.Y = Projectile.velocity.Y + speed;
                            return;
                        }
                    }
                    else if (Projectile.velocity.Y > goToY)
                    {
                        Projectile.velocity.Y = Projectile.velocity.Y - speed;
                        if (Projectile.velocity.Y > 0f && goToY < 0f)
                        {
                            Projectile.velocity.Y = Projectile.velocity.Y - speed;
                            return;
                        }
                    }
                }
            }

            if (Projectile.ai[0] == 75)
            {
                Projectile.tileCollide = true;
                
                Vector2 Speed = new Vector2(12f, 0f).RotatedByRandom(2 * Math.PI);
                Vector2 newVelocity = Speed.RotatedBy(2 * Math.PI / 2 * (Main.rand.NextDouble() - 0.5));
                Projectile.velocity = newVelocity;
            }
        }

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);
        
        	if (Main.netMode != NetmodeID.Server) 
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/FlamingWispGore").Type);
            }
		}
    }
}