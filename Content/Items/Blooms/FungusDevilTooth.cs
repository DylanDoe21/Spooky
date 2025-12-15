using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Blooms
{
	public class FungusDevilTooth : ModItem
	{
        public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 2;

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[2] 
            {
				new Color(201, 51, 33),
                new Color(204, 223, 216)
			};
		}

		public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 40;
            Item.consumable = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.rare = ModContent.RarityType<BloomHMRarity>();
			Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.maxStack = 9999;
			Item.scale = 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<BloomBuffsPlayer>().CanConsumeFruit("FungusDevilTooth");
        }

		public override bool? UseItem(Player player)
		{
			player.GetModPlayer<BloomBuffsPlayer>().AddBuffToList("FungusDevilTooth", 10800);

			return true;
		}
    }
}
