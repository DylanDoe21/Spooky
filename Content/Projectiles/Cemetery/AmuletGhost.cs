using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class AmuletGhost : ModProjectile
    {
        int MoveSpeedX = 0;
	 	int MoveSpeedY = 0;

        bool isAttacking = false;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> AuraTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
			Projectile.width = 82;
            Projectile.height = 66;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.localNPCHitCooldown = 30;
			Projectile.usesLocalNPCImmunity = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 3;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            AuraTexture ??= ModContent.Request<Texture2D>(Texture + "Aura");

            int numHorizontalFrames = 6;

			Vector2 drawOrigin = new((ProjTexture.Width() / numHorizontalFrames) * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

			Rectangle rectangle = new((ProjTexture.Width() / numHorizontalFrames) * (int)Projectile.ai[2], 
			ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, 
			ProjTexture.Width() / numHorizontalFrames, 
			ProjTexture.Height() / Main.projFrames[Projectile.type]);

			var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int i = 0; i < 360; i += 60)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.White);

                Vector2 circular = new Vector2(2f, 2f).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(AuraTexture.Value, vector + circular, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

			return false;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Projectile.alpha += 85; //85 is 1/3rd of 255 and this projectile can hit enemies 3 times before dying
		}

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            Projectile.spriteDirection = Projectile.direction;

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                {
                    Projectile.Kill();
                }
            }

            if (Projectile.timeLeft < 540)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC NPC = Main.npc[i];
                    if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 300f)
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

			if (!isAttacking || Projectile.timeLeft >= 540)
            {
                IdleAI(player);
            }

			//go to the player if they are too fars
			if (Projectile.Distance(player.Center) > 1200f)
			{
				Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
				Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
				Projectile.netUpdate = true;
			}

            //prevent Projectiles clumping together
			for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.08f;
					if (Projectile.position.X < other.position.X)
					{
						Projectile.velocity.X -= pushAway;
					}
					else
					{
						Projectile.velocity.X += pushAway;
					}
					if (Projectile.position.Y < other.position.Y)
					{
						Projectile.velocity.Y -= pushAway;
					}
					else
					{
						Projectile.velocity.Y += pushAway;
					}
				}
			}
		}

        public void IdleAI(Player player)
		{
            int MaxSpeed = Projectile.Distance(player.Center) > 200f ? 5 : 2;

			//flies to players X position
			if (Projectile.Center.X >= player.Center.X && MoveSpeedX >= -MaxSpeed - 1) 
			{
				MoveSpeedX -= 2;
			}
			else if (Projectile.Center.X <= player.Center.X && MoveSpeedX <= MaxSpeed + 1)
			{
				MoveSpeedX += 2;
			}

			Projectile.velocity.X += MoveSpeedX * 0.01f;
			Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -MaxSpeed - 1, MaxSpeed + 1);
			
			//flies to players Y position
			if (Projectile.Center.Y >= player.Center.Y - 120f && MoveSpeedY >= -MaxSpeed)
			{
				MoveSpeedY -= 2;
			}
			else if (Projectile.Center.Y <= player.Center.Y - 120f && MoveSpeedY <= MaxSpeed)
			{
				MoveSpeedY += 2;
			}

			Projectile.velocity.Y += MoveSpeedY * 0.01f;
			Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y, -MaxSpeed, MaxSpeed);
        }

        public void AttackingAI(NPC target)
		{
            Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 12;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
        }
    }
}