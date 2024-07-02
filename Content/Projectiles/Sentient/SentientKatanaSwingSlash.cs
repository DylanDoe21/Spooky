using Terraria;
using Terraria.ModLoader;
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
            Color SlashColor1 = Color.Lerp(Color.Blue, Color.Red, Intensity);
			Color SlashColor2 = Color.Lerp(Color.Red, Color.Blue, Intensity);

			//these are the slash textures themselves
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)rectangle, SlashColor1 * Intensity, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, Scale * 1.1f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)rectangle, SlashColor2 * Intensity, Projectile.rotation, origin, Scale * 0.9f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)rectangle, SlashColor1 * Intensity, Projectile.rotation, origin, Scale * 0.7f, effects, 0f);
			
            //draw extra lines on top of each slash
            Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)ProjTexture.Frame(1, 2, 0, 1), SlashColor1 * 0.6f * Intensity, Projectile.rotation + Projectile.ai[0] * 0.01f, origin, Scale * 1.1f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)ProjTexture.Frame(1, 2, 0, 1), SlashColor2 * 0.5f * Intensity, Projectile.rotation + Projectile.ai[0] * -0.05f, origin, Scale * 0.9f, effects, 0f);
			Main.spriteBatch.Draw(ProjTexture.Value, vector, (Rectangle?)ProjTexture.Frame(1, 2, 0, 1), SlashColor1 * 0.4f * Intensity, Projectile.rotation + Projectile.ai[0] * -0.1f, origin, Scale * 0.7f, effects, 0f);

            return false;
        }

        public override void CutTiles()
        {
            Vector2 VectorX = (Projectile.rotation - (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
            Vector2 VectorY = (Projectile.rotation + (float)Math.PI / 4f).ToRotationVector2() * 60f * Projectile.scale;
            float Distance = 60f * Projectile.scale;
            Utils.PlotTileLine(Projectile.Center + VectorX, Projectile.Center + VectorY, Distance, DelegateMethods.CutTiles);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float Length = 85f * Projectile.scale;
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

                float divide = 1.75f;

                Projectile.NewProjectile(player.GetSource_OnHit(target), Main.MouseWorld.X, Main.MouseWorld.Y, 0, 0, 
                ModContent.ProjectileType<SentientKatanaSlashSpawner>(), Projectile.damage / (int)divide, Projectile.knockBack, player.whoAmI, 0f, 0f);
            }
        }
    }
}