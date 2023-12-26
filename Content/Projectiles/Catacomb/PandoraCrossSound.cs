using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class PandoraCrossSound : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 64;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6.28318548f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, tex.Height / Main.projFrames[Projectile.type] * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(tex, vector, rectangle, Color.Cyan, Projectile.rotation, drawOrigin, Projectile.ai[0] / 37, SpriteEffects.None, 0);

            return true;
        }

        public override bool? CanDamage()
        {
			return false;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }
    
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.position = new Vector2(player.Center.X - Projectile.width / 2, player.Center.Y - Projectile.height / 2);

            if (Projectile.ai[0] < 350)
            {
                Projectile.ai[0] += 50;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            for (int target = 0; target < Main.maxNPCs; target++)
            {
                NPC npc = Main.npc[target];

                if (npc.Distance(Projectile.Center) <= Projectile.ai[0] && npc.active && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type])
                {
                    //damage enemies
                    player.ApplyDamageToNPC(npc, Projectile.damage * 2, 0, 0, false);

                    //push all enemies away from you
                    Vector2 Knockback = Projectile.Center - npc.Center;
                    Knockback.Normalize();
                    Knockback *= 6;

                    if (npc.knockBackResist > 0)
                    {
                        npc.velocity = -Knockback;
                    }
                }
            }
        }
    }
}