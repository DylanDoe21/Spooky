using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Food
{
	public class EyeFruit : ModItem
	{
		public override void SetStaticDefaults() 
        {
			Item.ResearchUnlockCount = 5;
			
            ItemID.Sets.IsFood[Type] = true;

			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[1] 
            {
				new Color(206, 206, 206)
			};
		}

		public override void SetDefaults() 
        {
			Item.DefaultToFood(28, 28, Main.rand.NextBool(2) ? BuffID.Dangersense : BuffID.NightOwl, 7200);
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ConsumeItem(Player player) 
		{
			if (Main.rand.NextBool(5))
			{
				player.AddBuff(BuffID.Poisoned, 900);
			}

			return true;
		}
	}
}