using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Items.Food
{
	public class EyeFruit : ModItem
	{
		public override void SetStaticDefaults() 
        {
			// DisplayName.SetDefault("Eye Fruit");
			// Tooltip.SetDefault("Can grant you night vision or dangersense when eaten\nHowever, it may sometimes poison you\n'No, it does not taste like fruit or candy'");
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

			ItemID.Sets.FoodParticleColors[Item.type] = new Color[1] 
            {
				new Color(206, 206, 206)
			};
		}

		public override void SetDefaults() 
        {
			Item.DefaultToFood(28, 28, Main.rand.Next(3) == 0 ? BuffID.Dangersense : BuffID.NightOwl, 7200);
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
		}

		public override bool ConsumeItem(Player player) 
		{
			if (Main.rand.Next(5) == 0)
			{
				player.AddBuff(BuffID.Poisoned, 900);
			}

			return true;
		}
	}
}