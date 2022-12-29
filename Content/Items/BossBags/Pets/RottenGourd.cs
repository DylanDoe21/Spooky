using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Content.Projectiles.Pets;
using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Items.BossBags.Pets
{
	public class RottenGourd : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rotten Gourd");
			Tooltip.SetDefault("Summons a squishy little gourd to follow you"
			+ "\n'It smells really bad'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Fish);
			Item.width = 30;
			Item.height = 42;
			Item.master = true;
			Item.rare = ItemRarityID.Master;
			Item.value = Item.buyPrice(gold: 25);
			Item.shoot = ModContent.ProjectileType<RotGourdPet>();
			Item.buffType = ModContent.BuffType<RotGourdPetBuff>();
		}

		public override bool? UseItem(Player player)
        {
			player.AddBuff(Item.buffType, 2);
			return true;
        }

		public override bool CanUseItem(Player player)
		{
			return player.miscEquips[0].IsAir;
		}
	}
}