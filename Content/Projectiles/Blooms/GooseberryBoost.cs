using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Content.Buffs;

namespace Spooky.Content.Projectiles.Blooms
{
    public class GooseberryBoost : ModProjectile
    {
        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 360;
            Projectile.penetrate = 1;
        }

        public override bool? CanDamage()
		{
			return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[1] == 0)
            {
                Projectile.velocity *= 0;

                Projectile.ai[1] = 1;
            }

			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 30)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;

				foreach (var NPC in Main.ActiveNPCs)
				{
					if (NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type])
					{
						if (Projectile.Hitbox.Intersects(NPC.Hitbox))
						{
							SoundEngine.PlaySound(SoundID.NPCDeath1 with { Pitch = 1f }, Projectile.Center);

							Projectile.Kill();
						}
					}
				}

				foreach (var Proj in Main.ActiveProjectiles)
				{
					if (Proj.hostile)
					{
						if (Projectile.Hitbox.Intersects(Proj.Hitbox))
						{
							SoundEngine.PlaySound(SoundID.NPCDeath1 with { Pitch = 1f }, Projectile.Center);

							Projectile.Kill();
						}
					}
				}

                if (Projectile.Distance(player.Center) <= 150f)
                {
                    Vector2 desiredVelocity = Projectile.DirectionTo(player.MountedCenter) * 20;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
                }
				
				if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    SoundEngine.PlaySound(SoundID.Item2, Projectile.Center);

					player.AddBuff(ModContent.BuffType<GooseberryBoostBuff>(), 300);

                    Projectile.Kill();
                }
            }

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
        }
    }
}