using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Buffs.Debuff
{
	public class EmbryoCooldown : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Embryotic Shatter");
			Description.SetDefault("The embyro's revival must recharge"
			+ "\nIncreases all damage and critical strike chance by 8%");
			Main.debuff[Type] = true;  
            Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = false;
			Main.persistentBuff[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex)
		{
			player.GetDamage<GenericDamageClass>() += 0.08f;
			player.GetCritChance<GenericDamageClass>() += 8;
		}  
    }
}
