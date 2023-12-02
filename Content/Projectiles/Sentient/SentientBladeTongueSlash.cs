using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Drawing;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{ 
    public class TonguebladeSlash : SwordSlashBase
    {
        public override string Texture => "Spooky/Content/Projectiles/SwordSlashBase";

        float SaveKnockback;
        bool SavedKnockback = false;

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlash(Projectile, lightColor);

            return false;
        }

        public void DrawSlash(Projectile proj, Color lightColor)
        {
            Vector2 vector = proj.Center - Main.screenPosition;
            Asset<Texture2D> Texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashBase");
            Rectangle rectangle = Texture.Frame(1, 2);
            Vector2 origin = rectangle.Size() / 2f;
            float Scale = proj.scale * 1.75f;
            SpriteEffects effects = ((!(proj.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
            float CurrentAI = proj.localAI[0] / proj.ai[1];
            float Intensity = Utils.Remap(CurrentAI, 0f, 0.6f, 0f, 1f) * Utils.Remap(CurrentAI, 0.6f, 1f, 1f, 0f);

            //these are the slash textures themselves
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, Color.Red * Intensity * 0.4f, proj.rotation + proj.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - CurrentAI), origin, Scale, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, Color.Red * Intensity * 0.35f, proj.rotation + proj.ai[0] * 0.01f, origin, Scale * 1.2f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, Color.Red * Intensity * 0.3f, proj.rotation, origin, Scale * 0.8f, effects, 0f);
            Main.spriteBatch.Draw(Texture.Value, vector, rectangle, Color.Red * Intensity * 0.25f, proj.rotation, origin, Scale * 0.975f * 1.2f, effects, 0f);
        }

        public override void CutTiles()
        {
            Vector2 VectorX = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 120f * Projectile.scale;
            Vector2 VectorY = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 120f * Projectile.scale;
            float Distance = 120f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + VectorX, Projectile.Center + VectorY, Distance, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float Length = 160f * Projectile.scale;
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
                target.AddBuff(BuffID.Confused, 180);
            }

            for (int numDusts = 0; numDusts < 30; numDusts++)
			{
                int dustGore = Dust.NewDust(target.position, target.width, target.height, 103, 0f, -2f, 0, default, 1.5f);
                Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-10f, 10f);
                Main.dust[dustGore].noGravity = true;
            }
        }
    }
}