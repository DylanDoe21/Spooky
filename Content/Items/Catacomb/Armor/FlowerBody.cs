using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Content.Items.Catacomb.Misc;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class FlowerBody : ModItem
	{
        public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 30;
			Item.height = 22;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.15f;
			player.GetCritChance(DamageClass.Generic) += 15;
			player.thorns += 1.05f;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 25)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}