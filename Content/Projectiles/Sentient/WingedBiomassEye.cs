using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Sentient
{
    public class WingedBiomassEye : ModProjectile
    {
        int Bounces = 0;

        bool IsStickingToTarget = false;

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
			Projectile.width = 18;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
		}
		
		public override void AI()
        {
            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            NPC target = Main.npc[(int)Projectile.ai[0]];
            Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 25;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);

            if (!target.active)
            {
                Projectile.Kill();
            }
		}

        public override void OnKill(int timeLeft)
		{
            //TODO: find a different vanilla sound
            //SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
        }
    }
}