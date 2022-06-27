using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class EntityDebuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("");
			Description.SetDefault("");
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			Lighting.GlobalBrightness = 0f;
			player.GetModPlayer<SpookyPlayer>().EntityBuff = true;
		}  
    }

	//disable spawns with this debuff
	public class EntityDebuffSpawnRate : GlobalNPC
    {
        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (Main.LocalPlayer.GetModPlayer<SpookyPlayer>().EntityBuff)
            {
				pool.Clear();
            }
        }
    }
}
