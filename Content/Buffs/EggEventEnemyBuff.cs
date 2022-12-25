using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs
{
	public class EggEventEnemyBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Capillary buff");
		}

        private bool init;
        private int oldDamage;
        private Color oldColor;
        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!init)
            {
                oldDamage = npc.damage;
                npc.damage = (int)(npc.damage * 2f);
                oldColor = npc.color;
                npc.color = Color.Red * 0.50f;
                init = true;
            }

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.damage = oldDamage;
                npc.color = oldColor;
            }
        }
    }
}
