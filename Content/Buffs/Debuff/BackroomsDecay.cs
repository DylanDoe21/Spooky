using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs.Debuff
{
	public class BackroomsDecay : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
			if (!npc.friendly)
            {
				if (npc.life > (npc.lifeMax / 2))
				{
					if (npc.lifeRegen > 0)
					{
						npc.lifeRegen = 0;
					}

					if (!npc.boss && npc.type != NPCID.EaterofWorldsHead && npc.type != NPCID.EaterofWorldsBody && npc.type != NPCID.EaterofWorldsTail)
					{
						npc.lifeRegen -= 20;
					}
					else
					{
						npc.lifeRegen -= 10;
					}
				}
			}
		}
    }
}
