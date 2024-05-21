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
		public override void SetDefaults()
        {
            Projectile.width = 88;
            Projectile.height = 88;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
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

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 300;

            Projectile.position = new Vector2(player.Center.X - (Projectile.width / 2), player.Center.Y - (Projectile.height / 2));

            if (player.dead || !player.GetModPlayer<BloomBuffsPlayer>().SpringRose)
            {
                Projectile.Kill();
            }
        }
    }
}