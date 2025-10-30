using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SporeWandBall : ModProjectile
    {
        public bool IsStickingToTarget = false;

        public override void SetDefaults()
        {
			Projectile.width = 24;
            Projectile.height = 24;
            Projectile.localNPCHitCooldown = 60;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.friendly = true;                              			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsStickingToTarget)
            {
                Projectile.ai[1] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                Projectile.timeLeft = 180;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
        }
		
		public override void AI()
        {
            if (IsStickingToTarget) 
            {
				Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                Projectile.ai[0]++;
                if (Projectile.ai[0] % 60 == 0 && Main.rand.NextBool())
                {
                    int newProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, 
                    ModContent.ProjectileType<SporeCloud>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: 2);
                    Main.projectile[newProj].DamageType = DamageClass.Magic;
                    Main.projectile[newProj].alpha = 125;
                }

                int npcTarget = (int)Projectile.ai[1];
                if (npcTarget < 0 || npcTarget >= 200) 
                {   
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = true;
                    IsStickingToTarget = false;
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
                {
                    Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                }
                else 
                {
                    Projectile.velocity = Vector2.Zero;
                    Projectile.tileCollide = true;
                    IsStickingToTarget = false;
                }
			}
			else 
            {
				Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.025f * (float)Projectile.direction;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.1f;
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (!IsStickingToTarget)
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.8f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                }
            }

			return false;
		}

        public override void OnKill(int timeLeft)
		{
            //SoundEngine.PlaySound(SoundID.NPCHit2, Projectile.Center);
        }
    }
}