using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Catacomb.Blooms
{
	public class DandelionHerd : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 56;
            Item.height = 68;
            Item.consumable = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.maxStack = 9999;
			Item.scale = 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<BloomBuffsPlayer>().CanConsumeFruit("DandelionHerd");
        }

		public override bool? UseItem(Player player)
		{
			player.GetModPlayer<BloomBuffsPlayer>().AddBuffToList("DandelionHerd", 18000);

			return true;
		}
    }
}
