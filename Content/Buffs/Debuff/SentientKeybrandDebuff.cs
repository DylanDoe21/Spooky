using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Buffs.Debuff
{
	public class SentientKeybrandDebuff : ModBuff
	{
		public override string Texture => "Spooky/Content/Buffs/Debuff/DebuffPlaceholder";

		private bool initializeDefense;
        private int storedDefense;

		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;
		}

		public override void Update(NPC npc, ref int buffIndex)
		{
			//store the npcs original defense value
			if (!initializeDefense)
            {
                storedDefense = npc.defense;
				initializeDefense = true;
			}

			//lower the npcs defense by the stack of sentient keybrand defense reduction the npc has
			if (!npc.GetGlobalNPC<NPCGlobal>().InitializedKeybrandDefense)
			{
				npc.defense = storedDefense;

				//if the npcs normal defense is lower than the keybrand defense reduction stack number, set its defense to zero manually to prevent negative defense
				if (npc.GetGlobalNPC<NPCGlobal>().KeybrandDefenseStacks > storedDefense)
				{
					npc.defense = 0;
				}
				//otherwise just normally lower then npcs defense by the stack amount
				else
				{
					npc.defense -= npc.GetGlobalNPC<NPCGlobal>().KeybrandDefenseStacks;
				}

				npc.GetGlobalNPC<NPCGlobal>().InitializedKeybrandDefense = true;
			}
		}
	}
}
