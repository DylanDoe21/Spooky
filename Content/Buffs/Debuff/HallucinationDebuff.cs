using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.NPCs.Hallucinations;

namespace Spooky.Content.Buffs.Debuff
{
	public class HallucinationDebuff1 : ModBuff
	{
		public override void SetStaticDefaults()
		{
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			Lighting.GlobalBrightness = 0f;
		}
    }

	public class HallucinationDebuff2 : HallucinationDebuff1
	{
	}

	public class HallucinationDebuff3 : HallucinationDebuff1
	{
	}

	public class HallucinationDebuff4 : HallucinationDebuff1
	{
		bool initializeVolume;
		float saveMusicVolume;

		public override void Update(Player player, ref int buffIndex)
		{
			//save the players music volume
			if (!initializeVolume)
			{
				saveMusicVolume = Main.musicVolume;
				initializeVolume = true;
			}

			//after saving the music volume, force it all the way up during the flesh piles dialogue
			if (player.buffTime[buffIndex] > 2)
			{
				Main.musicVolume = 1f;
			}
			else
			{
				Main.musicVolume = saveMusicVolume;
			}

			if (!NPC.AnyNPCs(ModContent.NPCType<TheFlesh>()))
			{
				player.buffTime[buffIndex] = 0;
			}

			Lighting.GlobalBrightness = 0f;
			player.velocity *= 0;
		}
	}
}
