using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Buffs.Debuff
{
	public class BeePollinationProcessBuff : ModBuff
	{
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }
    }
}
