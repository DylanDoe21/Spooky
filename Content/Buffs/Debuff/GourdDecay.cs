using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs.Debuff
{
	public class GourdDecay : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rotting");
			Main.debuff[Type] = true;
		}

		private bool initializeStats;
        private int storedDamage;
        private int storedDefense;

		public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.buffTime[buffIndex] > 5 && npc.buffTime[buffIndex] < 5)
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

			if (npc.buffTime[buffIndex] < 5)
            {
				npc.damage = storedDamage;
                npc.defense = storedDefense;
            }
		}
    }
}
