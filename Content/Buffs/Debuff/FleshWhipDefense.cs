using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs.Debuff
{
	public class FleshWhipDefense : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eye Lasher Defense");
			Main.debuff[Type] = true;
		}

		private bool initializeStats;
        private int storedDefense;

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                storedDefense = npc.defense;
				npc.defense = (int)(npc.defense * 0.8f);

                initializeStats = true;
			}

			if (npc.buffTime[buffIndex] < 5)
            {
                npc.defense = storedDefense;
            }
		}
    }
}
