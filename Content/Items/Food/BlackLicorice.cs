using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Food
{
	public class BlackLicorice : ModItem
	{
		public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 5;

            ItemID.Sets.IsFood[Type] = true;

			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[2] 
            {
				new Color(32, 30, 34),
				new Color(91, 88, 93)
			};
		}

        public override void SetDefaults() 
        {
			Item.DefaultToFood(28, 28, BuffID.Weak, 3600);
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
		}
	}
}