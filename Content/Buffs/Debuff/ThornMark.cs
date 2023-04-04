using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class ThornMark : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Thorn Mark");
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.friendly)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.RedTorch);
			}
		}
    }
}
