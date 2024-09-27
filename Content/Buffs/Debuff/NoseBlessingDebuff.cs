using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class NoseBlessingDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
			if (npc.buffTime[buffIndex] < 5)
            {
				npc.AddBuff(ModContent.BuffType<NoseBlessingDebuffCooldown>(), 300);
			}
		}
    }
}
