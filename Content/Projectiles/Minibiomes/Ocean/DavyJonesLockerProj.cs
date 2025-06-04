using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Ocean
{
	public class DavyJonesLockerProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.width = 58;
			Projectile.height = 108;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.hide = true;
			Projectile.timeLeft = 20;
			Projectile.penetrate = -1;
			Projectile.aiStyle = -1;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindProjectiles.Add(index);
		}

		public override bool? CanDamage()
        {
			return false;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            if (Projectile.direction >= 0)
            {
                Projectile.rotation = direction.ToRotation() - 1.57f * (float)Projectile.direction;
            }
            else
            {
                Projectile.rotation = direction.ToRotation() + 1.57f * (float)Projectile.direction;
            }

            player.itemRotation = Projectile.rotation;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.Center = new Vector2(player.MountedCenter.X, player.MountedCenter.Y);

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (player.channel) 
            {
                Projectile.timeLeft = 2;

                Projectile.ai[2]++;
                if (Projectile.ai[2] >= ItemGlobal.ActiveItem(player).useTime && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                {
                    player.statMana -= ItemGlobal.ActiveItem(player).mana;

					SoundEngine.PlaySound(SoundID.Item177 with { Pitch = -1f, Volume = 0.5f }, Projectile.Center);
					SoundEngine.PlaySound(SoundID.NPCDeath1 with { Pitch = -1f, Volume = 0.5f }, Projectile.Center);

					if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                        ShootSpeed.Normalize();
                        ShootSpeed *= 12f;

                        Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 20f;

						for (int numDusts = 0; numDusts < 12; numDusts++)
						{
							Dust dust = Dust.NewDustPerfect(new Vector2(Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y), DustID.WaterCandle,
							new Vector2(ShootSpeed.X + Main.rand.Next(-7, 8), ShootSpeed.Y + Main.rand.Next(-7, 8)) * 0.5f, default, default, 2f);
							dust.noGravity = true;
							dust.velocity += player.velocity;
						}

                        int Type = Main.rand.NextBool(10) ? ModContent.ProjectileType<LockerSock>() : ModContent.ProjectileType<LockerFish>();

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y, 
                        ShootSpeed.X + Main.rand.Next(-3, 4), ShootSpeed.Y + Main.rand.Next(-3, 4), Type, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                    Projectile.ai[2] = 0;
                }
			}

            //player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}