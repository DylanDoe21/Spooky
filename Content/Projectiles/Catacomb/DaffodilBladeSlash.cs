using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{ 
    public class DaffodilBladeSlash : SwordSlashBase
    {
        public override string Texture => "Spooky/Content/Projectiles/SwordSlashBase";

        float SaveKnockback;
        bool SavedKnockback = false;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlash(Projectile, lightColor);

            return true;
        }

        public override void CutTiles()
        {
            Vector2 vector2 = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 42f * Projectile.scale;
            Vector2 vector3 = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 42f * Projectile.scale;
            float num2 = 42f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + vector2, Projectile.Center + vector3, num2, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength2 = 52f * Projectile.scale;
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
            Vector2 vector = proj.Center - Main.screenPosition;
            Asset<Texture2D> Texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashBase");
            Rectangle rectangle = Texture.Frame(1, 4);
            Vector2 origin = rectangle.Size() / 2f;
            float num = proj.scale * 0.75f;
            SpriteEffects effects = ((!(proj.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            float num2 = proj.localAI[0] / proj.ai[1];
            float Fade = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
            float num4 = 0.975f;
            float fromValue = Lighting.GetColor(proj.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);

            //these are the slash textures themselves
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, new Color(140, 184, 0) * Fade * 0.65f, proj.rotation, origin, num * 0.85f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, new Color(165, 91, 2) * Fade * 0.6f, proj.rotation, origin, num * num4 * 0.7f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, new Color(229, 26, 0) * Fade * 0.5f, proj.rotation, origin, num * num4 * 0.5f, effects, 0f);

            for (float num5 = 0f; num5 < 8f; num5++)
            {
                float num6 = proj.rotation + proj.ai[0] * num5 * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 4f) * proj.ai[0];
                Vector2 drawpos = vector + num6.ToRotationVector2() * (Texture.Value.Width * 0.5f - 6f) * num * 0.6f;
                float num7 = num5 / 9f;
            }

            Vector2 drawpos2 = vector + (proj.rotation + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 4f) * proj.ai[0]).ToRotationVector2() * (Texture.Value.Width * 0.5f - 4f) * num * 1.2f;
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

            //since this projectile is weird and only knocks enemies back in one direction, manually handle knockback here
            Vector2 Knockback = player.Center - target.Center;
            Knockback.Normalize();
            Knockback *= SaveKnockback * 2;

            if (target.knockBackResist > 0)
            {
                target.velocity = -Knockback * target.knockBackResist;
            }

            if (Main.rand.NextBool(8))
            {
                target.AddBuff(BuffID.Poisoned, 300);
            }
        }
    }
}