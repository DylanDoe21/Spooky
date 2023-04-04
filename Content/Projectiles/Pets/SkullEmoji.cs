using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
	public class SkullEmoji : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Skull Emoji");
			Main.projFrames[Projectile.type] = 14;
			Main.projPet[Projectile.type] = true;
		}

		public override void SetDefaults()
		{
			Projectile.width = 40;
            Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().SkullEmojiPet = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().SkullEmojiPet)
            {
				Projectile.timeLeft = 2;
            }

            Projectile.frameCounter++;
			if (Projectile.frameCounter >= 8)
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 14) 
				{
					Projectile.frame = 0;
				}
			}

            Projectile.spriteDirection = player.direction;

            Projectile.ai[0]++;
            Vector2 destination = new Vector2(player.Center.X + 9, player.Center.Y - 60 + (float)Math.Sin(Projectile.ai[0] / 30) * 10);
            Projectile.Center = destination;
        }
    }
}