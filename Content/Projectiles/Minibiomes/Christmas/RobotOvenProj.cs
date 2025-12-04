using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
	public class RobotOvenProj : ModProjectile
	{
        public static readonly SoundStyle DingSound = new("Spooky/Content/Sounds/ChefRobotDing", SoundType.Sound) { Volume = 0.5f, PitchVariance = 0.6f };

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //floats
            writer.Write(Projectile.localAI[0]);
            writer.Write(Projectile.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //floats
            Projectile.localAI[0] = reader.ReadSingle();
            Projectile.localAI[1] = reader.ReadSingle();
        }

		public override void SetDefaults()
		{
            Projectile.width = 24;
            Projectile.height = 70;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 20;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
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

            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 2 - Projectile.height / 2);

            if (direction.X > 0) 
            {
                player.direction = 1;
            }
            else 
            {
                player.direction = -1;
            }

			if (Projectile.frame <= 1 && Projectile.ai[2] == 0)
            {
                Projectile.timeLeft = 60;

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime / 3)
                {
                    Projectile.frame++;

                    Projectile.localAI[0] = 0;
                }
			}
			else 
            {
                Projectile.frame = 2;

                if (Projectile.timeLeft == 59)
                {
                    SoundEngine.PlaySound(DingSound, Projectile.Center);
                }

                if (Projectile.timeLeft == 30)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath, Projectile.Center);

                    if (Projectile.localAI[1] > 0 && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                    {
                        player.statMana -= ItemGlobal.ActiveItem(player).mana;
                    }
                }

                if (Projectile.timeLeft >= 5 && Projectile.timeLeft <= 30 && Projectile.timeLeft % 5 == 0)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                        ShootSpeed.Normalize();
                        ShootSpeed *= 5;

                        Vector2 newVelocity = ShootSpeed.RotatedByRandom(MathHelper.ToRadians(5));

                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, newVelocity.X, newVelocity.Y, 
                        ModContent.ProjectileType<RobotOvenFire>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }

                if (Projectile.timeLeft < 5)
                {
                    if (!player.channel)
                    {
                        Projectile.Kill();
                    }

                    if (Projectile.localAI[1] <= 0)
                    {
                        Projectile.localAI[1]++;
                    }

                    Projectile.frame = 0;
                    Projectile.ai[2] = 0;

                    Projectile.netUpdate = true;
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}