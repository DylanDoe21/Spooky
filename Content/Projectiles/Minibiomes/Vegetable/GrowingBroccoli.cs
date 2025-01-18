using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{ 
    public class GrowingBroccoli : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
		{
			DrawOffsetX = 0;
			DrawOriginOffsetY = -16;
			DrawOriginOffsetX = -80;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
            Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			if (Projectile.ai[1] > 0)
            {
		    	Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, 
                new Rectangle(50 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 26), lightColor, 
                Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);
            }

			return false;
		}

		//The AI of the projectile
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			NPC Parent = Main.npc[(int)Projectile.ai[2]];

			if (!Parent.active || Parent.life <= 0)
			{
				Projectile.Kill();
			}

			if (Projectile.ai[2] != 0)
			{
				Vector2 pos = new Vector2(20, 0).RotatedBy(Projectile.rotation);
				Projectile.Center = pos + Parent.Center;
			}

			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = 1;
				Projectile.rotation = Projectile.ai[0];
				Projectile.ai[0] = 0;
			}

			Projectile.ai[1] += 2;
			if (Projectile.ai[1] > 50)
			{
				Projectile.ai[1] = 50;

				Projectile.ai[0]++;
				if (Projectile.ai[0] >= 30)
				{
					player.ApplyDamageToNPC(Parent, Projectile.damage, 0, default);
					Projectile.Kill();
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            for (int numGores = 1; numGores <= 4; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                	Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/GrowingBroccoliGore" + numGores).Type);
				}
            }
		}
	}
}