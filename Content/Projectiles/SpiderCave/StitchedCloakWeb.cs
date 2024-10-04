using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class StitchedCloakWeb : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

		public override void SetDefaults()
        {
            Projectile.width = 82;
            Projectile.height = 80;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 480;
            Projectile.penetrate = -1;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.immune[Projectile.owner] = 35;
            target.AddBuff(ModContent.BuffType<StitchedCloakWebSlow>(), 30);
        }

        public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 5f + 0.5f;

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

			Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            Color color = Projectile.GetAlpha(new Color(125, 125, 125, 0).MultiplyRGBA(Color.White));
            
            Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, 0, drawOrigin, 1f + fade / 10f, SpriteEffects.None, 0f);

            return false;
        }

        public override void AI()
        {
            if (Projectile.timeLeft < 60)
            {
                Projectile.alpha += 5;
            }
        }
    }
}