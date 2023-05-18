/*
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles
{
    public class SwordSlashBase : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ownerHitCheckDistance = 300f;
            Projectile.localNPCHitCooldown = 30;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.scale = 2f;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            Player player = Main.player[Projectile.owner];
            float num = Projectile.localAI[0] / Projectile.ai[1];
            float num2 = Projectile.ai[0];
            float num3 = Projectile.velocity.ToRotation();
            Projectile.rotation = (float)Math.PI * num2 * num + num3 + num2 * (float)Math.PI + player.fullRotation;
            float num5 = 1f;
            float num6 = 1.2f;

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) - Projectile.velocity;
            Projectile.scale = num6 + num * num5;

            float num8 = Projectile.rotation + Main.rand.NextFloatDirection() * ((float)Math.PI / 2f) * 0.7f;
            Vector2 vector2 = Projectile.Center + num8.ToRotationVector2() * 84f * Projectile.scale;
            Vector2 vector3 = (num8 + Projectile.ai[0] * ((float)Math.PI / 2f)).ToRotationVector2();

            if (Projectile.localAI[0] >= Projectile.ai[1])
            {
                Projectile.Kill();
            }
        }

        public override void CutTiles()
        {
            Vector2 vector2 = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
            Vector2 vector3 = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
            float num2 = 60f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + vector2, Projectile.Center + vector3, num2, DelegateMethods.CutTiles);
        }

        public override bool? CanCutTiles() 
        {
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength2 = 94f * Projectile.scale;
            float num3 = (float)Math.PI * 2f / 25f * Projectile.ai[0];
            float maximumAngle2 = (float)Math.PI / 4f;
            float num4 = Projectile.rotation + num3;
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
    }

    public class BladeTongueSlash : SwordSlashBase
    {
        public override string Texture => "Spooky/Content/Projectiles/SwordSlashBase";

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlash(Projectile);
            return true;
        }

        public void DrawSlash(Projectile proj)
        {
            Vector2 vector = proj.Center - Main.screenPosition;
            Asset<Texture2D> val = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashBase");
            Rectangle rectangle = val.Frame(1, 4);
            Vector2 origin = rectangle.Size() / 2f;
            float num = proj.scale * 1.1f;
            SpriteEffects effects = ((!(proj.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            float num2 = proj.localAI[0] / proj.ai[1];
            float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
            float num4 = 0.975f;
            float fromValue = Lighting.GetColor(proj.Center.ToTileCoordinates()).ToVector3().Length() / (float)Math.Sqrt(3.0);
            fromValue = Utils.Remap(fromValue, 0.2f, 1f, 0f, 1f);
            Main.spriteBatch.Draw(val.Value, vector, rectangle, new Color(199, 7, 49) * 0.4f * fromValue * num3, proj.rotation + proj.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - num2), origin, num, effects, 0f);
            Main.spriteBatch.Draw(val.Value, vector, rectangle, new Color(199, 7, 49) * 0.4f, proj.rotation + proj.ai[0] * 0.01f, origin, num * 0.6f, effects, 0f);
            Main.spriteBatch.Draw(val.Value, vector, rectangle, new Color(199, 7, 49) * 0.4f, proj.rotation, origin, num * 0.8f, effects, 0f);
            Main.spriteBatch.Draw(val.Value, vector, rectangle, new Color(199, 7, 49) * 0.4f, proj.rotation, origin, num * num4 * 0.6f, effects, 0f);
            Main.spriteBatch.Draw(val.Value, vector, val.Frame(1, 4, 0, 3), new Color(112, 11, 176) * 0.6f * num3, proj.rotation + proj.ai[0] * 0.01f, origin, num * 0.6f, effects, 0f);
            Main.spriteBatch.Draw(val.Value, vector, val.Frame(1, 4, 0, 3), new Color(112, 11, 176) * 0.5f * num3, proj.rotation + proj.ai[0] * -0.05f, origin, num * 0.8f * 0.6f, effects, 0f);
            Main.spriteBatch.Draw(val.Value, vector, val.Frame(1, 4, 0, 3), new Color(112, 11, 176) * 0.4f * num3, proj.rotation + proj.ai[0] * -0.1f, origin, num * 0.6f * 0.6f, effects, 0f);
            
            for (float num5 = 0f; num5 < 8f; num5++)
            {
                float num6 = proj.rotation + proj.ai[0] * num5 * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 4f) * proj.ai[0];
                Vector2 drawpos = vector + num6.ToRotationVector2() * (val.Value.Width * 0.5f - 6f) * num * 0.6f;
                float num7 = num5 / 9f;
            }

            Vector2 drawpos2 = vector + (proj.rotation + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 4f) * proj.ai[0]).ToRotationVector2() * (val.Value.Width * 0.5f - 4f) * num * 0.6f;
        }
    }
}
*/