using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class EnsnaredDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npc.boss && npc.type != NPCID.EaterofWorldsHead && npc.type != NPCID.EaterofWorldsBody && npc.type != NPCID.EaterofWorldsTail)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.GreenMoss);
				
				npc.velocity.X *= 0.2f;
				npc.velocity.Y += !npc.noTileCollide ? 0.10f : 0.01f;
			}
		}
    }
}
