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
            Vector2 vector = Projectile.Center - Main.screenPosition;
			Asset<Texture2D> Texture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/SwordSlashSpecial");
			Rectangle rectangle = Texture.Frame(1, 2);
			Vector2 origin = rectangle.Size() / 2f;
			float Scale = Projectile.scale * 1.44f;
			SpriteEffects effects = ((!(proj.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
			float CurrentAI = Projectile.localAI[0] / Projectile.ai[1];
			float Intensity = Utils.Remap(CurrentAI, 0f, 0.6f, 0f, 1f) * Utils.Remap(CurrentAI, 0.6f, 1f, 1f, 0f);
			Color color = Color.Lerp(Color.DarkViolet, Color.MediumPurple, Intensity);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, color * Intensity, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - CurrentAI), origin, Scale, effects, 0f);
			Color SlashColor1 = Color.Lerp(Color.DarkViolet, Color.Indigo, Intensity);
            Color SlashColor2 = Color.Lerp(Color.DarkViolet, Color.Purple, Intensity);
            Color SlashColor3 = Color.Lerp(Color.DarkViolet, Color.DarkViolet, Intensity);

            //these are the slash textures themselves
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, SlashColor1 * Intensity * 0.3f, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, Scale, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, SlashColor2 * Intensity * 0.3f, Projectile.rotation, origin, Scale, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)rectangle, SlashColor3 * Intensity * 0.5f, Projectile.rotation, origin, Scale * 0.975f, effects, 0f);
			
            //draw extra lines on top of each slash
            Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), Color.DarkOrange * 0.6f * Intensity, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, Scale, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), Color.Orange * 0.5f * Intensity, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, Scale * 0.8f, effects, 0f);
			Main.spriteBatch.Draw(Texture.Value, vector, (Rectangle?)Texture.Frame(1, 2, 0, 1), Color.Gold * 0.4f * Intensity, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, Scale * 0.6f, effects, 0f);
			
            //draw star sparkle and trail on top of the slashes
            for (float Repeats = 0f; Repeats < 8f; Repeats += 1f) 
            {
				float Fade = Projectile.rotation + Projectile.ai[0] * Repeats * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(CurrentAI, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0];
				Vector2 drawpos = vector + Fade.ToRotationVector2() * ((float)Texture.Width() * 0.5f - 6f) * Scale * 1.02f;
				DrawPrettyStarSparkle(Projectile.Opacity, (SpriteEffects)0, drawpos, Color.Gold, Color.Gold, CurrentAI, 0f, 0.5f, 0.5f, 1f, Fade, new Vector2(0f, Utils.Remap(CurrentAI, 0f, 1f, 3f, 0f)) * Scale * 0.75f, Vector2.One * Scale * 0.75f);
			
                Vector2 drawpos2 = vector + (Projectile.rotation + Utils.Remap(CurrentAI, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0]).ToRotationVector2() * ((float)Texture.Width() * 0.5f - 4f) * Scale;
			    DrawPrettyStarSparkle(Projectile.Opacity, (SpriteEffects)0, drawpos2, Color.Gold, Color.Gold, CurrentAI, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(CurrentAI, 0f, 1f, 4f, 1f)) * Scale * 0.75f, Vector2.One * Scale * 0.75f);
            }
        }

        private static void DrawPrettyStarSparkle(float opacity, SpriteEffects dir, Vector2 drawpos, Color drawColor, Color shineColor, float flareCounter, float fadeInStart, float fadeInEnd, float fadeOutStart, float fadeOutEnd, float rotation, Vector2 scale, Vector2 fatness) 
        {
			Texture2D Texture = TextureAssets.Extra[98].Value;
			Color color = shineColor * opacity * 0.5f;
			color.A = (byte)0;
			Vector2 origin = Texture.Size() / 2f;
			Color color2 = drawColor * 0.5f;
			float Intensity = Utils.GetLerpValue(fadeInStart, fadeInEnd, flareCounter, clamped: true) * Utils.GetLerpValue(fadeOutEnd, fadeOutStart, flareCounter, clamped: true);
			Vector2 vector = new Vector2(fatness.X * 0.5f, scale.X) * Intensity;
			Vector2 vector2 = new Vector2(fatness.Y * 0.5f, scale.Y) * Intensity;
			color *= Intensity;
			color2 *= Intensity;
			Main.EntitySpriteDraw(Texture, drawpos, null, color, (float)Math.PI / 2f + rotation, origin, vector, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color, 0f + rotation, origin, vector2, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, (float)Math.PI / 2f + rotation, origin, vector * 0.6f, dir);
			Main.EntitySpriteDraw(Texture, drawpos, null, color2, 0f + rotation, origin, vector2 * 0.6f, dir);
		}

        public override void CutTiles()
        {
            Vector2 VectorX = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 78f * Projectile.scale;
            Vector2 VectorY = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 78f * Projectile.scale;
            float Distance = 78f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + VectorX, Projectile.Center + VectorY, Distance, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float Length = 112f * Projectile.scale;
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