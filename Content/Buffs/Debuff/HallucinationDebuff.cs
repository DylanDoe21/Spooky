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
		public override void Update(Player player, ref int buffIndex)
		{
			player.velocity.X *= 0;
			player.velocity.Y += 0.35f;
			Lighting.GlobalBrightness = 0f;
		}
	}
}
