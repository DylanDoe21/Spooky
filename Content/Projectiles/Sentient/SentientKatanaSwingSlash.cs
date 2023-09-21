using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{ 
    public class SentientKatanaSwingSlash : SwordSlashBase
    {
        public override string Texture => "Spooky/Content/Projectiles/SwordSlashCutter";

        float SaveKnockback;
        bool SavedKnockback = false;
        bool hasHitSomething = false;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlash(Projectile, lightColor);

            return true;
        }

        public override void CutTiles()
        {
            Vector2 vector2 = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 30f * Projectile.scale;
            Vector2 vector3 = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 30f * Projectile.scale;
            float num2 = 30f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + vector2, Projectile.Center + vector3, num2, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength2 = 40f * Projectile.scale;
            float Fade = (float)Math.PI * 2f / 25f * Projectile.ai[0];
            float maximumAngle2 = (float)Math.PI / 4f;
            float num4 = Projectile.rotation + Fade;
            if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength2, num4, maximumAngle2))
            {
                return true;
            }
            float num5 = Utils.Remap(Projectile.localAI[0], Projectile.ai[1] * 0.3f, Projectile.ai[1] * 0.5f, 1f, 0f);
            if (num5 > 0f)
            {
                float coneRotation2 = num4 - (float)Math.PI / 4f * Projectile.ai[0] * num5;
                if (targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, coneLength2, coneRotation2, maximumAngle2))
                {
                    return true;
                }
            }
            return false;
        }

        public void DrawSlash(Projectile proj, Color lightColor)
        {
            Vector2 vector = Projectile.Center - Main.screenPosition;
			Asset<Texture2D> Texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashCutter");
			Rectangle rectangle = Texture.Frame(1, 2);
			Vector2 origin = rectangle.Size() / 2f;
			SpriteEffects effects = (SpriteEffects)((!(Projectile.ai[0] >= 0f)) ? 2 : 0);
			float num2 = Projectile.localAI[0] / Projectile.ai[1];
			float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
			float amount = num3;
			Color color = Color.Lerp(Color.Red, Color.Blue, amount);
            Color color2 = Color.Lerp(Color.Blue, Color.Red, amount);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color2 * num3, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, 1.1f, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color * num3, Projectile.rotation, origin, 0.9f, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color2 * num3, Projectile.rotation, origin, 0.7f, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), color2 * 0.6f * num3, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, 1.1f, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), color * 0.5f * num3, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, 0.9f, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), color2 * 0.4f * num3, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, 0.7f, effects, 0f);
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

            if (!hasHitSomething)
            {
                hasHitSomething = true;

                float divide = 1.5f;

                Projectile.NewProjectile(player.GetSource_OnHit(target), Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, 
                ModContent.ProjectileType<SentientKatanaSlashSpawner>(), Projectile.damage / (int)divide, Projectile.knockBack, player.whoAmI, 0f, 0f);
            }
        }
    }
}