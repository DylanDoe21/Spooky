using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class LivingAxeBleed : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.friendly)
            {
				if (npc.lifeRegen > 0)
				{
                    npc.lifeRegen = 0;
				}

                npc.lifeRegen -= 60;

                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood);
			}
		}
    }
}
