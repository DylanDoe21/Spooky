using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class CursedDollDebuff : ModBuff
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
				if (Main.rand.NextBool(10))
				{
					Dust.NewDust(npc.position, npc.width, npc.height, DustID.Shadowflame, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, default, default, 1.5f);
				}
			}
		}
    }
}
