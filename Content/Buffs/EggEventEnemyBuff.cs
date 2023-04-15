using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs
{
	public class EggEventEnemyBuff : ModBuff
	{
        private bool initializeStats;
        private int storedDamage;
        private int storedDefense;
        private Color storedColor;

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                storedDamage = npc.damage;
                npc.damage = (int)(npc.damage * 2f);

                storedDefense = npc.defense;
                npc.defense = (int)(npc.defense * 2f);

                storedColor = npc.color;
                npc.color = Color.Red * 0.75f;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.damage = storedDamage;
                npc.defense = storedDefense;
                npc.color = storedColor;
            }
        }
    }
}
