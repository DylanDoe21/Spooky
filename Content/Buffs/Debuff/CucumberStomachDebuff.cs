using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class CucumberStomachDebuff : ModBuff
	{
        public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

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

				npc.lifeRegen -= Main.raining ? 15 : 8;
			}
		}
    }
}
