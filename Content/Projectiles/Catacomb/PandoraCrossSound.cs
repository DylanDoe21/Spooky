using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class PandoraCrossSound : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(Color.Cyan), Projectile.rotation, drawOrigin, Projectile.ai[0] / 47, SpriteEffects.None, 0);

            return false;
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

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - Projectile.height / 2);

            if (Projectile.ai[0] < 350)
            {
                Projectile.ai[0] += 25;
            }
            else
            {
                Projectile.alpha += 50;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
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