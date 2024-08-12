using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs.Debuff
{
	public class HunterScarfMark : ModBuff
	{
        private bool initializeStats;
        Color storedColor;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                storedColor = npc.color;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] <= 1)
            {
                npc.color = storedColor;
            }
            else
            {
                Color color = new Color(125 - npc.alpha, 125 - npc.alpha, 125 - npc.alpha, 0).MultiplyRGBA(Color.OrangeRed);
                npc.color = color;
            }
        }
    }
}
