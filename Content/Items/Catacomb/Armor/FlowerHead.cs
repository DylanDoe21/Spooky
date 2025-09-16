using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class FlowerHead : ModItem, ISpecialArmorDraw
	{
		public string HeadTexture => "Spooky/Content/Items/Catacomb/Armor/FlowerHeadRealHead";

        public Vector2 Offset => new Vector2(0, 4f);

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 38;
			Item.height = 36;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<FlowerBody>() && legs.type == ModContent.ItemType<FlowerLegs>();
		}

        public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.FlowerArmor");
            player.GetModPlayer<SpookyPlayer>().FlowerArmorSet = true;
            player.lifeRegen += 5;
		}

		public override void UpdateEquip(Player player)
        {
			player.manaCost -= 0.15f;
			player.maxMinions += 2;
			player.maxTurrets += 1;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}