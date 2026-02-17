using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
    public class CactusSentryProj : ModProjectile
    {
        public bool isAttacking = false;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 42;
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
            if (!isAttacking)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 4)
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
                    if (Projectile.frame >= 7)
                    {
                        Projectile.frame = 4;
                    }
                }
            }

            Projectile.velocity.X *= 0.95f;

            Projectile.velocity.Y++;
            if (Projectile.velocity.Y > 20f)
            {
                Projectile.velocity.Y = 20f;
            }

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 500f)
                {
					AttackingAI(NPC);

					break;
				}
                else
                {
                    isAttacking = false;
                }
            }
        }

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 8)
            {
                SoundEngine.PlaySound(SoundID.Item17 with { Volume = 0.35f }, Projectile.Center);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int Needle = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, 
                    4f * Projectile.DirectionTo(new Vector2(Projectile.Center.X, Projectile.Center.Y - 50)).RotatedByRandom(MathHelper.ToRadians(85)),
                    ModContent.ProjectileType<CactusNeedle>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[Needle].DamageType = DamageClass.Summon;
                }
            
                Projectile.ai[0] = 0;
            }
		}
    }
}