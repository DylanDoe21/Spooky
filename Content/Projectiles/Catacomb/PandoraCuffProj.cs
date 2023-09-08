using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class PandoraCuffProj : ModProjectile
    {
        public bool IsStickingToTarget = false;

        //TODO: find a cool metal clinking sound for when it attaches to an enemy
        //public static readonly SoundStyle BiteSound = new("Spooky/Content/Sounds/Bite", SoundType.Sound) { PitchVariance = 0.7f };

        public override void SetDefaults()
        {
			Projectile.width = 36;
            Projectile.height = 42;     
			Projectile.friendly = true;                              			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;                 					
            Projectile.timeLeft = 30;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            if (!IsStickingToTarget)
            {
                Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

                Color color = new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0).MultiplyRGBA(Color.DeepPink);

                Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

                float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

                for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
                {
                    Color newColor = color;
                    newColor = Projectile.GetAlpha(newColor);
                    newColor *= 1f;

                    var effects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                    Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale + (fade / 2), effects, 0);
                }
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsStickingToTarget && target.type == (int)Projectile.ai[0])
            {
                Projectile.ai[1] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
        }
		
		public override void AI()
        {
            Projectile.timeLeft = 30;

            if (IsStickingToTarget) 
            {
				Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                int npcTarget = (int)Projectile.ai[1];
                if (npcTarget < 0 || npcTarget >= 200) 
                {
                    Projectile.Kill();
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
                {
                    Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                }
                else 
                {
                    Projectile.Kill();
                }
			}
			else 
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 4)
                    {
                        Projectile.frame = 0;
                    }
                }

				Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
                Projectile.rotation = Projectile.velocity.ToRotation();

                if (Projectile.spriteDirection == -1)
                {
                    Projectile.rotation += MathHelper.Pi;
                }
			}
		}
    }
}