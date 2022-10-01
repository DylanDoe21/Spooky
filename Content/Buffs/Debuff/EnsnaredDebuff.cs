using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class EnsnaredDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ensnared");
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.boss && npc.type != NPCID.EaterofWorldsHead)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Grass);
				npc.velocity *= 0;
			}
		}
    }
}
