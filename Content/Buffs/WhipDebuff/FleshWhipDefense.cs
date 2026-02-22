using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.WhipDebuff
{
	public class FleshWhipDefense : ModBuff
	{
        public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

        private bool initializeStats;
        private int storedDefense;

        public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
			BuffID.Sets.IsATagBuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats && npc.buffTime[buffIndex] >= 5)
            {
                storedDefense = npc.defense;
				npc.defense = (int)(npc.defense * 0.8f);

                initializeStats = true;
			}

			if (npc.buffTime[buffIndex] < 5)
            {
                npc.defense = storedDefense;
                initializeStats = false;
            }
		}
    }
}
