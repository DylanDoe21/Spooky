using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;

using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.Costume;

namespace Spooky.Core
{
    public class ItemGlobal : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.HasBuff(ModContent.BuffType<CatacombDebuff>()))
            {
                if (item.pick > 0 || item.hammer > 0 || item.axe > 0 || item.createTile > 0 || item.type == ItemID.RodofDiscord)
                {
                    return false;
                }
            }

            return true;
        }

		public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
		{
			if (item.type == ItemID.GoodieBag)
			{
				int[] Masks = new int[] { ModContent.ItemType<BananalizardHead>(), ModContent.ItemType<DylanDoeHead>(),
                ModContent.ItemType<KrakenHead>(), ModContent.ItemType<TacoHead>(), ModContent.ItemType<WaasephiHead>() };

                itemLoot.Add(ItemDropRule.OneFromOptions(30, Masks));
			}
		}
	}
}