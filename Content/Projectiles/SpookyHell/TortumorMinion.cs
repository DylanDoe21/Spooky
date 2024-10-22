using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;
using System.IO;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class TortumorMinion : ModProjectile
    {
		int MoveSpeedX = 0;
	 	int MoveSpeedY = 0;

		bool isAttacking = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 12;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

		public override void SendExtraAI(BinaryWriter writer) 
		{
			//ints
			writer.Write(MoveSpeedX);
			writer.Write(MoveSpeedY);

			//bools
			writer.Write(isAttacking);
		}
        
		public override void ReceiveExtraAI(BinaryReader reader)
		{	
			//ints
			MoveSpeedX = reader.ReadInt32();
			MoveSpeedY = reader.ReadInt32();

			//bools
			isAttacking = reader.ReadBoolean();
		}
        
        public override void SetDefaults()
        {
			Projectile.width = 58;
            Projectile.height = 62;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
			Projectile.minionSlots = 1;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<TortumorMinionBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<TortumorMinionBuff>()))
            {
				Projectile.timeLeft = 2;
			}

			Projectile.rotation += (Projectile.velocity.X / 30);

			for (int i = 0; i < 200; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 550f)
                {
					AttackingAI(Target);

					break;
				}
				else
				{
					isAttacking = false;
				}

				NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 550f)
                {
					AttackingAI(NPC);

					break;
				}
				else
				{
					isAttacking = false;
				}
            }

			if (!isAttacking)
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

		public void AttackingAI(NPC target)
		{
			isAttacking = true;

			Projectile.ai[0]++;

			//idle animation
			Projectile.frameCounter++;
			if (Projectile.ai[0] < 30)
			{
				if (Projectile.frameCounter >= 8)
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
				if (Projectile.ai[0] == 30)
                {
                    Projectile.frame = 8;
                }

				if (Projectile.frameCounter >= 8)
				{
					Projectile.frameCounter = 0;
					Projectile.frame++;
					if (Projectile.frame >= 12)
					{
						Projectile.frame = 11;
					}
				}
			}

			int MaxSpeed = Projectile.Distance(target.Center) > 200f ? 5 : 2;

			//flies to targets X position
			if (Projectile.Center.X >= target.Center.X && MoveSpeedX >= -MaxSpeed - 1) 
			{
				MoveSpeedX -= 2;
			}
			else if (Projectile.Center.X <= target.Center.X && MoveSpeedX <= MaxSpeed + 1)
			{
				MoveSpeedX += 2;
			}

			Projectile.velocity.X += MoveSpeedX * 0.01f;
			Projectile.velocity.X = MathHelper.Clamp(Projectile.velocity.X, -MaxSpeed - 1, MaxSpeed + 1);
			
			//flies to targets Y position
			if (Projectile.Center.Y >= target.Center.Y - 120f && MoveSpeedY >= -MaxSpeed)
			{
				MoveSpeedY -= 2;
			}
			else if (Projectile.Center.Y <= target.Center.Y - 120f && MoveSpeedY <= MaxSpeed)
			{
				MoveSpeedY += 2;
			}

			Projectile.velocity.Y += MoveSpeedY * 0.01f;
			Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y, -MaxSpeed, MaxSpeed);

			if (Projectile.ai[0] == 60)
			{
				SoundEngine.PlaySound(SoundID.Item87, Projectile.Center);

				Vector2 ShootSpeed = target.Center - Projectile.Center;
				ShootSpeed.Normalize();
				ShootSpeed *= 25f;
						
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<TortumorMinionOrb>(), Projectile.damage, 1f, Main.myPlayer, Main.rand.Next(0, 2));
			}

			if (Projectile.ai[0] >= 80)
			{
				Projectile.ai[0] = 0;
			}
		}

		public void IdleAI(Player player)
		{
			//idle animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 8)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }

			Projectile.ai[0] = 0;

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
    }
}