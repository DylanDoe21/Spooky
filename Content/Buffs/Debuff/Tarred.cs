using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs.Debuff
{
	public class Tarred : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeStats;
        Color storedColor;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats && npc.buffTime[buffIndex] >= 5)
            {
                storedColor = npc.color;

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] < 5)
            {
                npc.color = storedColor;
            }
            else
            {
                Color color = npc.GetAlpha(Color.Black);
                npc.color = color;
            }
        }
    }
}
