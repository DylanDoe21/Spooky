using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.Catacomb.Misc;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class FlowerHead : ModItem, IExtendedHelmet
	{
		public string ExtensionTexture => "Spooky/Content/Items/Catacomb/Armor/FlowerHead_RealHead";
        public Vector2 ExtensionSpriteOffset(PlayerDrawSet drawInfo) => new Vector2(0, -8f);

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