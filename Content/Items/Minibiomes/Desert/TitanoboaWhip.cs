using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

using Spooky.Content.Projectiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Desert
{
    public class TitanoboaWhip : ModItem
    {
		public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/Orroboro/HissShort", SoundType.Sound) { Volume = 0.5f };

        public override void SetDefaults()
        {
            Item.damage = 100;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.width = 46;
            Item.height = 48;
			Item.useTime = 60;
			Item.useAnimation = 60;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = UseSound;
			Item.shoot = ModContent.ProjectileType<TitanoboaWhipProj>();
			Item.shootSpeed = 3.5f;
        }

		public override bool MeleePrefix() 
		{
			return true;
		}
    }
}