using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Items.SpookyBiome.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class WizardGangsterHead2 : ModItem, ISpecialHelmetDraw
	{
		public string HeadTexture => "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHead2_Hat";

		public string GlowTexture => "Spooky/Content/Items/SpookyBiome/Armor/WizardGangsterHead_Glow";

		public Vector2 Offset => new Vector2(0, 4f);

		public override void SetDefaults() 
		{
			Item.defense = 2;
			Item.width = 30;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<WizardGangsterBody>() && legs.type == ModContent.ItemType<WizardGangsterLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawOutlines = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.WizardGangsterArmor2");

			if (player.HasItem(ItemID.PlatinumCoin))
			{
				player.manaCost -= 0.2f;
			}
			else
			{
				float bonusPerGold = 0.02f;
				int numGoldCoins = player.CountItem(ItemID.GoldCoin);

				if (numGoldCoins < 10)
				{
					player.manaCost -= bonusPerGold * numGoldCoins;
				}
				else
				{
					player.manaCost -= 0.2f;
				}
			}
        }

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Magic) += 0.05f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddRecipeGroup("SpookyMod:GoldBars", 8)
			.AddIngredient(ItemID.Silk, 8)
			.AddIngredient(ModContent.ItemType<SpookyGlowshroom>(), 20)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}