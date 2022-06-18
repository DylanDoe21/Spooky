using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SpookyBody : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Spooky Horseman's Chestmail");
			Tooltip.SetDefault("3% increased damage"
			+ "\nEnemies are more likely to target you");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.buyPrice(gold: 2);
			Item.rare = 1;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage<GenericDamageClass>() += 0.03f;
			player.aggro += 120;
		}
	}
}
