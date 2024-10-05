using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs.Debuff
{
	public class PeptoDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeStats;
        private int storedDamage;
        private int storedDefense;
        Color storedColor;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!initializeStats)
            {
                npc.damage = (int)(npc.damage * 0.8f);
                npc.defense = (int)(npc.defense * 0.75f);

                initializeStats = true;
            }

            if (npc.buffTime[buffIndex] > 5)
            {
                Color color = npc.GetAlpha(Color.DeepPink);
                npc.color = color;
            }
        }
    }
}