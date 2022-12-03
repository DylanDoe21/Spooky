using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class FleshWhipDefense : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Eye Lasher Defense");
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.friendly)
			{
				npc.defense = (int)(npc.defense * 0.95f);
			}
		}
    }
}
