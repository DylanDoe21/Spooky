using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.Pets
{
	public class RottenGourd : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Rotten Gourd");
			/* Tooltip.SetDefault("Summons a squishy little gourd to follow you"
			+ "\n'Its small, but smells awful'"); */
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 30;
			Item.height = 42;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 5);
			Item.shoot = ModContent.ProjectileType<RotGourdPet>();
			Item.buffType = ModContent.BuffType<RotGourdPetBuff>();
		}

		public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }
	}
}