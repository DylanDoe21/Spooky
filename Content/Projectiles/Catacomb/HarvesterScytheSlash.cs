using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{ 
    public class HarvesterScytheSlash : SwordSlashBase
    {
        public override string Texture => "Spooky/Content/Projectiles/SwordSlashSpecial";

        float SaveKnockback;
        bool SavedKnockback = false;
        bool hasHitSomething = false;

        private static Asset<Texture2D> ProjTexture;

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            
            Vector2 vector = Projectile.Center - Main.screenPosition;
			Rectangle rectangle = ProjTexture.Frame(1, 2);
			Vector2 origin = rectangle.Size() / 2f;
            float Scale = Projectile.scale * 1.02f;
			SpriteEffects effects = ((!(Projectile.ai[0] >= 0f)) ? SpriteEffects.FlipVertically : SpriteEffects.None);
			float CurrentAI = Projectile.localAI[0] / Projectile.ai[1];
			float Intensity = Utils.Remap(CurrentAI, 0f, 0.6f, 0f, 1f) * Utils.Remap(CurrentAI, 0.6f, 1f, 1f, 0f);
			Color SlashColor = Color.Lerp(Color.Brown, Color.Teal, Intensity);

            //these are the slash textures themselves
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)rectangle, SlashColor * Intensity, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, Scale * 1.3f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)rectangle, SlashColor * Intensity, Projectile.rotation, origin, Scale * 1f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)rectangle, SlashColor * Intensity, Projectile.rotation, origin, Scale * 0.7f, effects, 0f);
			
            //draw extra lines on top of each slash
            Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)ProjTexture.Frame(1, 2, 0, 1), Color.DarkGray * 0.6f * Intensity, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, Scale * 1.3f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)ProjTexture.Frame(1, 2, 0, 1), Color.DarkGray * 0.4f * Intensity, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, Scale * 1f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)ProjTexture.Frame(1, 2, 0, 1), Color.DarkGray * 0.2f * Intensity, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, Scale * 0.7f, effects, 0f);
			
            //draw star sparkle and trail on top of the slashes
            for (float Repeats = 0f; Repeats < 8f; Repeats += 1f) 
            {
				float Fade = Projectile.rotation + Projectile.ai[0] * Repeats * ((float)Math.PI * -2f) * 0.025f + Utils.Remap(CurrentAI, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0];
				Vector2 drawpos = vector + Fade.ToRotationVector2() * ((float)ProjTexture.Width() * 0.5f - 6f) * Scale * 1.32f;
				DrawPrettyStarSparkle(Projectile.Opacity, (SpriteEffects)0, drawpos, Color.White * Intensity, Color.White * Intensity, CurrentAI, 0f, 0.5f, 0.5f, 1f, Fade, new Vector2(0f, Utils.Remap(CurrentAI, 0f, 1f, 3f, 0f)) * Scale * 0.85f, Vector2.One * Scale * 0.85f);
            
                Vector2 drawpos2 = vector + (Projectile.rotation + Utils.Remap(CurrentAI, 0f, 1f, 0f, (float)Math.PI / 4f) * Projectile.ai[0]).ToRotationVector2() * ((float)ProjTexture.Width() * 0.5f - 4f) * Scale * 1.3f;
			    DrawPrettyStarSparkle(Projectile.Opacity, (SpriteEffects)0, drawpos2, Color.White * Intensity, Color.White * Intensity, CurrentAI, 0f, 0.5f, 0.5f, 1f, 0f, new Vector2(2f, Utils.Remap(CurrentAI, 0f, 1f, 4f, 1f)) * Scale * 0.85f, Vector2.One * Scale * 0.85f);
            }

            return false;
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
            float Length = 105f * Projectile.scale;
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

                if (player.ownedProjectileCounts[ModContent.ProjectileType<SoulBolt>()] < 10)
                {
                    Projectile.NewProjectile(player.GetSource_OnHit(target), target.Center.X, target.Center.Y, 0, 0,
                    ModContent.ProjectileType<SoulBolt>(), Projectile.damage / 2, 0f, Main.myPlayer, 0, 0);
                }
            }
        }
    }
}