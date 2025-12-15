using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Blooms
{
	public class FungusRedCage : ModItem
	{
        public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 2;

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[3] 
            {
				new Color(201, 51, 33),
                new Color(190, 100, 35),
                new Color(251, 178, 90)
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
            return player.GetModPlayer<BloomBuffsPlayer>().CanConsumeFruit("FungusRedCage");
        }

		public override bool? UseItem(Player player)
		{
			player.GetModPlayer<BloomBuffsPlayer>().AddBuffToList("FungusRedCage", 10800);

			return true;
		}
    }
}
