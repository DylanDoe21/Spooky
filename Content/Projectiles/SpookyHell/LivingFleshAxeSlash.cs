using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{ 
    public class LivingFleshAxeSlash : SwordSlashBase
    {
        public override string Texture => "Spooky/Content/Projectiles/SwordSlashSpecial";

        float SaveKnockback;
        bool SavedKnockback = false;
        bool hasHitSomething = false;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlash(Projectile, lightColor);

            return false;
        }

        public void DrawSlash(Projectile proj, Color lightColor)
        {
            Vector2 vector = proj.Center - Main.screenPosition;
            Asset<Texture2D> Texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashSpecial");
            Rectangle rectangle = Texture.Frame(1, 2);
            Vector2 origin = rectangle.Size() / 2f;
            float Scale = proj.scale * 0.98f;
            SpriteEffects effects = ((!(proj.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            float CurrentAI = proj.localAI[0] / proj.ai[1];
            float Intensity = Utils.Remap(CurrentAI, 0f, 0.6f, 0f, 1f) * Utils.Remap(CurrentAI, 0.6f, 1f, 1f, 0f);

            //these are the slash textures themselves
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, Color.Blue * Intensity * 0.75f, proj.rotation, origin, Scale * 1.5f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, Color.Crimson * Intensity * 0.75f, proj.rotation, origin, Scale * 0.75f, effects, 0f);

            //draw extra lines on top of each slash
            Main.spriteBatch.Draw(Texture.Value, vector, Texture.Frame(1, 2, 0, 1), Color.Gray * Intensity * 0.75f, proj.rotation, origin, Scale * 1.5f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, Texture.Frame(1, 2, 0, 1), Color.Gray * Intensity * 0.75f, proj.rotation, origin, Scale * 0.75f, effects, 0f);
        }

        public override void CutTiles()
        {
            Vector2 VectorX = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 85f * Projectile.scale;
            Vector2 VectorY = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 85f * Projectile.scale;
            float Distance = 85f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + VectorX, Projectile.Center + VectorY, Distance, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float Length = 128f * Projectile.scale;
            float Fade = (float)Math.PI * 2f / 25f * Projectile.ai[0];
            float MaxAngle = (float)Math.PI / 4f;
            float ActualFade = Projectile.rotation + Fade;
            if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, Length, ActualFade, MaxAngle))
            {
                return true;
            }

            float AIRemap = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.3f, Projectile.ai[1] * 0.5f, 1f, 0f);
            if (AIRemap > 0f)
            {
                float Rotation = ActualFade - (float)Math.PI / 4f * Projectile.ai[0] * AIRemap;
                if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, Length, Rotation, MaxAngle))
                {
                    return true;
                }
            }
            
            return false;
        }

        public override bool PreAI()
		{
            if (!SavedKnockback)
            {
                SaveKnockback = Projectile.knockBack;
                SavedKnockback = true;
            }
            else
            {
                Projectile.knockBack = 0;
            }

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            Player player = Main.player[Projectile.owner];

            Projectile.damage = (int)(damageDone * 0.8f);

            //since this projectile is weird and only knocks enemies back in one direction, manually handle knockback here
            Vector2 Knockback = player.Center - target.Center;
            Knockback.Normalize();
            Knockback *= SaveKnockback * 2;

            if (target.knockBackResist > 0)
            {
                target.velocity = -Knockback * target.knockBackResist;
            }

            if (target.life <= target.lifeMax * 0.5 && !target.boss)
            {
                target.takenDamageMultiplier = 1.5f;
            }

            if (!hasHitSomething)
            {
                hasHitSomething = true;

                SoundEngine.PlaySound(SoundID.NPCDeath21, player.Center);

                for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center.X, target.Center.Y, Main.rand.Next(-5, 6), Main.rand.Next(-15, -10), 
                        ModContent.ProjectileType<LivingFleshAxeEye>(), Projectile.damage, 2f, Main.myPlayer);
                    }
                }
            }
        }
    }
}