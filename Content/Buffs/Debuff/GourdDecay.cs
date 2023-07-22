using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs.Debuff
{
	public class GourdDecay : ModBuff
	{
		private bool initializeStats;
        private int storedDamage;
        private int storedDefense;

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
			if (!npc.friendly && !npc.boss && npc.type != NPCID.EaterofWorldsHead && npc.type != NPCID.EaterofWorldsBody && npc.type != NPCID.EaterofWorldsTail)
            {
				if (Main.rand.NextBool(50))
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-3, -1), 
                        ModContent.ProjectileType<DecayDebuffFly>(), 0, 0f, npc.target, 0f, (float)npc.whoAmI);
					}
				}

                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }

                npc.lifeRegen -= 5;

				if (npc.buffTime[buffIndex] < 1800 && npc.buffTime[buffIndex] > 5)
				{
					if (!initializeStats)
					{
						storedDamage = npc.damage;
						storedDefense = npc.defense;
						npc.damage = (int)(npc.damage * 0.8f);
						npc.defense = (int)(npc.defense * 0.8f);

						initializeStats = true;
					}
				}
			}

			if (npc.buffTime[buffIndex] < 5)
            {
				npc.damage = storedDamage;
                npc.defense = storedDefense;
            }
		}
    }
}
