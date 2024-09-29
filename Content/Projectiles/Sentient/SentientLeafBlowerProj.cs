using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientLeafBlowerProj : ModProjectile
	{
        int playerCenterOffset = 8;

        public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.2f, Pitch = 0.75f };

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 9;
		}

		public override void SetDefaults()
		{
            Projectile.width = 54;
            Projectile.height = 114;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
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
                Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
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

            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 9)
                {
                    Projectile.frame = 0;
                }
            }

            player.itemRotation = Projectile.rotation;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - 7 - Projectile.height / 2);

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

                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                {
                    player.statMana -= ItemGlobal.ActiveItem(player).mana;

                    SoundEngine.PlaySound(UseSound, Projectile.Center);

                    Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y - playerCenterOffset);
                    ShootSpeed.Normalize();
                    ShootSpeed *= 12f;

                    Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 42f;

                    int Chunk = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X + muzzleOffset.X, Projectile.Center.Y + muzzleOffset.Y - playerCenterOffset, 
                    ShootSpeed.X + Main.rand.Next(-2, 3), ShootSpeed.Y + Main.rand.Next(-2, 3), ModContent.ProjectileType<OrganicChunk>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Main.projectile[Chunk].frame = Main.rand.Next(0, 6);

                    Projectile.localAI[0] = 0;
                }
			}

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}