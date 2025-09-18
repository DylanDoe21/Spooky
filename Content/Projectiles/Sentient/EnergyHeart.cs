using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Sentient
{
	public class EnergyHeart : ModProjectile
	{
        int NumNPCsTargetted = 0;

        public static readonly SoundStyle PumpSound = new("Spooky/Content/Sounds/HeartPump", SoundType.Sound) { Volume = 2f, PitchVariance = 0.7f };

		public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 3;
		}

		public override void SetDefaults()
		{
            Projectile.width = 48;
            Projectile.height = 62;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
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

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

			if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}

			if (player.channel)
            {
                Projectile.timeLeft = 2;

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 GoTo = Main.MouseWorld;

                    float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 10, 20);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);
                }

                Projectile.ai[0]++;

                int ShootTime = 30 + ItemGlobal.ActiveItem(player).useTime / 3;
                float MaxDistance = 300f;

                if (Projectile.frame == 1)
                {
                    ShootTime = 20 + ItemGlobal.ActiveItem(player).useTime / 3;
                    MaxDistance = 425f;
                }
                if (Projectile.frame == 2) 
                {
                    ShootTime = 10 + ItemGlobal.ActiveItem(player).useTime / 3;
                    MaxDistance = 550f;
                }
                
                if (Projectile.ai[0] >= ShootTime)
                {
                    if (Projectile.ai[0] == ShootTime && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                    {
						SoundEngine.PlaySound(PumpSound, Projectile.Center);

						player.statMana -= ItemGlobal.ActiveItem(player).mana;

						int MaxNPCsTargetted = 3 * (Projectile.frame + 1);
                        
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (NumNPCsTargetted < MaxNPCsTargetted)
                            {
                                NPC NPC = Main.npc[i];
                                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Projectile.Distance(NPC.Center) <= MaxDistance)
                                {
                                    Vector2 ShootSpeed = NPC.Center - Projectile.Center;
                                    ShootSpeed.Normalize();
                                    ShootSpeed *= 15f;

                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, 
                                    ModContent.ProjectileType<EnergyHeartBolt>(), Projectile.damage, 0, Projectile.owner, ShootSpeed.ToRotation());

                                    NumNPCsTargetted++;
                                }
                            }
                        }
                    }

                    //heart pumping effect
                    Projectile.ai[1]++;
                    if (Projectile.ai[1] < 3)
                    {
                        Projectile.scale += 0.22f;
                    }
                    if (Projectile.ai[1] >= 3)
                    {
                        Projectile.scale -= 0.22f;
                    }
                    
                    if (Projectile.ai[1] > 6)
                    {
                        NumNPCsTargetted = 0;

                        Projectile.scale = 1f;

                        Projectile.ai[1] = 0;
                        Projectile.ai[0] = 0;
                    }
                }

                //make heart grow every 15 seconds
                Projectile.ai[2]++;

                if (Projectile.ai[2] >= 420 && Projectile.frame < 2)
                {
                    SoundEngine.PlaySound(SoundID.AbigailAttack, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item167 with { Pitch = 0.5f }, Projectile.Center);

                    Projectile.frame++;

                    Projectile.ai[2] = 0;
                }

                player.heldProj = Projectile.whoAmI;
                player.SetDummyItemTime(2);
            }
        }
	}
}