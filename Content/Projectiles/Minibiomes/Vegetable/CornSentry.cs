using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Minibiomes.Vegetable
{
    public class CornSentry : ModProjectile
    {
        public bool isAttacking = false;

        private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> PupilTexture;
        private static Asset<Texture2D> PupilLargeTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 8;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 62;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

        public override void AI()
        {
            //animation
            if (!isAttacking)
            {
                //reset ai timer
                Projectile.ai[0] = 0;

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 9)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 8)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 4)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 8)
                    {
                        Projectile.frame = 0;
                    }
                }
            }

            //fall down constantly
            Projectile.velocity.Y += 0.65f;
            if (Projectile.velocity.Y > 20f)
            {
                Projectile.velocity.Y = 20f;
            }

            //target an enemy
            for (int i = 0; i < 200; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(Projectile.Center, Target.Center) <= 500f)
                {
					Shoot(Target);

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 500f)
                {
					Shoot(NPC);

					break;
				}
                else
                {
                    isAttacking = false;
                }
            }
        }

        public void Shoot(NPC target)
		{
            isAttacking = true;

            Projectile.ai[0]++;
			if (Projectile.ai[0] >= 45)
			{
				SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);

				float Speed = 6f;
				Vector2 vector = new(Projectile.position.X + 2 + (Projectile.width / 2), Projectile.position.Y - 23 + (Projectile.height / 2));
				float rotation = (float)Math.Atan2(vector.Y - (target.position.Y + (target.height * 0.5f)), vector.X - (target.position.X + (target.width * 0.5f)));
				Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y - 12,
				perturbedSpeed.X, Main.rand.Next(-6, 1), ModContent.ProjectileType<CornSentryKernel>(), Projectile.damage, 0f, Main.myPlayer);

                Projectile.ai[0] = 0;
            }
		}
    }
}