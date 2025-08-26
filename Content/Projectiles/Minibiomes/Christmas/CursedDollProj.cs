using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Minibiomes.Christmas
{
	public class CursedDollProj : ModProjectile
	{
		public override void SetDefaults()
		{
            Projectile.width = 22;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 5;
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
                Vector2 ProjDirection = Main.MouseWorld - player.position;
                ProjDirection.Normalize();
                Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
            }

            Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

            Projectile.position = player.Center - Projectile.Size / 2 + new Vector2((Projectile.direction == -1 ? -15 : 15), 0);

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                player.itemRotation = Projectile.rotation;

                player.bodyFrame.Y = player.bodyFrame.Height * 3;

                Projectile.ai[2]++;
                if (Projectile.ai[2] >= ItemGlobal.ActiveItem(player).useTime && player.CheckMana(ItemGlobal.ActiveItem(player)))
                {
                    player.statMana -= ItemGlobal.ActiveItem(player).mana;

                    SoundEngine.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy with { Volume = 0.25f }, Projectile.Center);

					int Distance = 250;

					foreach (NPC npc in Main.ActiveNPCs)
					{
						if (npc.Distance(Projectile.Center) <= Distance && !npc.friendly && !npc.dontTakeDamage && !NPCID.Sets.CountsAsCritter[npc.type])
						{
							npc.AddBuff(ModContent.BuffType<CursedDollDebuff>(), 300);
						}
					}

                    int Amount = Main.rand.Next(1, 3);
                    for (int numDusts = 0; numDusts < Amount; numDusts++)
                    {
                        //create dust rings for debugging
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 offset = new Vector2();
                            double angle = Main.rand.NextDouble() * 2d * Math.PI;
                            offset.X += (float)(Math.Sin(angle) * Distance);
                            offset.Y += (float)(Math.Cos(angle) * Distance);
                            Vector2 DustPos = Projectile.Center + offset - new Vector2(4, 4);
                            Dust dust = Main.dust[Dust.NewDust(DustPos, 0, 0, DustID.Shadowflame, 0, 0, 100, Color.White, 1f)];
                            dust.velocity = -((DustPos - Projectile.Center) * Main.rand.NextFloat(0.01f, 0.12f));
                            dust.noGravity = true;
                            dust.scale = 2f;
                        }
                    }

                    Projectile.ai[2] = 0;
                }

                if (direction.X > 0) 
                {
					player.direction = 1;
				}
				else 
                {
					player.direction = -1;
				}
            }
            else
            {
				Projectile.active = false;
            }

            player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}