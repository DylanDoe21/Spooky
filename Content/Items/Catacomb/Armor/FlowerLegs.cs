using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Spooky.Content.Items.Catacomb.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class FlowerLegs : ModItem
	{
		public static readonly SoundStyle WalkSound = new("Spooky/Content/Sounds/FlowerPotWalk", SoundType.Sound);

        public override void SetStaticDefaults()
        {
			if (Main.netMode == NetmodeID.Server)
			{
				return;
			}

            int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);

            ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlot] = true;
        }

        public override void SetDefaults() 
		{
			Item.defense = 15;
			Item.width = 24;
			Item.height = 16;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 1);
		}

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.08f;
            player.maxFallSpeed = 18f;
			player.noKnockback = true;
			player.hermesStepSound.Style = WalkSound;
        }
    }
}