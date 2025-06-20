using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
    public class UnstableSteroid : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
			Item.height = 58;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.maxStack = 9999;
            Item.consumable = true;
			Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
			Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 10);
            Item.UseSound = SoundID.Item3;
			Item.buffType = ModContent.BuffType<SteroidBuff>();
            Item.buffTime = 18000;
        }

		public override void UseAnimation(Player player)
		{
			player.ClearBuff(ModContent.BuffType<SteroidWeakness>());
		}
    }
}