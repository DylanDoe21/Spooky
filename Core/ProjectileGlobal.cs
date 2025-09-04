using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Biomes;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Dusts;
using Spooky.Content.Generation;
using Spooky.Content.Projectiles.Blooms;

namespace Spooky.Core
{
    public class ProjectileGlobal : GlobalProjectile
    {
		public override bool InstancePerEntity => true;

		public bool SpongeAbsorbAttempt = false;

		public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.player[projectile.owner];

			//creepy candle makes magic projectiles inflict on fire
			if (player.GetModPlayer<SpookyPlayer>().MagicCandle && projectile.DamageType == DamageClass.Magic)
            {
                if (Main.rand.NextBool(3))
                {
                    target.AddBuff(BuffID.OnFire, 120);
                }
            }

            //root armor set makes ranged projectiles life-steal sometimes
            if (player.GetModPlayer<SpookyPlayer>().RootSet && player.GetModPlayer<SpookyPlayer>().RootHealCooldown <= 0 && projectile.DamageType == DamageClass.Ranged && damageDone >= 2)
            {
                if (Main.rand.NextBool(5))
                {
                    //heal based on how much damage was done
                    int LifeHealed = damageDone > 12 ? 6 : damageDone / 2;
					player.statLife += LifeHealed;
					player.HealEffect(LifeHealed, true);
					player.GetModPlayer<SpookyPlayer>().RootHealCooldown = 300;
                }
            }
        }

        public override bool PreAI(Projectile projectile)
		{
            //convert spooky mod tiles with different clentaminator solutions
            if (projectile.type == ProjectileID.PureSpray || projectile.type == ProjectileID.DirtSpray)
            {
                TileConversionMethods.ConvertSpookyIntoPurity((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.HallowSpray)
            {
                TileConversionMethods.ConvertSpookyIntoHallow((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.CorruptSpray)
            {
                TileConversionMethods.ConvertSpookyIntoCorruption((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.CrimsonSpray)
            {
                TileConversionMethods.ConvertSpookyIntoCrimson((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.SnowSpray)
            {
                TileConversionMethods.ConvertSpookyIntoSnow((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }
            if (projectile.type == ProjectileID.SandSpray)
            {
                TileConversionMethods.ConvertSpookyIntoDesert((int)(projectile.position.X + (projectile.width * 0.5f)) / 16, (int)(projectile.position.Y + (projectile.height * 0.5f)) / 16, 2);
            }

			Player player = Main.player[projectile.owner];

			if (player.GetModPlayer<BloomBuffsPlayer>().VegetableEggplantPaint && projectile.velocity != Vector2.Zero && projectile.DamageType == DamageClass.Ranged && Main.GameUpdateCount % 10 == 0)
			{
				Projectile.NewProjectile(projectile.GetSource_FromAI(), projectile.Center, Vector2.Zero, ModContent.ProjectileType<EgplantPaint>(), projectile.damage / 3, 0f, Main.LocalPlayer.whoAmI, Main.rand.Next(0, 5));
			}

			//minions inflict toxic and have toxic cloud dusts with the hazmat armor set
			if (player.GetModPlayer<SpookyPlayer>().HazmatSet && projectile.DamageType == DamageClass.Summon && projectile.minionSlots > 0)
			{
				if (Main.rand.NextBool(18))
				{
					Color[] colors = { Color.Lime, Color.Green };

					int DustEffect = Dust.NewDust(projectile.position, projectile.width, 3, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Main.rand.Next(colors) * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
					Main.dust[DustEffect].velocity.X = 0;
					Main.dust[DustEffect].velocity.Y = -1;
					Main.dust[DustEffect].alpha = 100;
				}

				foreach (NPC npc in Main.ActiveNPCs)
				{
					if (npc != null && npc.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[npc.type] && Vector2.Distance(projectile.Center, npc.Center) <= 200f)
					{
						npc.AddBuff(ModContent.BuffType<HazmatMinionToxic>(), 2);
					}
				}
			}

			return base.PreAI(projectile);
		}

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
			if (projectile.type == ProjectileID.WorldGlobe) 
            {
				Player player = Main.player[projectile.owner];
				if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (player.InModBiome<SpookyBiome>())
                    {
                        if (!Flags.SpookyBackgroundAlt)
                        {
                            Flags.SpookyBackgroundAlt = true;
                        }
                        else
                        { 
                            Flags.SpookyBackgroundAlt = false;
                        }
                        
                        NetMessage.SendData(MessageID.WorldData);

                        if (!Main.gameMenu)
                        {
                            SpookyWorld.BGTransitionFlash = 1f;
                        }
                    }

                    if (player.InModBiome<CemeteryBiome>())
                    {
                        if (!Flags.CemeteryBackgroundAlt)
                        {
                            Flags.CemeteryBackgroundAlt = true;
                        }
                        else
                        { 
                            Flags.CemeteryBackgroundAlt = false;
                        }
                        
                        NetMessage.SendData(MessageID.WorldData);

                        if (!Main.gameMenu)
                        {
                            SpookyWorld.BGTransitionFlash = 1f;
                        }
                    }
				}
			}

            return base.PreKill(projectile, timeLeft);
        }
    }

	public static partial class ProjectileUtil
	{
		public static T ModProjectile<T>(this Projectile projectile) where T : ModProjectile
		{
			return projectile.ModProjectile as T;
		}
	}
}
