using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.Blooms
{
	public class Dragonfruit : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 52;
            Item.height = 52;
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
            return player.GetModPlayer<BloomBuffsPlayer>().CanConsumeFruit("Dragonfruit");
        }

		public override bool? UseItem(Player player)
		{
			player.GetModPlayer<BloomBuffsPlayer>().AddBuffToList("Dragonfruit", 10800);

            if (player.GetModPlayer<BloomBuffsPlayer>().DragonfruitStacks < 10)
            {
			    player.GetModPlayer<BloomBuffsPlayer>().DragonfruitStacks++;
            }

			return true;
		}
    }
}
