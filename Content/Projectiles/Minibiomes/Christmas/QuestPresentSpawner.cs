using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
    public class QuestPresentSpawner : ModProjectile
    {
		bool Shake = false;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = -1;
        }

		public override bool? CanDamage()
		{
			return false;
		}

        public override void AI()
        {
			Projectile.frame = (int)Projectile.ai[1];

			NPC TownNPCToKill = Main.npc[(int)Projectile.ai[2]];

			TownNPCToKill.velocity.X = 0;

			Projectile.ai[0]++;
			if (Projectile.ai[0] <= 60)
			{
				if (Shake)
				{
					Projectile.rotation += 0.1f;
					if (Projectile.rotation > 0.2f)
					{
						Shake = false;
						Projectile.netUpdate = true;
					}
				}
				else
				{
					Projectile.rotation -= 0.1f;
					if (Projectile.rotation < -0.2f)
					{
						Shake = true;
						Projectile.netUpdate = true;
					}
				}
			}
			else
			{
				SoundEngine.PlaySound(SoundID.ResearchComplete with { Pitch = 1.5f }, Projectile.Center);

				for (int numGores = 1; numGores <= 12; numGores++)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(null, Projectile.position, new Vector2(Main.rand.Next(-5, 6), Main.rand.Next(-5, -2)), Main.rand.Next(276, 283));
					}
				}

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					TownNPCToKill.StrikeInstantKill();
				}

				Projectile.Kill();
			}
        }
    }
}