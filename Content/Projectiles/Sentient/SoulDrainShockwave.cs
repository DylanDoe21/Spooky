using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SoulDrainShockwave : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle ExplosionSound = new("Spooky/Content/Sounds/EggEvent/BiomassExplode1", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
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

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);

            for (int i = 0; i < 360; i += 90)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 1f), Main.rand.NextFloat(1f, 1f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, vector + circular, rectangle, color, Projectile.rotation, drawOrigin, Projectile.ai[0] / 47, SpriteEffects.None, 0);
            }

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

            if (Projectile.ai[0] < 450)
            {
                if (Projectile.ai[0] == 0)
                {
                    SoundEngine.PlaySound(ExplosionSound, Projectile.Center);
                }

                Projectile.ai[0] += 25;
            }
            else
            {
                Projectile.alpha += 20;

                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int target = 0; target < Main.maxNPCs; target++)
            {
                NPC npc = Main.npc[target];

                if (npc.Distance(Projectile.Center) <= Projectile.ai[0] && npc.active && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type])
                {
                    Player player = Main.player[Projectile.owner];

                    //damage enemies
                    player.ApplyDamageToNPC(npc, Projectile.damage, 0, 0, false, null, true);

                    //push all enemies away from you
                    Vector2 Knockback = Projectile.Center - npc.Center;
                    Knockback.Normalize();
                    Knockback *= 10;

                    if (npc.knockBackResist > 0)
                    {
                        npc.velocity = -Knockback;
                    }
                }
            }
        }
    }
}