using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class FlameIdolProj : ModProjectile
	{
        private static Asset<Texture2D> GlowTexture;

        public override void SetDefaults()
		{
            Projectile.width = 50;
            Projectile.height = 70;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override void PostDraw(Color lightColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Catacomb/FlameIdolProjGlow");

            Vector2 drawOrigin = new(GlowTexture.Width() * 0.5f, Projectile.height * 0.5f);

            var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            for (int numEffect = 0; numEffect < 4; numEffect++)
            {
                float shakeX = Main.rand.Next(-2, 2);
			    float shakeY = Main.rand.Next(-2, 2);

                Vector2 vector = new Vector2(Projectile.Center.X - 1 + shakeX, Projectile.Center.Y + shakeY) + (numEffect / 4 * 6f + Projectile.rotation).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, GlowTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, GlowTexture.Width(), GlowTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(GlowTexture.Value, vector, rectangle, Color.White * 0.5f, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0);
            }
        }

        public override bool? CanDamage()
        {
			return false;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - player.position;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            Projectile.position = player.MountedCenter - Projectile.Size / 2 + new Vector2((Projectile.direction == -1 ? -12 : 12), -5);

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;

                player.bodyFrame.Y = player.bodyFrame.Height * 3;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                {
                    player.statMana -= ItemGlobal.ActiveItem(player).mana;

                    SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, player.MountedCenter);

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<FlameIdolRing>(), Projectile.damage, 0f, Main.myPlayer);

                    Projectile.localAI[0] = 0;
                }

                if (direction.X > 0) 
                {
					player.direction = 1;
				}
				else 
                {
					player.direction = -1;
				}
            }
            else
            {
				Projectile.active = false;
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}