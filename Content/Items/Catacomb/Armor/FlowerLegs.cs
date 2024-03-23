using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Content.Items.Catacomb.Misc;
using Steamworks;

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
			Item.height = 18;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

        public override void UpdateEquip(Player player)
        {
            player.endurance += 0.08f;
            player.maxFallSpeed = 18f;
			player.noKnockback = true;
        }

		public override void EquipFrameEffects(Player player, EquipType type)
        {
			player.hermesStepSound.Style = WalkSound;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantChunk>(), 12)
			.AddIngredient(ItemID.ClayBlock, 50)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
    }
}