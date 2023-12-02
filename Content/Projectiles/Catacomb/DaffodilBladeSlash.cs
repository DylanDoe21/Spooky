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
            Vector2 vector = Projectile.Center - Main.screenPosition;
            Asset<Texture2D> Texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashBase");
            Rectangle rectangle = Texture.Frame(1, 2);
            Vector2 origin = rectangle.Size() / 2f;
            float Scale = Projectile.scale * 1.44f;
            SpriteEffects effects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            float CurrentAI = Projectile.localAI[0] / Projectile.ai[1];
            float Intensity = Utils.Remap(CurrentAI, 0f, 0.6f, 0f, 1f) * Utils.Remap(CurrentAI, 0.6f, 1f, 1f, 0f);

            //these are the slash textures themselves
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, new Color(140, 184, 0) * Intensity * 0.65f, Projectile.rotation, origin, Scale * 0.85f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, new Color(165, 91, 2) * Intensity * 0.57f, Projectile.rotation, origin, Scale * 0.7f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, new Color(229, 26, 0) * Intensity * 0.5f, Projectile.rotation, origin, Scale * 0.5f, effects, 0f);

            return false;
        }

        public override void CutTiles()
        {
            Vector2 VectorX = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 70f * Projectile.scale;
            Vector2 VectorY = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 70f * Projectile.scale;
            float Distance = 70f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + VectorX, Projectile.Center + VectorY, Distance, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float Length = 95f * Projectile.scale;
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