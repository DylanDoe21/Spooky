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

namespace Spooky.Content.Projectiles.Cemetery
{ 
    public class SpiritSwordSlash : SwordSlashBase
    {
        public override string Texture => "Spooky/Content/Projectiles/SwordSlashBase";

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
            Vector2 vector2 = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 45f * Projectile.scale;
            Vector2 vector3 = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 45f * Projectile.scale;
            float num2 = 45f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + vector2, Projectile.Center + vector3, num2, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float coneLength2 = 60f * Projectile.scale;
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
			Asset<Texture2D> Texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashBase");
			Rectangle rectangle = Texture.Frame(1, 2);
			Vector2 origin = rectangle.Size() / 2f;
			float num = Projectile.scale * 0.8f;
			SpriteEffects effects = (SpriteEffects)((!(Projectile.ai[0] >= 0f)) ? 2 : 0);
			float num2 = Projectile.localAI[0] / Projectile.ai[1];
			float num3 = Utils.Remap(num2, 0f, 0.6f, 0f, 1f) * Utils.Remap(num2, 0.6f, 1f, 1f, 0f);
			float num4 = 0.975f;
			float amount = num3;
			Color color = Color.Lerp(Color.DarkViolet, Color.MediumPurple, amount);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color * num3, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - num2), origin, num, effects, 0f);
			Color color2 = Color.Lerp(Color.DarkViolet, Color.DarkViolet, amount);
			Color color3 = Color.Lerp(Color.DarkViolet, Color.Purple, amount);
			Color color4 = Color.Lerp(Color.DarkViolet, Color.Indigo, amount);
			color4.A = (byte)(float)(int)(color4.A * (1f));
			Color color5 = color4 * 0.5f;
			color5.G = (byte)((int)color5.G);
			color5.B = (byte)((int)color5.R * (0.25f * 0.75f));
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color5 * num3 * 0.3f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, num, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color3 * num3 * 0.3f, Projectile.rotation, origin, num, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color2 * num3 * 0.5f, Projectile.rotation, origin, num * num4, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), Color.DarkOrange * 0.6f * num3, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, num, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), Color.Orange * 0.5f * num3, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, num * 0.8f, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), Color.Gold * 0.4f * num3, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, num * 0.6f, effects, 0f);
			
            for (float num5 = 0f; num5 < 8f; num5 += 1f) 
            {
				float num6 = Projectile.rotation + Projectile.ai[0] * num5 * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0];
				Vector2 drawpos = vector + num6.ToRotationVector2() * ((float)Texture.Width() * 0.5f - 6f) * num;
				float num7 = num5 / 9f;
				DrawPrettyStarSparkle(Projectile.Opacity, (SpriteEffects)0, drawpos, Color.Gold, Color.Gold, num2, 0f, 0.5f, 0.5f, 1f, num6, new Vector2(0f, Utils.Remap(num2, 0f, 1f, 3f, 0f)) * num * 1.5f, Vector2.One * num * 1.5f);
			
                Vector2 drawpos2 = vector + (Projectile.rotation + Utils.Remap(num2, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0]).ToRotationVector2() * ((float)Texture.Width() * 0.5f - 4f) * num;
			    DrawPrettyStarSparkle(Projectile.Opacity, (SpriteEffects)0, drawpos2, Color.Gold, Color.Gold, num2, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(num2, 0f, 1f, 4f, 1f)) * num * 1.5f, Vector2.One * num * 1.5f);
            }
        }

        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) 
        {
			Texture2D value = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = value.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float num = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * num;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * num;
			color *= num;
			color2 *= num;
			Main.EntitySpriteDraw(value, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
			Main.EntitySpriteDraw(value, drawpos, null, color, 0f + rotation, origin, vector2, dir);
			Main.EntitySpriteDraw(value, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(value, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
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

                SoundEngine.PlaySound(SoundID.Item69, player.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center.X, target.Center.Y, 
                    Main.rand.Next(-2, 2), Main.rand.Next(-7, -5), ModContent.ProjectileType<SpookySkull>(), Projectile.damage, 0, Main.myPlayer, 0, 0);
                }
            }
        }
    }
}