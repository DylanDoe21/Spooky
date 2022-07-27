using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Buffs.Debuff;

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
    }
}