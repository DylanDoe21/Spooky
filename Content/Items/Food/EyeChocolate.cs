using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Food
{
	public class EyeChocolate : ModItem
	{
		public override void SetStaticDefaults() 
        {
			DisplayName.SetDefault("Eye Chocolate");
			Tooltip.SetDefault("Minor improvements to all stats\n'Very soft and chewy'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
            ItemID.Sets.IsFood[Type] = true;

			// This is to show the correct frame in the inventory
			// The MaxValue argument is for the animation speed, we want it to be stuck on frame 1
			// Setting it to max value will cause it to take 414 days to reach the next frame
			// No one is going to have game open that long so this is fine
			// The second argument is the number of frames, which is 3
			// The first frame is the inventory texture, the second frame is the holding texture,
			// and the third frame is the placed texture
			Main.RegisterItemAnimation(Type, new DrawAnimationVertical(int.MaxValue, 3));

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[2] 
            {
				new Color(255, 255, 255),
				new Color(185, 178, 169)
			};
		}

		public override void SetDefaults() 
        {
			Item.DefaultToFood(28, 28, BuffID.WellFed, 18000);
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
		}
	}
}