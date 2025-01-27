using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Buffs.Debuff
{
	public class BackroomsDecay : ModBuff
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
				if (npc.life > (npc.lifeMax / 2))
				{
					if (npc.lifeRegen > 0)
					{
						npc.lifeRegen = 0;
					}

					npc.lifeRegen -= npc.IsTechnicallyBoss() ? 15 : 30;
				}
			}
		}
    }
}
