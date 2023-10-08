using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Cemetery
{
	public class PolybiusSwirl : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.width = 138;
            Projectile.height = 138;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = 5;
            Projectile.alpha = 100;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Cemetery/PolybiusSwirlBack").Value;

            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);
            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + drawOrigin + new Vector2(-69, Projectile.gfxOffY - 69);

            Main.EntitySpriteDraw(tex, vector, null, lightColor * 0.65f, -Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;

            Projectile.rotation -= 0.05f;

            Projectile.position = new Vector2(Main.MouseWorld.X - 69, Main.MouseWorld.Y - 69);

            if (!player.GetModPlayer<SpookyPlayer>().PolybiusArcadeGame || Main.gamePaused)
            {
                Projectile.Kill();
            }
		}

        public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            player.AddBuff(ModContent.BuffType<PolybiusCooldown>(), 1200);
        }
    }
}