using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.Blooms
{
	public class SummerOrange : ModItem
	{
        public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 2;

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[2] 
            {
				new Color(228, 188, 68),
				new Color(190, 100, 35)
			};
		}

		public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 52;
            Item.consumable = true;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.rare = ModContent.RarityType<BloomRarity>();
			Item.UseSound = SoundID.Item2;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.maxStack = 9999;
			Item.scale = 0.5f;
        }

        public override bool CanUseItem(Player player)
        {
            return player.GetModPlayer<BloomBuffsPlayer>().CanConsumeFruit("SummerOrange");
        }

		public override bool? UseItem(Player player)
		{
			player.GetModPlayer<BloomBuffsPlayer>().AddBuffToList("SummerOrange", 18000);

			return true;
		}
    }
}
