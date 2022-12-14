using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs
{
	public class EggEventEnemyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Capillary buff");
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.friendly)
			{
				npc.damage = (int)(npc.damage * 2f);
				npc.defense = (int)(npc.defense * 1.5f);
			}
		}
    }
}
