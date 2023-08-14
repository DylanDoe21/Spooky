using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class FlowerBody : ModItem
	{
        public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 28;
			Item.height = 20;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 1);
		}

		public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) += 0.18f;
			player.GetCritChance(DamageClass.Generic) += 18;
			player.thorns += 1.05f;
        }
    }
}