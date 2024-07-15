using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Terraria.Audio;

namespace Spooky.Content.Projectiles.Blooms
{
    public class RoseThornRing : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
        {
            Projectile.width = 98;
            Projectile.height = 92;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 35;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

			Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, lightColor, 0, drawOrigin, 1f + fade / 10f, SpriteEffects.None, 0f);

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 5;

            Projectile.position = new Vector2(player.Center.X - (Projectile.width / 2), player.Center.Y - (Projectile.height / 2));

            if (player.dead || !player.GetModPlayer<BloomBuffsPlayer>().SpringRose)
            {
                Projectile.Kill();
            }
        }
    }
}