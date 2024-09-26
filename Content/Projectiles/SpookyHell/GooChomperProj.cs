using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class GooChomperProj : ModProjectile
    {
		public static readonly SoundStyle BiteSound = new("Spooky/Content/Sounds/Bite", SoundType.Sound) { Pitch = 0.5f, Volume = 0.5f };

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 4;
		}

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 58;
            Projectile.DamageType = DamageClass.Generic;
			Projectile.localNPCHitCooldown = 30;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override bool? CanDamage()
        {
            return Projectile.frame >= 3;
        }

		public override void AI()
        {
			NPC Parent = Main.npc[(int)Projectile.ai[0]];

			Projectile.frameCounter++;
            if (Projectile.frameCounter >= 7)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
					SoundEngine.PlaySound(BiteSound, Projectile.Center);

					Parent.GetGlobalNPC<NPCGlobal>().HasGooChompterAttached = false;
					
					Projectile.Kill();
                }
            }

			if (!Parent.active)
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.Center = Parent.Center;
			}
		}

		public override void OnKill(int timeLeft)
		{
			for (int numGores = 1; numGores <= 4; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(Main.rand.Next(-7, 8), Main.rand.Next(-2, 3)), ModContent.Find<ModGore>("Spooky/GooSlugGore" + Main.rand.Next(1, 3)).Type, 0.75f);
				}
			}
		}
    }
}